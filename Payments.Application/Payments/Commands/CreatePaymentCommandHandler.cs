using MediatR;
using Payments.Application.Common.Interfaces;
using Payments.Domain.Entities;

namespace Payments.Application.Payments.Commands
{
    public sealed class CreatePaymentCommandHandler(
        IOrdersWriteRepository ordersWrite,
        IWriteRepository<Payment> paymentsWrite,
        ICurrentUserProvider currentUser) : IRequestHandler<CreatePaymentCommand, Guid>
    {
        public async Task<Guid> Handle(CreatePaymentCommand request, CancellationToken ct)
        {
            var userId = currentUser.GetCurrentUser();

            var order = await ordersWrite.GetTrackingByIdAsync(request.OrderId, ct)
                ?? throw new ApplicationException("order.not_found");

            if (!order.EnsureOwner(userId).IsSuccess)
                throw new ApplicationException("auth.forbidden");

            if (!order.EnsurePayable().IsSuccess)
                throw new ApplicationException("order.not_payable");

            var paymentResult = Payment.Create(order.Id, userId, order.Total, request.IdempotencyKey);
            if (!paymentResult.IsSuccess)
                throw new ApplicationException(paymentResult.Error.Code);

            var payment = paymentResult.Value!;
            await paymentsWrite.AddAsync(payment, ct);

            return payment.Id;
        }
    }
}
