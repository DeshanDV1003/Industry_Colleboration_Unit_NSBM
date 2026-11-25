using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Interviews;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class AiInterviewTurnConfiguration : IEntityTypeConfiguration<AiInterviewTurn>
{
    public void Configure(EntityTypeBuilder<AiInterviewTurn> builder)
    {
        builder.ToTable("AiInterviewTurns");
        builder.Property(t => t.Speaker).HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.Text).HasMaxLength(AiInterviewRubric.MaxAnswerLength).IsRequired();
        builder.HasIndex(t => new { t.SessionId, t.Order }).IsUnique();
    }
}
