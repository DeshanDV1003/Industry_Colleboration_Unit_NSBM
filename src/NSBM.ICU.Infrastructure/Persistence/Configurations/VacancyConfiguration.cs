using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Opportunities;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class VacancyConfiguration : IEntityTypeConfiguration<Vacancy>
{
    public void Configure(EntityTypeBuilder<Vacancy> builder)
    {
        builder.ToTable("Vacancies");
        builder.MapPartnerContent();
        builder.Property(v => v.Title).HasMaxLength(150).IsRequired();
        builder.Property(v => v.Description).HasMaxLength(4000).IsRequired();
        builder.Property(v => v.WorkMode).HasConversion<string>().HasMaxLength(20);
        builder.Property(v => v.Location).HasMaxLength(150).IsRequired();
        builder.HasIndex(v => new { v.Status, v.CareerPathId });

        builder.HasOne(v => v.CareerPath).WithMany().HasForeignKey(v => v.CareerPathId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(v => v.Skills).WithOne().HasForeignKey(s => s.VacancyId).OnDelete(DeleteBehavior.Cascade);
    }
}
