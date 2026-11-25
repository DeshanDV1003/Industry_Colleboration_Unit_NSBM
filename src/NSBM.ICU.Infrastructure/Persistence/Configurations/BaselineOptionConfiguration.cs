using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Careers;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class BaselineOptionConfiguration : IEntityTypeConfiguration<BaselineOption>
{
    public void Configure(EntityTypeBuilder<BaselineOption> builder)
    {
        builder.ToTable("BaselineOptions");
        builder.Property(o => o.Text).HasMaxLength(300).IsRequired();
    }
}
