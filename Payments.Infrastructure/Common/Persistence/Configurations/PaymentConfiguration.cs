using Payments.Infrastructure.Common.Persistence.Converters;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Payments.Domain.Entities;

namespace Payments.Infrastructure.Common.Persistence.Configurations
{
    internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> e)
        {
            e.ToTable(DbSchema.Tables.Payments);
            e.HasKey(x => x.Id);

            e.Property(x => x.OrderId)
                .HasColumnName(DbSchema.Columns.OrderId)
                .IsRequired();

            e.Property(x => x.UserId)
                .HasColumnName(DbSchema.Columns.UserId)
                .IsRequired();

            e.Property(x => x.Amount)
                .HasColumnName(DbSchema.Columns.Amount)
                .HasColumnType("numeric(18,2)")
                .IsRequired();

            e.Property(x => x.Currency)
                .HasColumnName(DbSchema.Columns.Currency)
                .HasMaxLength(DbSchema.Lengths.CurrencyCode)
                .IsRequired();

            e.Property(x => x.Status)
                .HasColumnName(DbSchema.Columns.Status)
                .HasConversion<string>()
                .HasMaxLength(DbSchema.Lengths.Status)
                .IsRequired();

            e.Property(x => x.CreatedAt)
                .HasColumnName(DbSchema.Columns.CreatedAt)
                .IsRequired();

            e.Property(x => x.IdempotencyKey)
                .HasConversion(ValueConverters.IdempotencyKeyConverter)
                .HasColumnName(DbSchema.Columns.IdempotencyKey)
                .HasMaxLength(DbSchema.Lengths.IdempotencyKey)
                .IsRequired();

            e.HasIndex(x => new { x.OrderId, x.IdempotencyKey }).IsUnique();

            e.Property(x => x.ProviderPaymentId)
                .HasColumnName(DbSchema.Columns.ProviderPaymentId)
                .HasMaxLength(DbSchema.Lengths.ProviderPaymentId);

            e.Property(x => x.FailureReason)
                .HasColumnName(DbSchema.Columns.FailureReason)
                .HasMaxLength(DbSchema.Lengths.FailureReason);

            e.HasIndex(x => x.OrderId);
            e.HasIndex(x => new { x.OrderId, x.CreatedAt });
            e.HasIndex(x => x.UserId);

            // один Successful платеж на заказ (partial unique index)
            e.HasIndex(p => p.OrderId)
                .IsUnique()
                .HasDatabaseName("ux_payments_one_success_per_order")
                .HasFilter("status = 'Successful'");
        }
    }
}
