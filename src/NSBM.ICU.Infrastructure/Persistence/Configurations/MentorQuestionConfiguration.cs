using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Mentoring;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class MentorQuestionConfiguration : IEntityTypeConfiguration<MentorQuestion>
{
    public void Configure(EntityTypeBuilder<MentorQuestion> builder)
    {
        builder.ToTable("MentorQuestions");
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Body).HasMaxLength(4000).IsRequired();
        builder.Property(x => x.Reply).HasMaxLength(4000);
        builder.Property(x => x.RepliedBy).HasMaxLength(200);
        builder.Property(x => x.RepliedByCompany).HasMaxLength(150);
        builder.Ignore(x => x.IsAnswered);
        builder.HasIndex(x => new { x.StudentId, x.AskedAt });
        builder.HasOne(x => x.Student).WithMany().HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.CareerPath).WithMany().HasForeignKey(x => x.CareerPathId).OnDelete(DeleteBehavior.SetNull);
    }
}
