using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Organisations;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class PartnerEnquiryConfiguration : IEntityTypeConfiguration<PartnerEnquiry>
{
    public void Configure(EntityTypeBuilder<PartnerEnquiry> builder)
    {
        builder.ToTable("PartnerEnquiries");
        builder.Property(e => e.OrganisationName).HasMaxLength(150).IsRequired();
        builder.Property(e => e.ContactName).HasMaxLength(120).IsRequired();
        builder.Property(e => e.Email).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Phone).HasMaxLength(30);
        builder.Property(e => e.Interest).HasConversion<string>().HasMaxLength(20);
        builder.Property(e => e.Message).HasMaxLength(2000).IsRequired();
        builder.Property(e => e.HandledBy).HasMaxLength(200);
        builder.HasIndex(e => new { e.IsHandled, e.SubmittedAt });
    }
}
