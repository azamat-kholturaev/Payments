using MediatR;
using Payments.Application.Payments.Queries;

namespace Payments.Application.Orders.Queries
{
    public sealed record GetOrderQuery(Guid OrderId) : IRequest<OrderDto>;

    public sealed record OrderDto(Guid Id, decimal Amount, string Currency, string Status);


}
