using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Learning;
using NSBM.ICU.Domain.Entities.People;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class TaskAttemptReviewConfiguration : IEntityTypeConfiguration<TaskAttemptReview>
{
    public void Configure(EntityTypeBuilder<TaskAttemptReview> builder)
    {
        builder.ToTable("TaskAttemptReviews");
        builder.Property(x => x.ReviewerName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Comment).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.Improvements).HasMaxLength(2000);
        builder.HasIndex(x => x.AttemptId);
        builder.HasOne(x => x.Attempt).WithMany(a => a.Reviews).HasForeignKey(x => x.AttemptId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<IndustryPartner>().WithMany().HasForeignKey(x => x.ReviewerPartnerId).OnDelete(DeleteBehavior.Restrict);
    }
}
