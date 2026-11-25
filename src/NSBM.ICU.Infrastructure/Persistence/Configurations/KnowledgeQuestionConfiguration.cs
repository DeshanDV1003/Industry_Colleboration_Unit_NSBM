using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Tasks;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class KnowledgeQuestionConfiguration : IEntityTypeConfiguration<KnowledgeQuestion>
{
    public void Configure(EntityTypeBuilder<KnowledgeQuestion> builder)
    {
        builder.ToTable("KnowledgeQuestions");
        builder.Property(q => q.Prompt).HasMaxLength(500).IsRequired();
    }
}
