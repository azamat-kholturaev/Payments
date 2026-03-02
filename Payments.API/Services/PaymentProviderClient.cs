using Payments.Application.Common.Interfaces;

namespace Payments.API.Services
{
    public sealed class PaymentProviderClient : IPaymentProviderClient
    {
        public async Task<ProviderResult> ConfirmAsync(Guid paymentId, decimal amount, string currency, CancellationToken ct)
        {
            await Task.Delay(50, ct);

            if (amount <= 0)
                return new ProviderResult(false, string.Empty, "invalid_amount");

            return new ProviderResult(true, $"prov_{paymentId:N}", null);
        }
    }
}
