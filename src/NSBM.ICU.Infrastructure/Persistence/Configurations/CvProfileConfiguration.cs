using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.People;
using NSBM.ICU.Domain.Entities.Profiles;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class CvProfileConfiguration : IEntityTypeConfiguration<CvProfile>
{
    public void Configure(EntityTypeBuilder<CvProfile> builder)
    {
        builder.ToTable("CvProfiles");
        builder.Property(x => x.Headline).HasMaxLength(150);
        builder.Property(x => x.Summary).HasMaxLength(2000);
        builder.Property(x => x.Education).HasMaxLength(2000);
        builder.Property(x => x.Projects).HasMaxLength(4000);
        builder.Property(x => x.GitHubUrl).HasMaxLength(300);
        builder.Property(x => x.LinkedInUrl).HasMaxLength(300);
        builder.HasIndex(x => x.StudentId).IsUnique();
        builder.HasOne<Student>().WithOne().HasForeignKey<CvProfile>(x => x.StudentId).OnDelete(DeleteBehavior.Cascade);
    }
}
