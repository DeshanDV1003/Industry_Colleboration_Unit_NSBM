using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Learning;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class BaselineAttemptConfiguration : IEntityTypeConfiguration<BaselineAttempt>
{
    public void Configure(EntityTypeBuilder<BaselineAttempt> builder)
    {
        builder.ToTable("BaselineAttempts");
        builder.HasIndex(a => new { a.StudentId, a.CareerPathId, a.CompletedAt });
        builder.HasOne(a => a.Student).WithMany().HasForeignKey(a => a.StudentId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(a => a.CareerPath).WithMany().HasForeignKey(a => a.CareerPathId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(a => a.Answers).WithOne().HasForeignKey(x => x.AttemptId).OnDelete(DeleteBehavior.Cascade);
    }
}
