using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Events;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class EventTeamConfiguration : IEntityTypeConfiguration<EventTeam>
{
    public void Configure(EntityTypeBuilder<EventTeam> builder)
    {
        builder.ToTable("EventTeams");
        builder.Property(t => t.Name).HasMaxLength(60).IsRequired();
        builder.Property(t => t.JoinCode).HasMaxLength(10).IsRequired();
        builder.HasIndex(t => new { t.EventId, t.JoinCode }).IsUnique();
    }
}
