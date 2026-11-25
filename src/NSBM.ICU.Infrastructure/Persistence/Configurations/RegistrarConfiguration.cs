using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.People;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class RegistrarConfiguration : IEntityTypeConfiguration<Registrar>
{
    public void Configure(EntityTypeBuilder<Registrar> builder)
    {
        builder.ToTable("Registrars");
        builder.MapPerson();
        builder.Property(r => r.Designation).HasMaxLength(150);
        builder.Property(r => r.Office).HasMaxLength(150);
    }
}
