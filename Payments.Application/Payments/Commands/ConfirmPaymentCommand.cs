using Payments.Application.Common.Abstractions;

namespace Payments.Application.Payments.Commands;

public sealed record ConfirmPaymentCommand(Guid PaymentId, string IdempotencyKey) : ICommand<ConfirmPaymentResultDto>;

public sealed record ConfirmPaymentResultDto(Guid PaymentId, Guid OrderId, string PaymentStatus, string OrderStatus, string? ProviderPaymentId, string? FailureReason);
