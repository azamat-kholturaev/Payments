using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Payments.Domain.Entities;

namespace Payments.Infrastructure.Common.Persistence.Configurations
{
    internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> e)
        {
            e.ToTable(DbSchema.Tables.Orders);
            e.HasKey(x => x.Id);

            e.Property(x => x.UserId)
                .HasColumnName(DbSchema.Columns.UserId)
                .IsRequired();

            e.Property(x => x.Status)
                .HasColumnName(DbSchema.Columns.Status)
                .HasConversion<string>()
                .HasMaxLength(DbSchema.Lengths.Status)
                .IsRequired();

            e.OwnsOne(x => x.Total, m =>
            {
                m.Property(p => p.Amount)
                    .HasColumnName(DbSchema.Columns.Amount)
                    .HasColumnType("numeric(18,2)")
                    .IsRequired();

                m.OwnsOne(p => p.Currency, c =>
                {
                    c.Property(x => x.Code)
                        .HasColumnName(DbSchema.Columns.Currency)
                        .HasMaxLength(DbSchema.Lengths.CurrencyCode)
                        .IsRequired();
                });
            });

            e.HasIndex(x => new { x.UserId, x.Id });
            e.HasIndex(x => x.Status);
        }
    }
}
