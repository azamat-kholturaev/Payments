using MediatR;
using Payments.Application.Common.Interfaces;
using Payments.Domain.Entities;

namespace Payments.Application.Payments.Commands
{
    public sealed class ConfirmPaymentCommandHandler(
        IOrdersWriteRepository ordersWrite,
        IWriteRepository<Payment> paymentsWrite,
        ICurrentUserProvider currentUser) : IRequestHandler<ConfirmPaymentCommand, Unit>
    {
        public async Task<Unit> Handle(ConfirmPaymentCommand request, CancellationToken ct)
        {
            var userId = currentUser.GetCurrentUser();

            var payment = await paymentsWrite.GetTrackingByIdAsync(request.PaymentId, ct)
                ?? throw new ApplicationException("payment.not_found");

            if (!payment.EnsureOwner(userId).IsSuccess)
                throw new ApplicationException("auth.forbidden");

            var order = await ordersWrite.GetForUpdateAsync(payment.OrderId, ct)
                ?? throw new ApplicationException("order.not_found");

            if (!order.EnsureOwner(userId).IsSuccess)
                throw new ApplicationException("auth.forbidden");

            var payable = order.EnsurePayable();
            if (!payable.IsSuccess)
                throw new ApplicationException(payable.Error.Code);

            var markPayment = payment.MarkSuccessful(request.ProviderPaymentId);
            if (!markPayment.IsSuccess)
                throw new ApplicationException(markPayment.Error.Code);

            var orderPaid = order.ApplySuccessfulPayment();
            if (!orderPaid.IsSuccess)
                throw new ApplicationException(orderPaid.Error.Code);

            return Unit.Value;
        }
    }
}
