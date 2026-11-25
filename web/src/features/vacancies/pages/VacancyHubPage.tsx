import { useState } from 'react'
import { Link } from 'react-router-dom'
import { Briefcase, Pencil, Plus, Users } from 'lucide-react'
import { ModerationStatusBadge } from '@/components/ModerationStatusBadge'
import { PageHeader } from '@/components/layout/PageHeader'
import { Badge } from '@/components/ui/Badge'
import { Button } from '@/components/ui/Button'
import { Card } from '@/components/ui/Card'
import { Dialog } from '@/components/ui/Dialog'
import { EmptyState } from '@/components/ui/EmptyState'
import { LinkButton } from '@/components/ui/LinkButton'
import { PageSpinner, Spinner } from '@/components/ui/Spinner'
import { WORK_MODE_LABEL } from '@/config/opportunities'
import { formatDate } from '@/lib/format'
import { getErrorMessage } from '@/lib/http'
import { useCompanyVacancies, useVacancy } from '../api/vacanciesApi'
import { VacancyDetailView } from '../components/VacancyDetailView'

function VacancyPreviewDialog({ vacancyId, onClose }: { vacancyId: number | null; onClose: () => void }) {
  const { data: vacancy } = useVacancy(vacancyId)
  return (
    <Dialog
      open={vacancyId !== null}
      onClose={onClose}
      title={vacancy?.title ?? 'Vacancy'}
      description={vacancy && <ModerationStatusBadge status={vacancy.status} />}
      footer={vacancy?.canBeEdited && <LinkButton to={`/industry-hub/vacancies/${vacancy.id}/edit`}><Pencil className="size-4" /> Edit</LinkButton>}
    >
      {vacancy ? <VacancyDetailView vacancy={vacancy} /> : <Spinner className="py-10" />}
    </Dialog>
  )
}

export default function VacancyHubPage() {
  const { data: vacancies, isLoading, isError, error } = useCompanyVacancies()
  const [previewId, setPreviewId] = useState<number | null>(null)

  if (isLoading) return <PageSpinner label="Loading vacancies…" />
  if (isError) return <EmptyState icon={Briefcase} title="Vacancies could not be loaded" description={getErrorMessage(error)} />

  const newButton = <LinkButton to="/industry-hub/vacancies/new"><Plus className="size-4" /> Post vacancy</LinkButton>

  return (
    <>
      <PageHeader
        title="Internship vacancies"
        description="Practice vacancies your company has posted. Students can search them after the Unit approves them."
        actions={newButton}
      />

      {vacancies?.length === 0 ? (
        <EmptyState icon={Briefcase} title="No vacancies yet" description="Post an internship so students can see what you look for." action={newButton} />
      ) : (
        <Card className="overflow-hidden">
          <table className="w-full text-sm">
            <thead className="border-b border-line bg-surface-subtle text-left text-xs font-semibold uppercase tracking-wider text-muted">
              <tr>
                <th className="px-5 py-3">Vacancy</th>
                <th className="hidden px-5 py-3 md:table-cell">Closes</th>
                <th className="px-5 py-3">Status</th>
                <th className="hidden px-5 py-3 lg:table-cell">Applicants</th>
                <th className="px-5 py-3"><span className="sr-only">Actions</span></th>
              </tr>
            </thead>
            <tbody className="divide-y divide-line">
              {vacancies?.map((v) => (
                <tr key={v.id} className="align-top">
                  <td className="px-5 py-4">
                    <button onClick={() => setPreviewId(v.id)} className="text-left font-semibold text-heading hover:text-primary">{v.title}</button>
                    <div className="mt-1 flex flex-wrap gap-1.5">
                      <Badge tone="primary">{v.careerPathName}</Badge>
                      <Badge>{WORK_MODE_LABEL[v.workMode]}</Badge>
                    </div>
                    {v.status === 'Rejected' && v.reviewNote && <p className="mt-2 text-xs text-danger">Reviewer: {v.reviewNote}</p>}
                  </td>
                  <td className="hidden px-5 py-4 md:table-cell">{formatDate(v.closingDate)}</td>
                  <td className="px-5 py-4"><ModerationStatusBadge status={v.status} /></td>
                  <td className="hidden px-5 py-4 lg:table-cell">
                    {v.applicationCount > 0
                      ? <Link to={`/candidate-review?vacancy=${v.id}`} className="flex items-center gap-1.5 font-semibold"><Users className="size-4" /> {v.applicationCount}</Link>
                      : <span className="flex items-center gap-1.5 text-muted"><Users className="size-4" /> 0</span>}
                  </td>
                  <td className="px-5 py-4 text-right">
                    {v.status === 'Approved'
                      ? <Button variant="ghost" size="sm" onClick={() => setPreviewId(v.id)}>View</Button>
                      : <Link to={`/industry-hub/vacancies/${v.id}/edit`} className="text-sm font-semibold">Edit</Link>}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </Card>
      )}

      <VacancyPreviewDialog vacancyId={previewId} onClose={() => setPreviewId(null)} />
    </>
  )
}
