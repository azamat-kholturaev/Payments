namespace Payments.Infrastructure.Authentication.TokenGenerator
{
    public sealed class JwtSettings
    {
        public const string Section = "Jwt";

        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public string Secret { get; set; } = default!;
        public int ExpirationMinutes { get; set; }
    }
}
