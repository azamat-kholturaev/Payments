using MediatR;
using Payments.Application.Common.Exceptions;
using Payments.Application.Common.Interfaces;
using Payments.Domain.Entities;

namespace Payments.Application.Orders.Queries;

public sealed class GetOrderQueryHandler(IReadRepository<Order> orders, ICurrentUserProvider currentUser) : IRequestHandler<GetOrderQuery, OrderDto>
{
    public async Task<OrderDto> Handle(GetOrderQuery request, CancellationToken ct)
    {
        var order = await orders.GetByIdAsync(request.OrderId, ct)
            ?? throw new AppException("order.not_found", "Order not found", 404);

        var userId = currentUser.GetCurrentUser();
        var owner = order.EnsureOwner(userId);
        if (!owner.IsSuccess)
            throw new AppException("order.forbidden", owner.Error.Message, 403);

        return new OrderDto(order.Id, order.UserId, order.Total.Amount, order.Total.Currency.Code, order.Status.ToString().ToLowerInvariant());
    }
}
