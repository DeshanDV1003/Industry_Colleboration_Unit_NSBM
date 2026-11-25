using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Careers;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class CareerPathSkillConfiguration : IEntityTypeConfiguration<CareerPathSkill>
{
    public void Configure(EntityTypeBuilder<CareerPathSkill> builder)
    {
        builder.ToTable("CareerPathSkills");
        builder.Property(s => s.Importance).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(s => new { s.CareerPathId, s.SkillId }).IsUnique();
        builder.HasOne(s => s.Skill).WithMany().HasForeignKey(s => s.SkillId).OnDelete(DeleteBehavior.Restrict);
    }
}
