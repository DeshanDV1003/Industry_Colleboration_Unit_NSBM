using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Simulation;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class SprintWorkItemConfiguration : IEntityTypeConfiguration<SprintWorkItem>
{
    public void Configure(EntityTypeBuilder<SprintWorkItem> builder)
    {
        builder.ToTable("SprintWorkItems");
        builder.Property(x => x.Key).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.TeammateName).HasMaxLength(100);
        builder.Property(x => x.TeammateStatus).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(x => x.Key).IsUnique();
        builder.HasOne(x => x.CareerPath).WithMany().HasForeignKey(x => x.CareerPathId).OnDelete(DeleteBehavior.Restrict);
    }
}
