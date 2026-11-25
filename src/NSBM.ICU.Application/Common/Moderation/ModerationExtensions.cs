using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Models;
using NSBM.ICU.Domain.Entities.Organisations;

namespace NSBM.ICU.Application.Common.Moderation;

public static class ModerationExtensions
{
    /// <summary>Applies an administrator's decision to pending partner content.</summary>
    public static void ApplyReview(this PartnerContent content, IReviewCommand review, string reviewerName, DateTime at)
    {
        if (!content.CanBeReviewed)
            throw new BusinessRuleException("Only pending items can be reviewed.");

        if (review.Decision == ReviewDecision.Approve)
            content.Approve(reviewerName, at, review.Note);
        else
            content.Reject(reviewerName, at, review.Note!);
    }

    /// <summary>Guards partner edits: the content must belong to the partner's company and not be live yet.</summary>
    public static void EnsureEditableBy(this PartnerContent content, int partnerCompanyId)
    {
        if (content.CompanyId != partnerCompanyId)
            throw new ForbiddenAccessException("This item belongs to another company.");
        if (!content.CanBeEdited)
            throw new BusinessRuleException("Approved items are live for students and can no longer be edited.");
    }
}
