using Payments.Application.Common.Abstractions;

namespace Payments.Application.Payments.Commands
{
    public sealed record CreatePaymentCommand(Guid OrderId, string IdempotencyKey) : ICommand<Guid>;
}
