using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Tasks;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class TaskCheckConfiguration : IEntityTypeConfiguration<TaskCheck>
{
    public void Configure(EntityTypeBuilder<TaskCheck> builder)
    {
        builder.ToTable("TaskChecks");
        builder.Property(c => c.Description).HasMaxLength(300).IsRequired();
        builder.Property(c => c.Pattern).HasMaxLength(500).IsRequired();
        builder.Property(c => c.Hint).HasMaxLength(500);
    }
}
