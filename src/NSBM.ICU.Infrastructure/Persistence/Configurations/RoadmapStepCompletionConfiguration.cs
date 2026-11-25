using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Learning;
using NSBM.ICU.Domain.Entities.People;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class RoadmapStepCompletionConfiguration : IEntityTypeConfiguration<RoadmapStepCompletion>
{
    public void Configure(EntityTypeBuilder<RoadmapStepCompletion> builder)
    {
        builder.ToTable("RoadmapStepCompletions");
        builder.HasIndex(c => new { c.StudentId, c.RoadmapStepId }).IsUnique();
        builder.HasOne<Student>().WithMany().HasForeignKey(c => c.StudentId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(c => c.RoadmapStep).WithMany().HasForeignKey(c => c.RoadmapStepId).OnDelete(DeleteBehavior.Restrict);
    }
}
