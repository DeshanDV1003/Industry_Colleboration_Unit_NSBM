using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Learning;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class TaskAttemptConfiguration : IEntityTypeConfiguration<TaskAttempt>
{
    public void Configure(EntityTypeBuilder<TaskAttempt> builder)
    {
        builder.ToTable("TaskAttempts");
        builder.Property(a => a.Code).HasMaxLength(20000).IsRequired();
        builder.HasIndex(a => new { a.StudentId, a.TaskId, a.AttemptNumber }).IsUnique();

        builder.HasOne(a => a.Student).WithMany().HasForeignKey(a => a.StudentId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(a => a.Task).WithMany().HasForeignKey(a => a.TaskId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(a => a.CheckResults).WithOne().HasForeignKey(r => r.AttemptId).OnDelete(DeleteBehavior.Cascade);
    }
}
