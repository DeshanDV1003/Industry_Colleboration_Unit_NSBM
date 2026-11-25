using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Entities.People;

namespace NSBM.ICU.Domain.Entities.Events;

/// <summary>A student's place at an event. The host company records attendance and issues the certificate.</summary>
public class EventRegistration : BaseEntity
{
    public int EventId { get; set; }
    public int StudentId { get; set; }
    public Student? Student { get; set; }
    public DateTime RegisteredAt { get; set; }
    public int? TeamId { get; set; }
    public EventTeam? Team { get; set; }

    public bool Attended { get; private set; }
    public string? CertificateCode { get; private set; }
    public DateTime? CertificateIssuedAt { get; private set; }

    public bool HasCertificate => CertificateCode is not null;

    public void MarkAttendance(bool attended)
    {
        if (HasCertificate && !attended)
            throw new DomainRuleException("A certificate was already issued, so attendance can no longer be removed.");
        Attended = attended;
    }

    /// <summary>Only attendees get a certificate, and each registration gets at most one.</summary>
    public bool TryIssueCertificate(string code, DateTime at)
    {
        if (!Attended || HasCertificate) return false;
        CertificateCode = code;
        CertificateIssuedAt = at;
        return true;
    }
}
