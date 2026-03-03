using MediatR;
using Payments.Application.Common.Exceptions;
using Payments.Application.Common.Interfaces;
using Payments.Domain.Common;
using Payments.Domain.Entities;
using System.Text.Json;

namespace Payments.Application.Payments.Commands
{
    public sealed class ConfirmPaymentCommandHandler(IPaymentsWriteRepository payments,
                                                     IOrdersWriteRepository orders,
                                                     ICurrentUserProvider currentUser,
                                                     IIdempotencyStore idempotencyStore,
                                                     IPaymentProviderClient provider)
        : IRequestHandler<ConfirmPaymentCommand, ConfirmPaymentResultDto>
    {
        public async Task<ConfirmPaymentResultDto> Handle(ConfirmPaymentCommand request, CancellationToken ct)
        {
            var userId = currentUser.GetCurrentUser();
            var scope = $"payments:confirm:{request.PaymentId}";

            var replay = await idempotencyStore.GetAsync(userId, request.IdempotencyKey, scope, ct);
            if (replay is not null)
                return JsonSerializer.Deserialize<ConfirmPaymentResultDto>(replay.Json)!;

            var payment = await payments.GetTrackingByIdAsync(request.PaymentId, ct)
                ?? throw new AppException("payment.not_found", "Payment not found", 404);

            var owner = payment.EnsureOwner(userId);
            if (!owner.IsSuccess)
                throw new AppException("payment.forbidden", owner.Error.Message, 403);

            var order = await orders.GetForUpdateAsync(payment.OrderId, ct)
                ?? throw new AppException("order.not_found", "Order not found", 404);

            var orderOwner = order.EnsureOwner(userId);
            if (!orderOwner.IsSuccess)
                throw new AppException("order.forbidden", orderOwner.Error.Message, 403);

            if (order.Status == OrderStatus.Paid)
            {
                if (payment.Status == PaymentStatus.Successful)
                {
                    var already = new ConfirmPaymentResultDto(payment.Id, payment.OrderId, "successful", "paid", payment.ProviderPaymentId, payment.FailureReason);
                    await idempotencyStore.TrySaveAsync(userId, request.IdempotencyKey, scope, 200, JsonSerializer.Serialize(already), ct);
                    return already;
                }

                if (payment.Status == PaymentStatus.Pending)
                {
                    var failedResult = payment.MarkFailed("already_paid_exists");
                    if (!failedResult.IsSuccess)
                        throw new AppException("payment.not_pending", failedResult.Error.Message, 409);
                }

                var conflictDto = new ConfirmPaymentResultDto(
                    payment.Id,
                    order.Id,
                    payment.Status.ToString().ToLowerInvariant(),
                    order.Status.ToString().ToLowerInvariant(),
                    payment.ProviderPaymentId,
                    payment.FailureReason ?? "already_paid_exists");

                await idempotencyStore.TrySaveAsync(userId, request.IdempotencyKey, scope, 200, JsonSerializer.Serialize(conflictDto), ct);
                return conflictDto;
            }

            if (payment.Status == PaymentStatus.Successful)
            {
                var already = new ConfirmPaymentResultDto(payment.Id, payment.OrderId, "successful", "paid", payment.ProviderPaymentId, payment.FailureReason);
                await idempotencyStore.TrySaveAsync(userId, request.IdempotencyKey, scope, 200, JsonSerializer.Serialize(already), ct);
                return already;
            }

            if (payment.Status == PaymentStatus.Failed)
            {
                if (string.Equals(payment.FailureReason, "already_paid_exists", StringComparison.OrdinalIgnoreCase))
                {
                    var conflictDto = new ConfirmPaymentResultDto(payment.Id, order.Id, "failed", order.Status.ToString().ToLowerInvariant(), payment.ProviderPaymentId, payment.FailureReason);
                    await idempotencyStore.TrySaveAsync(userId, request.IdempotencyKey, scope, 200, JsonSerializer.Serialize(conflictDto), ct);
                    return conflictDto;
                }

                throw new AppException("payment.not_pending", "Only pending payment can be confirmed", 409);
            }

            var providerResult = await provider.ConfirmAsync(payment.Id, payment.Amount, payment.Currency, ct);

            Result markResult;
            Result orderResult;
            if (providerResult.IsSuccess)
            {
                markResult = payment.MarkSuccessful(providerResult.ProviderPaymentId);
                if (!markResult.IsSuccess)
                    throw new AppException("payment.not_pending", markResult.Error.Message, 409);

                orderResult = order.ApplySuccessfulPayment();
                if (!orderResult.IsSuccess)
                    throw new AppException("order.not_payable", orderResult.Error.Message, 409);
            }
            else
            {
                markResult = payment.MarkFailed(providerResult.Error ?? "provider_unavailable");
                if (!markResult.IsSuccess)
                    throw new AppException("payment.not_pending", markResult.Error.Message, 409);
            }

            var dto = new ConfirmPaymentResultDto(
                payment.Id,
                order.Id,
                payment.Status.ToString().ToLowerInvariant(),
                order.Status.ToString().ToLowerInvariant(),
                payment.ProviderPaymentId,
                payment.FailureReason);

            await idempotencyStore.TrySaveAsync(userId, request.IdempotencyKey, scope, 200, JsonSerializer.Serialize(dto), ct);

            return dto;
        }
    }
}
