using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Interviews;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class AiInterviewScoreConfiguration : IEntityTypeConfiguration<AiInterviewScore>
{
    public void Configure(EntityTypeBuilder<AiInterviewScore> builder)
    {
        builder.ToTable("AiInterviewScores");
        builder.Property(x => x.Criterion).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Comment).HasMaxLength(1000).IsRequired();
        builder.HasIndex(x => new { x.SessionId, x.Criterion }).IsUnique();
    }
}
