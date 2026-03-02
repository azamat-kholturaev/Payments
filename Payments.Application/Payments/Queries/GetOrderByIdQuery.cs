using MediatR;

namespace Payments.Application.Payments.Queries
{
    public sealed record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDetailsDto>;

    public sealed record OrderDetailsDto(Guid Id, decimal Amount, string Currency, string Status);
}
