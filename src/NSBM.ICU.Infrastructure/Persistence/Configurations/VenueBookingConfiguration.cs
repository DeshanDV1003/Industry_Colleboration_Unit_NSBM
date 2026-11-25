using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Venues;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class VenueBookingConfiguration : IEntityTypeConfiguration<VenueBooking>
{
    public void Configure(EntityTypeBuilder<VenueBooking> builder)
    {
        builder.ToTable("VenueBookings");
        builder.Property(x => x.Title).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Purpose).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.Programme).HasMaxLength(150);
        builder.Property(x => x.Note).HasMaxLength(500);
        builder.Property(x => x.BookedByName).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => new { x.VenueId, x.Date });
        builder.HasIndex(x => new { x.Programme, x.AcademicYear, x.Date });

        builder.HasOne(x => x.Venue).WithMany().HasForeignKey(x => x.VenueId).OnDelete(DeleteBehavior.Restrict);
        // Deleting an event (demo reset only) removes its hall bookings with it.
        builder.HasOne(x => x.IndustryEvent).WithMany().HasForeignKey(x => x.IndustryEventId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.CalendarEvent).WithMany().HasForeignKey(x => x.CalendarEventId).OnDelete(DeleteBehavior.Cascade);
    }
}
