using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.People;
using NSBM.ICU.Domain.Entities.Planning;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class PlannerItemConfiguration : IEntityTypeConfiguration<PlannerItem>
{
    public void Configure(EntityTypeBuilder<PlannerItem> builder)
    {
        builder.ToTable("PlannerItems");
        builder.Property(x => x.Kind).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasIndex(x => new { x.StudentId, x.IsDone, x.DueDate });
        builder.HasOne<Student>().WithMany().HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Cascade);
    }
}
