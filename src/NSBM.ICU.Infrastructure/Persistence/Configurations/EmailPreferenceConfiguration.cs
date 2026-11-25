using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Notifications;
using NSBM.ICU.Infrastructure.Identity;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class EmailPreferenceConfiguration : IEntityTypeConfiguration<EmailPreference>
{
    public void Configure(EntityTypeBuilder<EmailPreference> builder)
    {
        builder.ToTable("EmailPreferences");
        builder.HasKey(x => x.UserId);
        builder.Property(x => x.UserId).HasMaxLength(450);
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
