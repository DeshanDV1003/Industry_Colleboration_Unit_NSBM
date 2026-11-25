using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Learning;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class TaskCheckResultConfiguration : IEntityTypeConfiguration<TaskCheckResult>
{
    public void Configure(EntityTypeBuilder<TaskCheckResult> builder)
    {
        builder.ToTable("TaskCheckResults");
        builder.HasIndex(r => new { r.AttemptId, r.CheckId }).IsUnique();
        builder.HasOne(r => r.Check).WithMany().HasForeignKey(r => r.CheckId).OnDelete(DeleteBehavior.Restrict);
    }
}
