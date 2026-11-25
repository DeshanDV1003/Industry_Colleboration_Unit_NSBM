import { ClipboardCheck, Info } from 'lucide-react'
import { ApplicationStatusBadge } from '@/components/ApplicationStatusBadge'
import { PageHeader } from '@/components/layout/PageHeader'
import { Badge } from '@/components/ui/Badge'
import { Card } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { LinkButton } from '@/components/ui/LinkButton'
import { PageSpinner } from '@/components/ui/Spinner'
import { WORK_MODE_LABEL } from '@/config/opportunities'
import { formatDate, formatDateTime } from '@/lib/format'
import { getErrorMessage } from '@/lib/http'
import { useMyApplications } from '../api/careerCenterApi'

export default function ApplicationsPage() {
  const { data: applications, isLoading, isError, error } = useMyApplications()

  if (isLoading) return <PageSpinner label="Loading your applications…" />
  if (isError) return <EmptyState icon={ClipboardCheck} title="Applications could not be loaded" description={getErrorMessage(error)} />

  return (
    <>
      <PageHeader
        title="My applications"
        description="Practice applications you have sent. Companies update the status; you see changes here straight away."
        actions={<LinkButton to="/career-center" variant="secondary">Browse internships</LinkButton>}
      />

      {applications?.length === 0 ? (
        <EmptyState
          icon={ClipboardCheck}
          title="No applications yet"
          description="Find an internship that fits your skills in the match center."
          action={<LinkButton to="/career-center">Open the match center</LinkButton>}
        />
      ) : (
        <div className="space-y-4">
          {applications?.map((a) => (
            <Card key={a.id} className="p-5">
              <div className="flex flex-wrap items-start justify-between gap-3">
                <div>
                  <p className="text-xs font-semibold text-muted">{a.companyName}</p>
                  <h3 className="mt-0.5 text-base font-semibold">{a.vacancyTitle}</h3>
                  <div className="mt-2 flex flex-wrap gap-1.5">
                    <Badge>{WORK_MODE_LABEL[a.workMode]}</Badge>
                    <Badge>{a.matchPercentAtSubmission}% skill match when applied</Badge>
                  </div>
                </div>
                <div className="text-right">
                  <ApplicationStatusBadge status={a.status} />
                  <p className="mt-1 text-xs text-muted">Updated {formatDateTime(a.statusChangedAt)}</p>
                </div>
              </div>
              <p className="mt-3 line-clamp-2 text-sm text-muted">“{a.coverNote}”</p>
              <p className="mt-2 text-xs text-muted">Applied {formatDate(a.submittedAt)}</p>
            </Card>
          ))}
          <p className="flex gap-2 text-xs text-muted">
            <Info className="mt-0.5 size-3.5 shrink-0" /> These are practice applications to fictional vacancies. Status changes are made by people at the company, never automatically.
          </p>
        </div>
      )}
    </>
  )
}
