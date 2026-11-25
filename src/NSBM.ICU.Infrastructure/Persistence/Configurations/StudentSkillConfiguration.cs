using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.People;
using NSBM.ICU.Domain.Entities.Profiles;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class StudentSkillConfiguration : IEntityTypeConfiguration<StudentSkill>
{
    public void Configure(EntityTypeBuilder<StudentSkill> builder)
    {
        builder.ToTable("StudentSkills");
        builder.HasIndex(s => new { s.StudentId, s.SkillId }).IsUnique();
        builder.HasOne<Student>().WithMany().HasForeignKey(s => s.StudentId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(s => s.Skill).WithMany().HasForeignKey(s => s.SkillId).OnDelete(DeleteBehavior.Restrict);
    }
}
