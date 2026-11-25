using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Infrastructure.Persistence.Seed.Data;

/// <summary>Work items for the virtual IT company's practice sprint. Pure data.</summary>
internal static class SimulationData
{
    public sealed record WorkItemSeed(string Key, string CareerPathSlug, string Title, string Description, int Points, string Teammate, SprintItemStatus TeammateStatus);

    public static readonly WorkItemSeed[] WorkItems =
    [
        new("SE-101", "software-engineering", "Add order status endpoint", "Expose GET /orders/{id}/status returning the current delivery stage.", 3, "Ishan (SE)", SprintItemStatus.InProgress),
        new("SE-102", "software-engineering", "Fix rounding in invoice totals", "Totals with discounts are off by one cent. Add a unit test that reproduces it.", 2, "Ishan (SE)", SprintItemStatus.ToDo),
        new("SE-103", "software-engineering", "Show tracking timeline in the web app", "Render the status history from the new endpoint as a timeline.", 5, "Malsha (SE)", SprintItemStatus.ToDo),
        new("QA-201", "quality-assurance", "Write test cases for order tracking", "Cover each status transition, invalid IDs and permission checks.", 3, "Dineth (QA)", SprintItemStatus.InProgress),
        new("QA-202", "quality-assurance", "Regression run on checkout", "Run the checkout regression suite and log any defects.", 2, "Dineth (QA)", SprintItemStatus.ToDo),
        new("QA-203", "quality-assurance", "Automate status API tests", "Add API tests for the status endpoint to the CI suite.", 3, "Dineth (QA)", SprintItemStatus.ToDo),
        new("DO-301", "devops", "Add status service to the pipeline", "Build, test and deploy the new service on every merge to main.", 3, "Sachini (DevOps)", SprintItemStatus.InReview),
        new("DO-302", "devops", "Dashboard for order-service latency", "Create a dashboard and an alert for p95 latency above 500 ms.", 2, "Sachini (DevOps)", SprintItemStatus.ToDo),
        new("CL-401", "cloud-engineering", "Private subnet for the orders database", "Move the database to a private subnet and update security groups.", 3, "Ruwan (Cloud)", SprintItemStatus.Done),
        new("CL-402", "cloud-engineering", "Least-privilege role for the status service", "Create a role that can only read the orders table and write logs.", 2, "Ruwan (Cloud)", SprintItemStatus.ToDo),
    ];
}
