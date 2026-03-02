using MediatR;
using Payments.Application.Common.Abstractions;

namespace Payments.Application.Payments.Commands
{
    public sealed record ConfirmPaymentCommand(Guid PaymentId, string ProviderPaymentId) : ICommand<Unit>;
}
