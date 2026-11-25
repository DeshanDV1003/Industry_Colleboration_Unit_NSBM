using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.People;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class AdministratorConfiguration : IEntityTypeConfiguration<Administrator>
{
    public void Configure(EntityTypeBuilder<Administrator> builder)
    {
        builder.ToTable("Administrators");
        builder.MapPerson();
        builder.Property(a => a.Designation).HasMaxLength(150);
    }
}
