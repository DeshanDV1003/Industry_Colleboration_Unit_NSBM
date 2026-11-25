using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Interviews;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class InterviewQuestionConfiguration : IEntityTypeConfiguration<InterviewQuestion>
{
    public void Configure(EntityTypeBuilder<InterviewQuestion> builder)
    {
        builder.ToTable("InterviewQuestions");
        builder.Property(q => q.Stage).HasConversion<string>().HasMaxLength(20);
        builder.Property(q => q.Topic).HasMaxLength(100).IsRequired();
        builder.Property(q => q.Prompt).HasMaxLength(500).IsRequired();
        builder.Property(q => q.Keywords).HasMaxLength(500);
        builder.Property(q => q.Tip).HasMaxLength(500);
        builder.Ignore(q => q.KeywordList);
        builder.HasIndex(q => new { q.Stage, q.CareerPathId });

        builder.HasOne(q => q.CareerPath).WithMany().HasForeignKey(q => q.CareerPathId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(q => q.FollowUps).WithOne().HasForeignKey(q => q.ParentQuestionId).OnDelete(DeleteBehavior.Restrict);
    }
}
