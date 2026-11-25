using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.People;
using NSBM.ICU.Domain.Entities.Simulation;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class SprintItemProgressConfiguration : IEntityTypeConfiguration<SprintItemProgress>
{
    public void Configure(EntityTypeBuilder<SprintItemProgress> builder)
    {
        builder.ToTable("SprintItemProgress");
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(x => new { x.StudentId, x.WorkItemId }).IsUnique();
        builder.HasOne<Student>().WithMany().HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.WorkItem).WithMany().HasForeignKey(x => x.WorkItemId).OnDelete(DeleteBehavior.Restrict);
    }
}
