using Payments.Application.Common.Abstractions;
using Payments.Domain.Common;

namespace Payments.Application.Orders.Commands
{
    public record CreateOrderCommand(decimal Amount, string Currency) : ICommand<Result<Guid>>;
}
