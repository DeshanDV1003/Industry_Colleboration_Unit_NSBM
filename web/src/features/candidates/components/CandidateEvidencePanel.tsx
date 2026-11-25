import { Info, UserRound } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { ApplicationStatusBadge } from '@/components/ApplicationStatusBadge'
import { MatchMeter } from '@/components/evidence/MatchMeter'
import { SkillEvidenceList } from '@/components/evidence/SkillEvidenceList'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { Spinner } from '@/components/ui/Spinner'
import { controlClass } from '@/components/ui/controlClass'
import { APPLICATION_STATUS_META, COMPANY_APPLICATION_STATUSES } from '@/config/opportunities'
import { cn } from '@/lib/cn'
import { getErrorMessage } from '@/lib/http'
import type { ApplicationStatus } from '@/types/opportunities'
import { useCandidateEvidence, useUpdateApplicationStatus } from '../api/candidatesApi'

interface CandidateEvidencePanelProps {
  studentId: number | null
  vacancyId: number | null
}

/** Each requirement compared with the candidate's claims and practice signals, plus the application status. */
export function CandidateEvidencePanel({ studentId, vacancyId }: CandidateEvidencePanelProps) {
  const { data: candidate, isLoading, isError, error } = useCandidateEvidence(studentId, vacancyId)
  const updateStatus = useUpdateApplicationStatus()

  if (studentId === null) {
    return <EmptyState icon={UserRound} title="Select a candidate" description="Pick an applicant or an opted-in profile to compare their evidence with this vacancy." />
  }
  if (isLoading) return <Card className="grid h-64 place-items-center"><Spinner /></Card>
  if (isError || !candidate) return <EmptyState icon={UserRound} title="Evidence not available" description={getErrorMessage(error)} />

  const changeStatus = (status: ApplicationStatus) =>
    updateStatus.mutate(
      { applicationId: candidate.applicationId!, status },
      {
        onSuccess: () => toast.success(`Application marked as ${APPLICATION_STATUS_META[status].label.toLowerCase()}. The student sees it in their tracker.`),
        onError: (e) => toast.error(getErrorMessage(e)),
      },
    )

  return (
    <div className="space-y-5">
      <Card>
        <CardBody className="flex flex-wrap items-start justify-between gap-4">
          <div>
            <h2 className="text-lg font-semibold">{candidate.studentName}</h2>
            <p className="text-sm text-muted">{candidate.studentNo} · {candidate.programme}</p>
            {candidate.careerPathName && <p className="mt-1 text-sm">Preparing for <strong className="text-heading">{candidate.careerPathName}</strong></p>}
          </div>
          {candidate.applicationId !== null && candidate.applicationStatus ? (
            <div className="min-w-[220px]">
              <div className="mb-1.5 flex items-center justify-between gap-2">
                <span className="text-xs font-semibold uppercase tracking-wider text-muted">Application</span>
                <ApplicationStatusBadge status={candidate.applicationStatus} />
              </div>
              <select
                aria-label="Change application status"
                value={candidate.applicationStatus === 'Submitted' ? '' : candidate.applicationStatus}
                disabled={updateStatus.isPending}
                onChange={(e) => changeStatus(e.target.value as ApplicationStatus)}
                className={cn(controlClass(false), 'h-10')}
              >
                {candidate.applicationStatus === 'Submitted' && <option value="" disabled>Move to…</option>}
                {COMPANY_APPLICATION_STATUSES.map((s) => <option key={s} value={s}>{APPLICATION_STATUS_META[s].label}</option>)}
              </select>
            </div>
          ) : (
            <span className="rounded-full bg-accent-soft px-3 py-1 text-xs font-semibold text-accent">Opted-in profile</span>
          )}
        </CardBody>
      </Card>

      <Card>
        <CardBody><MatchMeter percent={candidate.matchPercent} /></CardBody>
      </Card>

      <div className="grid gap-5 xl:grid-cols-2">
        <Card>
          <CardHeader title="Required skills" description={`For ${candidate.vacancyTitle}`} />
          <CardBody><SkillEvidenceList rows={candidate.required} showSources /></CardBody>
        </Card>
        <Card>
          <CardHeader title="Preferred skills" />
          <CardBody><SkillEvidenceList rows={candidate.preferred} showSources /></CardBody>
        </Card>
      </div>

      {candidate.coverNote && (
        <Card>
          <CardHeader title="Cover note" />
          <CardBody><p className="whitespace-pre-line">{candidate.coverNote}</p></CardBody>
        </Card>
      )}

      <p className="flex gap-2 text-xs text-muted">
        <Info className="mt-0.5 size-3.5 shrink-0" />
        Evidence helps a person review the candidate. Practice signals are indications, not proof — the platform never makes a hiring decision.
      </p>
    </div>
  )
}
