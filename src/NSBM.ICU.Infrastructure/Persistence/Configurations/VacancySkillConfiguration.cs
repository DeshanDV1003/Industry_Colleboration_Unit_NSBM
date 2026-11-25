using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Opportunities;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class VacancySkillConfiguration : IEntityTypeConfiguration<VacancySkill>
{
    public void Configure(EntityTypeBuilder<VacancySkill> builder)
    {
        builder.ToTable("VacancySkills");
        builder.HasIndex(s => new { s.VacancyId, s.SkillId }).IsUnique();
        builder.HasOne(s => s.Skill).WithMany().HasForeignKey(s => s.SkillId).OnDelete(DeleteBehavior.Restrict);
    }
}
