using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Careers;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class BaselineQuestionConfiguration : IEntityTypeConfiguration<BaselineQuestion>
{
    public void Configure(EntityTypeBuilder<BaselineQuestion> builder)
    {
        builder.ToTable("BaselineQuestions");
        builder.Property(q => q.Prompt).HasMaxLength(500).IsRequired();
        builder.Property(q => q.Explanation).HasMaxLength(1000);
        builder.HasOne(q => q.Skill).WithMany().HasForeignKey(q => q.SkillId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(q => q.Options).WithOne().HasForeignKey(o => o.QuestionId).OnDelete(DeleteBehavior.Cascade);
    }
}
