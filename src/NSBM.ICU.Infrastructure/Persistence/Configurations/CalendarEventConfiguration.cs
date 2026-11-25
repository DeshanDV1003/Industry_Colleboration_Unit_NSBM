using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Calendar;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class CalendarEventConfiguration : IEntityTypeConfiguration<CalendarEvent>
{
    public void Configure(EntityTypeBuilder<CalendarEvent> builder)
    {
        builder.ToTable("CalendarEvents");
        builder.Property(x => x.Title).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.Location).HasMaxLength(150);
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.Audience).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(x => new { x.StartDate, x.EndDate });

        // Restrict avoids multiple cascade paths; tasks and vacancies are never hard-deleted outside the demo reset.
        builder.HasOne(x => x.CareerPath).WithMany().HasForeignKey(x => x.CareerPathId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.IndustryTask).WithMany().HasForeignKey(x => x.IndustryTaskId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Vacancy).WithMany().HasForeignKey(x => x.VacancyId).OnDelete(DeleteBehavior.Restrict);
    }
}
