namespace Payments.Infrastructure.Common.Persistence.Rows
{
    public class IdempotencyRow
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();

        public Guid UserId { get; set; }
        public string Key { get; set; } = default!;
        public string Scope { get; set; } = default!;

        public int StatusCode { get; set; }
        public string ResponseJson { get; set; } = default!;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset ExpiresAt { get; set; } = DateTimeOffset.UtcNow.AddHours(24);
    }
}
