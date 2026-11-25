using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Opportunities;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class InternshipApplicationConfiguration : IEntityTypeConfiguration<InternshipApplication>
{
    public void Configure(EntityTypeBuilder<InternshipApplication> builder)
    {
        builder.ToTable("InternshipApplications");
        builder.Property(a => a.CoverNote).HasMaxLength(2000).IsRequired();
        builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(30);
        builder.HasIndex(a => new { a.VacancyId, a.StudentId }).IsUnique();

        builder.HasOne(a => a.Vacancy).WithMany().HasForeignKey(a => a.VacancyId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(a => a.Student).WithMany().HasForeignKey(a => a.StudentId).OnDelete(DeleteBehavior.Cascade);
    }
}
