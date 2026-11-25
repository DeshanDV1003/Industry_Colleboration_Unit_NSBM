using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Auditing;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

/// <summary>No foreign key to the actor's account: entries must outlive accounts that are removed.</summary>
public sealed class AuditEntryConfiguration : IEntityTypeConfiguration<AuditEntry>
{
    public void Configure(EntityTypeBuilder<AuditEntry> builder)
    {
        builder.ToTable("AuditEntries");
        builder.Property(x => x.ActorUserId).HasMaxLength(450);
        builder.Property(x => x.ActorName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.ActorRole).HasMaxLength(20);
        builder.Property(x => x.Category).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.Action).HasMaxLength(60).IsRequired();
        builder.Property(x => x.Summary).HasMaxLength(500).IsRequired();
        builder.Property(x => x.SubjectType).HasMaxLength(40);
        builder.Property(x => x.SubjectId).HasMaxLength(256);
        builder.HasIndex(x => x.At);
        builder.HasIndex(x => new { x.Category, x.At });
    }
}
