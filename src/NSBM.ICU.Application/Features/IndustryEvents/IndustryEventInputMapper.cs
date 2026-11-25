using NSBM.ICU.Application.Features.IndustryEvents.Models;
using NSBM.ICU.Domain.Entities.Events;

namespace NSBM.ICU.Application.Features.IndustryEvents;

internal static class IndustryEventInputMapper
{
    public static void ApplyTo(this IndustryEventInput input, IndustryEvent target)
    {
        target.Title = input.Title.Trim();
        target.Description = input.Description.Trim();
        target.Format = input.Format;
        target.StartDate = input.StartDate;
        target.EndDate = input.EndDate;
        target.Location = input.Location.Trim();
        target.Capacity = input.Capacity;
        target.RegistrationClosesOn = input.RegistrationClosesOn;
        target.MinTeamSize = input.MinTeamSize;
        target.MaxTeamSize = input.MaxTeamSize;
    }
}
