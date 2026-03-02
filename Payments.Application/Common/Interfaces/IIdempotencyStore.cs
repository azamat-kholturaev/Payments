namespace Payments.Application.Common.Interfaces
{
    public interface IIdempotencyStore
    {
        Task<IdemResponse?> GetAsync(Guid userId, string key, string scope, CancellationToken ct);
        Task<bool> TrySaveAsync(Guid userId, string key, string scope, int statusCode, string json, CancellationToken ct);
    }

    public sealed record IdemResponse(int StatusCode, string Json);
}
