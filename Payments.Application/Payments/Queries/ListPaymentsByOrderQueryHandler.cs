using MediatR;
using Payments.Application.Common.Exceptions;
using Payments.Application.Common.Interfaces;
using Payments.Domain.Entities;

namespace Payments.Application.Payments.Queries
{
    public sealed class ListPaymentsByOrderQueryHandler(IReadRepository<Order> orders, IPaymentsWriteRepository payments,
                                                        ICurrentUserProvider currentUser) : IRequestHandler<ListPaymentsByOrderQuery,
                                                            IReadOnlyList<PaymentListItemDto>>
    {
        public async Task<IReadOnlyList<PaymentListItemDto>> Handle(ListPaymentsByOrderQuery request, CancellationToken ct)
        {
            var order = await orders.GetByIdAsync(request.OrderId, ct)
                ?? throw new AppException("order.not_found", "Order not found", 404);

            var userId = currentUser.GetCurrentUser();
            var owner = order.EnsureOwner(userId);
            if (!owner.IsSuccess)
                throw new AppException("order.forbidden", owner.Error.Message, 403);

            var list = await payments.GetByOrderIdAsync(order.Id, ct);
            return [.. list
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new PaymentListItemDto(
                    x.Id,
                    x.Status.ToString().ToLowerInvariant(),
                    x.Amount,
                    x.Currency,
                    x.CreatedAt,
                    x.ProviderPaymentId,
                    x.FailureReason))];
        }
    }
}
