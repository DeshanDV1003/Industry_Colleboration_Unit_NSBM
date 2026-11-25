using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Learning;
using NSBM.ICU.Domain.Entities.People;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class InterviewAnswerConfiguration : IEntityTypeConfiguration<InterviewAnswer>
{
    public void Configure(EntityTypeBuilder<InterviewAnswer> builder)
    {
        builder.ToTable("InterviewAnswers");
        builder.Property(a => a.Text).HasMaxLength(4000).IsRequired();
        builder.Property(a => a.InputMode).HasConversion<string>().HasMaxLength(10);
        builder.Property(a => a.MatchedKeywords).HasMaxLength(500);
        builder.HasIndex(a => new { a.StudentId, a.QuestionId, a.AnsweredAt });

        builder.HasOne<Student>().WithMany().HasForeignKey(a => a.StudentId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(a => a.Question).WithMany().HasForeignKey(a => a.QuestionId).OnDelete(DeleteBehavior.Restrict);
    }
}
