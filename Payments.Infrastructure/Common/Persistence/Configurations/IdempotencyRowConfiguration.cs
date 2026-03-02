using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payments.Infrastructure.Common.Persistence.Rows;
using Microsoft.EntityFrameworkCore;

namespace Payments.Infrastructure.Common.Persistence.Configurations
{
    internal sealed class IdempotencyRowConfiguration : IEntityTypeConfiguration<IdempotencyRow>
    {
        public void Configure(EntityTypeBuilder<IdempotencyRow> e)
        {
            e.ToTable(DbSchema.Tables.Idempotency);
            e.HasKey(x => x.Id);

            e.Property(x => x.UserId)
                .HasColumnName(DbSchema.Columns.UserId)
                .IsRequired();

            e.Property(x => x.Key)
                .HasColumnName(DbSchema.Columns.Key)
                .HasMaxLength(DbSchema.Lengths.IdempotencyKey)
                .IsRequired();

            e.Property(x => x.Scope)
                .HasColumnName(DbSchema.Columns.Scope)
                .HasMaxLength(DbSchema.Lengths.IdempotencyScope)
                .IsRequired();

            e.Property(x => x.StatusCode)
                .HasColumnName(DbSchema.Columns.StatusCode)
                .IsRequired();

            e.Property(x => x.ResponseJson)
                .HasColumnName(DbSchema.Columns.ResponseJson)
                .HasColumnType("jsonb")
                .IsRequired();

            e.Property(x => x.CreatedAt)
                .HasColumnName(DbSchema.Columns.CreatedAt)
                .IsRequired();

            e.Property(x => x.ExpiresAt)
                .HasColumnName(DbSchema.Columns.ExpiresAt)
                .IsRequired();

            e.HasIndex(x => new { x.UserId, x.Key, x.Scope }).IsUnique();
            e.HasIndex(x => x.ExpiresAt);
        }
    }
}
