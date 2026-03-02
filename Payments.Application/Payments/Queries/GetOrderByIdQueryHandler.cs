using MediatR;
using Microsoft.EntityFrameworkCore;
using Payments.Application.Common.Interfaces;
using Payments.Domain.Entities;

namespace Payments.Application.Payments.Queries
{
    public sealed class GetOrderByIdQueryHandler(
        IReadRepository<Order> ordersRead,
        ICurrentUserProvider currentUser) : IRequestHandler<GetOrderByIdQuery, OrderDetailsDto>
    {
        public async Task<OrderDetailsDto> Handle(GetOrderByIdQuery request, CancellationToken ct)
        {
            var userId = currentUser.GetCurrentUser();

            var order = await ordersRead.Query()
                .Where(o => o.Id == request.OrderId && o.UserId == userId)
                .Select(o => new OrderDetailsDto(o.Id, o.Total.Amount, o.Total.Currency.Code, o.Status.ToString()))
                .FirstOrDefaultAsync(ct)
                ?? throw new ApplicationException("order.not_found");

            return order;
        }
    }
}
