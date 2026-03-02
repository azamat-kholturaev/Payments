namespace Payments.Infrastructure.Common.Persistence
{
    internal static class DbSchema
    {
        internal static class Tables
        {
            public const string Users = "users";
            public const string Orders = "orders";
            public const string Payments = "payments";
            public const string Currencies = "currencies";
            public const string Idempotency = "idempotency";
        }

        internal static class Columns
        {
            public const string Email = "email";
            public const string PasswordHash = "password_hash";
            public const string CreatedAt = "created_at";

            public const string UserId = "user_id";
            public const string OrderId = "order_id";
            public const string Status = "status";

            public const string Amount = "amount";
            public const string Currency = "currency";

            public const string IdempotencyKey = "idempotency_key";
            public const string ProviderPaymentId = "provider_payment_id";
            public const string FailureReason = "failure_reason";

            public const string Code = "code";
            public const string NumericCode = "numeric_code";
            public const string MinorUnits = "minor_units";
            public const string Name = "name";
            public const string IsActive = "is_active";
            public const string UpdatedAt = "updated_at";

            public const string Key = "key";
            public const string Scope = "scope";
            public const string StatusCode = "status_code";
            public const string ResponseJson = "response_json";
            public const string ExpiresAt = "expires_at";
        }

        internal static class Lengths
        {
            public const int Email = 320;
            public const int PasswordHash = 500;

            public const int Status = 16;
            public const int CurrencyCode = 3;

            public const int IdempotencyKey = 128;
            public const int ProviderPaymentId = 128;

            public const int FailureReason = 500;

            public const int CurrencyName = 128;

            public const int IdempotencyScope = 200;
        }
    }
}
