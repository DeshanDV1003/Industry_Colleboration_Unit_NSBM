using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Venues;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        builder.ToTable("Venues");
        builder.Property(x => x.Code).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Building).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Facilities).HasMaxLength(300);
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(20);
        builder.Ignore(x => x.DisplayName);
        builder.HasIndex(x => x.Code).IsUnique();
    }
}
