using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payments.Infrastructure.Common.Persistence.Rows;

namespace Payments.Infrastructure.Common.Persistence.Configurations
{
    internal sealed class CurrencyRowConfiguration : IEntityTypeConfiguration<CurrencyRow>
    {
        public void Configure(EntityTypeBuilder<CurrencyRow> e)
        {
            e.ToTable(DbSchema.Tables.Currencies);
            e.HasKey(x => x.Code);

            e.Property(x => x.Code)
                .HasColumnName(DbSchema.Columns.Code)
                .HasMaxLength(DbSchema.Lengths.CurrencyCode);

            e.Property(x => x.NumericCode)
                .HasColumnName(DbSchema.Columns.NumericCode)
                .IsRequired();

            e.Property(x => x.MinorUnits)
                .HasColumnName(DbSchema.Columns.MinorUnits)
                .IsRequired();

            e.Property(x => x.Name)
                .HasColumnName(DbSchema.Columns.Name)
                .HasMaxLength(DbSchema.Lengths.CurrencyName)
                .IsRequired();

            e.Property(x => x.IsActive)
                .HasColumnName(DbSchema.Columns.IsActive)
                .IsRequired();

            e.Property(x => x.UpdatedAt)
                .HasColumnName(DbSchema.Columns.UpdatedAt)
                .IsRequired();

            e.HasIndex(x => x.IsActive);
        }
    }
}
