using Payments.Application.Common.Abstractions;

namespace Payments.Application.Orders.Commands
{
    public record CreateOrderCommand(Guid UserId, decimal Amount, string Currency) : ICommand<Guid>;
}
