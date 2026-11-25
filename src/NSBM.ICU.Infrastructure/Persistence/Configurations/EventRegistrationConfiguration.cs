using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Events;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class EventRegistrationConfiguration : IEntityTypeConfiguration<EventRegistration>
{
    public void Configure(EntityTypeBuilder<EventRegistration> builder)
    {
        builder.ToTable("EventRegistrations");
        builder.Property(r => r.CertificateCode).HasMaxLength(20);
        builder.HasIndex(r => new { r.EventId, r.StudentId }).IsUnique();
        builder.HasIndex(r => r.CertificateCode).IsUnique().HasFilter("[CertificateCode] IS NOT NULL");
        builder.Ignore(r => r.HasCertificate);

        builder.HasOne(r => r.Student).WithMany().HasForeignKey(r => r.StudentId).OnDelete(DeleteBehavior.Cascade);
        // NoAction avoids a second cascade path; the aggregate clears TeamId before a team is removed.
        builder.HasOne(r => r.Team).WithMany().HasForeignKey(r => r.TeamId).OnDelete(DeleteBehavior.NoAction);
    }
}
