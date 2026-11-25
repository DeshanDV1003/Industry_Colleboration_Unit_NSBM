using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Interviews;
using NSBM.ICU.Domain.Entities.People;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class AiInterviewSessionConfiguration : IEntityTypeConfiguration<AiInterviewSession>
{
    public void Configure(EntityTypeBuilder<AiInterviewSession> builder)
    {
        builder.ToTable("AiInterviewSessions");
        builder.Property(s => s.Stage).HasConversion<string>().HasMaxLength(20);
        builder.Property(s => s.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(s => s.TargetRole).HasMaxLength(150).IsRequired();
        builder.Property(s => s.FocusSkills).HasMaxLength(500).IsRequired();
        builder.Property(s => s.FeedbackSummary).HasMaxLength(2000);
        builder.Property(s => s.Strengths).HasMaxLength(2000);
        builder.Property(s => s.Improvements).HasMaxLength(2000);
        builder.HasIndex(s => new { s.StudentId, s.StartedAt });

        builder.HasOne<Student>().WithMany().HasForeignKey(s => s.StudentId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(s => s.Turns).WithOne().HasForeignKey(t => t.SessionId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(s => s.Scores).WithOne().HasForeignKey(x => x.SessionId).OnDelete(DeleteBehavior.Cascade);
    }
}
