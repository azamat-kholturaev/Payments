namespace Payments.Application.Common.Interfaces
{
    public sealed record CurrencyInfo(string Code, int NumericCode, int MinorUnits, string Name, bool IsActive);

    public interface ICurrencyCatalog
    {
        Task<bool> IsSupportedAsync(string code, CancellationToken ct);
        Task<CurrencyInfo?> GetAsync(string code, CancellationToken ct);
    }

}
