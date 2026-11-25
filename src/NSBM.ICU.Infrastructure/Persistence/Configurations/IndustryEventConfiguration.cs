using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Events;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class IndustryEventConfiguration : IEntityTypeConfiguration<IndustryEvent>
{
    public void Configure(EntityTypeBuilder<IndustryEvent> builder)
    {
        builder.ToTable("IndustryEvents");
        builder.MapPartnerContent();
        builder.Property(e => e.Title).HasMaxLength(150).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(4000).IsRequired();
        builder.Property(e => e.Format).HasConversion<string>().HasMaxLength(20);
        builder.Property(e => e.Location).HasMaxLength(150).IsRequired();
        builder.HasIndex(e => new { e.Status, e.StartDate });

        // Registrations and teams belong to the event; removing one from the aggregate deletes its row.
        builder.HasMany(e => e.Registrations).WithOne().HasForeignKey(r => r.EventId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(e => e.Teams).WithOne().HasForeignKey(t => t.EventId).OnDelete(DeleteBehavior.Cascade);
    }
}
