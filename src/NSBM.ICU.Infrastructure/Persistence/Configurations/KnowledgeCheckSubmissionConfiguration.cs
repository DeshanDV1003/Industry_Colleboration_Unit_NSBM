using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Learning;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class KnowledgeCheckSubmissionConfiguration : IEntityTypeConfiguration<KnowledgeCheckSubmission>
{
    public void Configure(EntityTypeBuilder<KnowledgeCheckSubmission> builder)
    {
        builder.ToTable("KnowledgeCheckSubmissions");
        builder.HasIndex(s => new { s.StudentId, s.TaskId }).IsUnique();
        builder.HasOne(s => s.Student).WithMany().HasForeignKey(s => s.StudentId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(s => s.Task).WithMany().HasForeignKey(s => s.TaskId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(s => s.Answers).WithOne().HasForeignKey(a => a.SubmissionId).OnDelete(DeleteBehavior.Cascade);
    }
}
