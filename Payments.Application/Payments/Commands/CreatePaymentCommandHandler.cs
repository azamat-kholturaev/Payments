using MediatR;
using Payments.Application.Common.Exceptions;
using Payments.Application.Common.Interfaces;
using Payments.Domain.Entities;
using System.Text.Json;

namespace Payments.Application.Payments.Commands
{
    public sealed class CreatePaymentCommandHandler(IOrdersWriteRepository orders,
                                                    IPaymentsWriteRepository payments,
                                                    ICurrentUserProvider currentUser,
                                                    IIdempotencyStore idempotencyStore) : IRequestHandler<CreatePaymentCommand, PaymentDto>
    {
        public async Task<PaymentDto> Handle(CreatePaymentCommand request, CancellationToken ct)
        {
            var userId = currentUser.GetCurrentUser();
            var scope = $"payments:create:{request.OrderId}";

            var replay = await idempotencyStore.GetAsync(userId, request.IdempotencyKey, scope, ct);
            if (replay is not null)
                return JsonSerializer.Deserialize<PaymentDto>(replay.Json)!;

            var order = await orders.GetTrackingByIdAsync(request.OrderId, ct)
                ?? throw new AppException("order.not_found", "Order not found", 404);

            var owner = order.EnsureOwner(userId);
            if (!owner.IsSuccess)
                throw new AppException("order.forbidden", owner.Error.Message, 403);

            var payable = order.EnsurePayable();
            if (!payable.IsSuccess)
                throw new AppException("order.not_payable", payable.Error.Message, 409);

            var paymentResult = Payment.Create(order.Id, userId, order.Total, request.IdempotencyKey);

            if (!paymentResult.IsSuccess)
                throw new AppException("payment.invalid", paymentResult.Error.Message, 400);

            var payment = paymentResult.Value!;
            await payments.AddAsync(payment, ct);

            var dto = new PaymentDto(payment.Id,
                                     payment.OrderId,
                                     payment.Amount,
                                     payment.Currency,
                                     payment.Status.ToString().ToLowerInvariant(),
                                     payment.CreatedAt);

            await idempotencyStore.TrySaveAsync(userId, request.IdempotencyKey, scope, 201, JsonSerializer.Serialize(dto), ct);

            return dto;
        }
    }
}
