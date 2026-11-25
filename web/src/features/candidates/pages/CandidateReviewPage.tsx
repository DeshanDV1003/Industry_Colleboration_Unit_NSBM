import { useState } from 'react'
import { useSearchParams } from 'react-router-dom'
import { Briefcase } from 'lucide-react'
import { ApplicationStatusBadge } from '@/components/ApplicationStatusBadge'
import { PageHeader } from '@/components/layout/PageHeader'
import { Card } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { LinkButton } from '@/components/ui/LinkButton'
import { SegmentedControl } from '@/components/ui/SegmentedControl'
import { PageSpinner, Spinner } from '@/components/ui/Spinner'
import { controlClass } from '@/components/ui/controlClass'
import { useCompanyVacancies } from '@/features/vacancies'
import { cn } from '@/lib/cn'
import { formatDate } from '@/lib/format'
import { useCompanyApplications, useSharedProfiles } from '../api/candidatesApi'
import { CandidateEvidencePanel } from '../components/CandidateEvidencePanel'

type Source = 'applicants' | 'shared'

function CandidateButton({ selected, onClick, children }: { selected: boolean; onClick: () => void; children: React.ReactNode }) {
  return (
    <button
      onClick={onClick}
      className={cn('w-full rounded-xl border px-3 py-2.5 text-left transition-colors', selected ? 'border-primary bg-primary-soft' : 'border-line hover:bg-surface-subtle')}
    >
      {children}
    </button>
  )
}

export default function CandidateReviewPage() {
  const [params, setParams] = useSearchParams()
  const { data: vacancies, isLoading } = useCompanyVacancies()
  const approved = vacancies?.filter((v) => v.status === 'Approved') ?? []
  const vacancyId = Number(params.get('vacancy')) || approved[0]?.id || null

  const [source, setSource] = useState<Source>('applicants')
  const [studentId, setStudentId] = useState<number | null>(null)
  const { data: applications, isLoading: appsLoading } = useCompanyApplications(vacancyId)
  const { data: shared } = useSharedProfiles()

  if (isLoading) return <PageSpinner label="Loading candidates…" />
  if (approved.length === 0) {
    return (
      <EmptyState
        icon={Briefcase}
        title="No live vacancies yet"
        description="Candidate review opens once one of your vacancies is approved."
        action={<LinkButton to="/industry-hub">Go to vacancies</LinkButton>}
      />
    )
  }

  const selectVacancy = (id: number) => {
    setParams({ vacancy: String(id) }, { replace: true })
    setStudentId(null)
  }

  return (
    <>
      <PageHeader
        title="Candidate review"
        description="Compare each requirement with the candidate's listed skills and practice signals. Decisions are always made by your team."
      />

      <div className="grid gap-6 xl:grid-cols-[340px_minmax(0,1fr)]">
        <div className="space-y-4">
          <select aria-label="Vacancy" value={vacancyId ?? ''} onChange={(e) => selectVacancy(Number(e.target.value))} className={cn(controlClass(false), 'h-10')}>
            {approved.map((v) => <option key={v.id} value={v.id}>{v.title}</option>)}
          </select>

          <SegmentedControl
            ariaLabel="Candidate source"
            value={source}
            onChange={(s) => { setSource(s); setStudentId(null) }}
            options={[
              { value: 'applicants', label: 'Applicants', count: applications?.length ?? 0 },
              { value: 'shared', label: 'Shared profiles', count: shared?.length ?? 0 },
            ]}
          />

          <Card className="p-3">
            {source === 'applicants' ? (
              appsLoading ? <Spinner className="py-6" /> : applications?.length === 0 ? (
                <p className="p-3 text-sm text-muted">No applications for this vacancy yet.</p>
              ) : (
                <ul className="space-y-2">
                  {applications?.map((a) => (
                    <li key={a.id}>
                      <CandidateButton selected={studentId === a.studentId} onClick={() => setStudentId(a.studentId)}>
                        <div className="flex items-center justify-between gap-2">
                          <span className="font-semibold text-heading">{a.studentName}</span>
                          <ApplicationStatusBadge status={a.status} />
                        </div>
                        <p className="mt-0.5 text-xs text-muted">{a.studentNo} · applied {formatDate(a.submittedAt)} · {a.matchPercentAtSubmission}% match then</p>
                      </CandidateButton>
                    </li>
                  ))}
                </ul>
              )
            ) : shared?.length === 0 ? (
              <p className="p-3 text-sm text-muted">No students have opted in to profile sharing yet.</p>
            ) : (
              <ul className="space-y-2">
                {shared?.map((p) => (
                  <li key={p.studentId}>
                    <CandidateButton selected={studentId === p.studentId} onClick={() => setStudentId(p.studentId)}>
                      <span className="font-semibold text-heading">{p.studentName}</span>
                      <p className="mt-0.5 text-xs text-muted">{p.studentNo} · {p.careerPathName ?? 'No career path yet'}</p>
                    </CandidateButton>
                  </li>
                ))}
              </ul>
            )}
          </Card>
        </div>

        <CandidateEvidencePanel studentId={studentId} vacancyId={vacancyId} />
      </div>
    </>
  )
}
