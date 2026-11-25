using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.People;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class LecturerConfiguration : IEntityTypeConfiguration<Lecturer>
{
    public void Configure(EntityTypeBuilder<Lecturer> builder)
    {
        builder.ToTable("Lecturers");
        builder.MapPerson();
        builder.Property(l => l.StaffNo).HasMaxLength(30).IsRequired();
        builder.Property(l => l.Designation).HasMaxLength(100);
        builder.Property(l => l.Department).HasMaxLength(150);
        builder.HasIndex(l => l.StaffNo).IsUnique();
    }
}
