using MediatR;
using Microsoft.EntityFrameworkCore;
using Payments.Application.Common.Interfaces;
using Payments.Domain.Entities;

namespace Payments.Application.Payments.Queries
{
    public sealed class ListOrderPaymentsQueryHandler(
        IReadRepository<Order> ordersRead,
        IReadRepository<Payment> paymentsRead,
        ICurrentUserProvider currentUser) : IRequestHandler<ListOrderPaymentsQuery, IReadOnlyList<PaymentDto>>
    {
        public async Task<IReadOnlyList<PaymentDto>> Handle(ListOrderPaymentsQuery request, CancellationToken ct)
        {
            var userId = currentUser.GetCurrentUser();

            var orderExists = await ordersRead.Query().AnyAsync(x => x.Id == request.OrderId && x.UserId == userId, ct);
            if (!orderExists)
                throw new ApplicationException("order.not_found");

            return await paymentsRead.Query()
                .Where(p => p.OrderId == request.OrderId && p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new PaymentDto(
                    p.Id,
                    p.OrderId,
                    p.Amount,
                    p.Currency,
                    p.Status.ToString(),
                    p.IdempotencyKey.Value,
                    p.CreatedAt,
                    p.ProviderPaymentId,
                    p.FailureReason))
                .ToListAsync(ct);
        }
    }
}
