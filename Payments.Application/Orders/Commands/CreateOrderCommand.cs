using Payments.Application.Common.Abstractions;

namespace Payments.Application.Orders.Commands
{
    public record CreateOrderCommand(decimal Amount, string Currency) : ICommand<Guid>;
}
