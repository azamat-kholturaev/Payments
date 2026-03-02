namespace Payments.Application.Common.Interfaces
{
    public interface IPaymentProviderClient
    {
        Task<ProviderResult> ConfirmAsync(Guid paymentId, decimal amount, string currency, CancellationToken ct);
    }

    public sealed record ProviderResult(bool IsSuccess, string ProviderPaymentId, string? Error);
}
