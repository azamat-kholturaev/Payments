using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payments.Infrastructure.Common.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Payments.Domain.Entities;

namespace Payments.Infrastructure.Common.Persistence.Configurations
{
    internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> e)
        {
            e.ToTable(DbSchema.Tables.Users);
            e.HasKey(x => x.Id);

            e.Property(x => x.Email)
                .HasConversion(ValueConverters.EmailConverter)
                .HasColumnName(DbSchema.Columns.Email)
                .HasMaxLength(DbSchema.Lengths.Email)
                .IsRequired();

            e.HasIndex(x => x.Email).IsUnique();

            e.Property(x => x.PasswordHash)
                .HasConversion(ValueConverters.PasswordHashConverter)
                .HasColumnName(DbSchema.Columns.PasswordHash)
                .HasMaxLength(DbSchema.Lengths.PasswordHash)
                .IsRequired();

            e.Property(x => x.CreatedAt)
                .HasColumnName(DbSchema.Columns.CreatedAt)
                .IsRequired();
        }
    }
}
