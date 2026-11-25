using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSBM.ICU.Domain.Entities.Careers;
using NSBM.ICU.Domain.Entities.Learning;

namespace NSBM.ICU.Infrastructure.Persistence.Configurations;

public sealed class BaselineAnswerConfiguration : IEntityTypeConfiguration<BaselineAnswer>
{
    public void Configure(EntityTypeBuilder<BaselineAnswer> builder)
    {
        builder.ToTable("BaselineAnswers");
        builder.HasIndex(a => new { a.AttemptId, a.QuestionId }).IsUnique();
        builder.HasOne(a => a.Question).WithMany().HasForeignKey(a => a.QuestionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<BaselineOption>().WithMany().HasForeignKey(a => a.SelectedOptionId).OnDelete(DeleteBehavior.Restrict);
    }
}
