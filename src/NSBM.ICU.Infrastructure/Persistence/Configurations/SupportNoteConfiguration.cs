using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.People;
using NSBM.ICU.Domain.Entities.Support;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class SupportNoteConfiguration : IEntityTypeConfiguration<SupportNote>
{
    public void Configure(EntityTypeBuilder<SupportNote> builder)
    {
        builder.ToTable("SupportNotes");
        builder.Property(x => x.LecturerName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Text).HasMaxLength(2000).IsRequired();
        builder.HasIndex(x => new { x.StudentId, x.CreatedAt });
        builder.HasOne<Student>().WithMany().HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Lecturer>().WithMany().HasForeignKey(x => x.LecturerId).OnDelete(DeleteBehavior.Restrict);
    }
}
