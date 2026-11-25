using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Learning;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class KnowledgeAnswerConfiguration : IEntityTypeConfiguration<KnowledgeAnswer>
{
    public void Configure(EntityTypeBuilder<KnowledgeAnswer> builder)
    {
        builder.ToTable("KnowledgeAnswers");
        builder.Property(a => a.Text).HasMaxLength(4000).IsRequired();
        builder.HasIndex(a => new { a.SubmissionId, a.QuestionId }).IsUnique();
        builder.HasOne(a => a.Question).WithMany().HasForeignKey(a => a.QuestionId).OnDelete(DeleteBehavior.Restrict);
    }
}
