using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Tasks;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class TaskSkillConfiguration : IEntityTypeConfiguration<TaskSkill>
{
    public void Configure(EntityTypeBuilder<TaskSkill> builder)
    {
        builder.ToTable("TaskSkills");
        builder.HasIndex(s => new { s.TaskId, s.SkillId }).IsUnique();
        builder.HasOne(s => s.Skill).WithMany().HasForeignKey(s => s.SkillId).OnDelete(DeleteBehavior.Restrict);
    }
}
