using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Tasks;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class IndustryTaskConfiguration : IEntityTypeConfiguration<IndustryTask>
{
    public void Configure(EntityTypeBuilder<IndustryTask> builder)
    {
        builder.ToTable("IndustryTasks");
        builder.MapPartnerContent();
        builder.Property(t => t.Title).HasMaxLength(150).IsRequired();
        builder.Property(t => t.Summary).HasMaxLength(500).IsRequired();
        builder.Property(t => t.Instructions).HasMaxLength(8000).IsRequired();
        builder.Property(t => t.Level).HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.Situation).HasConversion<string>().HasMaxLength(30);
        builder.Property(t => t.StarterCode).HasMaxLength(20000);
        builder.Property(t => t.StarterLanguage).HasMaxLength(30);
        builder.HasIndex(t => new { t.Status, t.CareerPathId });

        builder.HasOne(t => t.CareerPath).WithMany().HasForeignKey(t => t.CareerPathId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(t => t.Skills).WithOne().HasForeignKey(s => s.TaskId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(t => t.Checks).WithOne().HasForeignKey(c => c.TaskId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(t => t.KnowledgeQuestions).WithOne().HasForeignKey(q => q.TaskId).OnDelete(DeleteBehavior.Cascade);
    }
}
