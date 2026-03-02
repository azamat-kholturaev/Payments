namespace Payments.Infrastructure.Common.Persistence.Rows
{
    public class CurrencyRow
    {
        public string Code { get; set; } = default!;
        public int NumericCode { get; set; }
        public int MinorUnits { get; set; }
        public string Name { get; set; } = default!;
        public bool IsActive { get; set; } = true;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
