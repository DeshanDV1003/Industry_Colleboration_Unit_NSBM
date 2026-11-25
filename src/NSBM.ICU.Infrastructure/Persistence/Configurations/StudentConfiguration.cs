using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.People;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.Property(s => s.ProfileCardSlug).HasMaxLength(32);
        builder.HasIndex(s => s.ProfileCardSlug).IsUnique().HasFilter("[ProfileCardSlug] IS NOT NULL");
        builder.ToTable("Students");
        builder.MapPerson();
        builder.Property(s => s.StudentNo).HasMaxLength(30).IsRequired();
        builder.Property(s => s.Programme).HasMaxLength(150);
        builder.Property(s => s.Batch).HasMaxLength(50);
        builder.HasIndex(s => s.StudentNo).IsUnique();
        builder.HasOne(s => s.CareerPath).WithMany().HasForeignKey(s => s.CareerPathId).OnDelete(DeleteBehavior.SetNull);
    }
}
