using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.People;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class IndustryPartnerConfiguration : IEntityTypeConfiguration<IndustryPartner>
{
    public void Configure(EntityTypeBuilder<IndustryPartner> builder)
    {
        builder.ToTable("IndustryPartners");
        builder.MapPerson();
        builder.Property(p => p.JobTitle).HasMaxLength(100);
        builder.HasOne(p => p.Company)
            .WithMany(c => c.Partners)
            .HasForeignKey(p => p.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
