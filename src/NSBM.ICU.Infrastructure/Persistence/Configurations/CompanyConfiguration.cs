using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Organisations;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies");
        builder.Property(c => c.Name).HasMaxLength(150).IsRequired();
        builder.Property(c => c.Sector).HasMaxLength(100);
        builder.Property(c => c.Location).HasMaxLength(150);
        builder.Property(c => c.Website).HasMaxLength(300);
        builder.Property(c => c.Description).HasMaxLength(2000);
        builder.HasIndex(c => c.Name).IsUnique();
    }
}
