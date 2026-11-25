using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Careers;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class CareerPathConfiguration : IEntityTypeConfiguration<CareerPath>
{
    public void Configure(EntityTypeBuilder<CareerPath> builder)
    {
        builder.ToTable("CareerPaths");
        builder.Property(p => p.Slug).HasMaxLength(50).IsRequired();
        builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Tagline).HasMaxLength(200);
        builder.Property(p => p.Description).HasMaxLength(2000);
        builder.Property(p => p.EntryRoles).HasMaxLength(500);
        builder.HasIndex(p => p.Slug).IsUnique();

        builder.HasMany(p => p.Skills).WithOne().HasForeignKey(s => s.CareerPathId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(p => p.RoadmapSteps).WithOne().HasForeignKey(s => s.CareerPathId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(p => p.BaselineQuestions).WithOne().HasForeignKey(q => q.CareerPathId).OnDelete(DeleteBehavior.Cascade);
    }
}
