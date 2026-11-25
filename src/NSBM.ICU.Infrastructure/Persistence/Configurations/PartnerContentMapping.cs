using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Organisations;
using NSBM.ICU.Domain.Entities.People;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

/// <summary>Columns and relationships shared by every partner-authored, moderated table.</summary>
internal static class PartnerContentMapping
{
    public static EntityTypeBuilder<TContent> MapPartnerContent<TContent>(this EntityTypeBuilder<TContent> builder)
        where TContent : PartnerContent
    {
        builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.ReviewedBy).HasMaxLength(200);
        builder.Property(c => c.ReviewNote).HasMaxLength(1000);

        builder.HasOne(c => c.Company).WithMany().HasForeignKey(c => c.CompanyId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<IndustryPartner>().WithMany().HasForeignKey(c => c.AuthorPartnerId).OnDelete(DeleteBehavior.Restrict);

        return builder;
    }
}
