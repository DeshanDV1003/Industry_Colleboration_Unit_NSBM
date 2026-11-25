using NSBM.ICU.Application.Common.Interfaces;

namespace NSBM.ICU.Infrastructure.Services;

public sealed class DateTimeService : IDateTime
{
    public DateTime UtcNow => DateTime.UtcNow;
}
