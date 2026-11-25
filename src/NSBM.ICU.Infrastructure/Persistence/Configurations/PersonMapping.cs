using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.People;
using NSBM.ICU.Infrastructure.Identity;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

/// <summary>Column rules shared by every role profile table.</summary>
internal static class PersonMapping
{
    public static EntityTypeBuilder<TPerson> MapPerson<TPerson>(this EntityTypeBuilder<TPerson> builder)
        where TPerson : Person
    {
        builder.Property(p => p.UserId).HasMaxLength(450).IsRequired();
        builder.Property(p => p.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(p => p.LastName).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Email).HasMaxLength(256).IsRequired();
        builder.Ignore(p => p.FullName);

        builder.HasIndex(p => p.UserId).IsUnique();
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Cascade);

        return builder;
    }
}
