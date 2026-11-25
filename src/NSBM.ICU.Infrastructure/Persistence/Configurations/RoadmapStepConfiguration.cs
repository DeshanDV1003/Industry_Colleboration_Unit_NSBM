using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Careers;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class RoadmapStepConfiguration : IEntityTypeConfiguration<RoadmapStep>
{
    public void Configure(EntityTypeBuilder<RoadmapStep> builder)
    {
        builder.ToTable("RoadmapSteps");
        builder.Property(s => s.Stage).HasConversion<string>().HasMaxLength(20);
        builder.Property(s => s.Title).HasMaxLength(150).IsRequired();
        builder.Property(s => s.Description).HasMaxLength(1000);
        builder.Property(s => s.Outcome).HasMaxLength(500);
        builder.HasIndex(s => new { s.CareerPathId, s.SortOrder }).IsUnique();
        builder.HasOne(s => s.Skill).WithMany().HasForeignKey(s => s.SkillId).OnDelete(DeleteBehavior.Restrict);
    }
}
