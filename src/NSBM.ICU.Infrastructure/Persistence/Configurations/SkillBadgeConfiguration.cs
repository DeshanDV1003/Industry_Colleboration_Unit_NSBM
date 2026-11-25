using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.People;
using NSBM.ICU.Domain.Entities.Profiles;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class SkillBadgeConfiguration : IEntityTypeConfiguration<SkillBadge>
{
    public void Configure(EntityTypeBuilder<SkillBadge> builder)
    {
        builder.ToTable("SkillBadges");
        builder.Property(b => b.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(b => b.EvidenceSummary).HasMaxLength(2000).IsRequired();
        builder.Property(b => b.StudentNote).HasMaxLength(500);
        builder.Property(b => b.ReviewerName).HasMaxLength(200);
        builder.Property(b => b.ReviewNote).HasMaxLength(1000);
        builder.Property(b => b.VerificationCode).HasMaxLength(20);

        builder.HasIndex(b => new { b.StudentId, b.SkillId }).IsUnique();
        builder.HasIndex(b => b.VerificationCode).IsUnique().HasFilter("[VerificationCode] IS NOT NULL");
        builder.HasIndex(b => new { b.Status, b.RequestedAt });

        builder.HasOne<Student>().WithMany().HasForeignKey(b => b.StudentId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(b => b.Skill).WithMany().HasForeignKey(b => b.SkillId).OnDelete(DeleteBehavior.Restrict);
    }
}
