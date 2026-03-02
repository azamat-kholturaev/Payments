using MediatR;

namespace Payments.Application.Orders.Queries;

public sealed record GetOrderQuery(Guid OrderId) : IRequest<OrderDto>;

public sealed record OrderDto(Guid Id, Guid UserId, decimal Amount, string Currency, string Status);
