namespace Payments.API.Contracts
{
    public sealed record CreatePaymentRequest(Guid OrderId, string IdempotencyKey);
    public sealed record CreatePaymentResponse(Guid PaymentId);
    public sealed record ConfirmPaymentRequest(string ProviderPaymentId);
}
