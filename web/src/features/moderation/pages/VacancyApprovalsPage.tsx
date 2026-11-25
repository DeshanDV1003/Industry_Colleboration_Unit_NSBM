import { useState } from 'react'
import { ListChecks } from 'lucide-react'
import { PageHeader } from '@/components/layout/PageHeader'
import { Badge } from '@/components/ui/Badge'
import { EmptyState } from '@/components/ui/EmptyState'
import { PageSpinner } from '@/components/ui/Spinner'
import { WORK_MODE_LABEL } from '@/config/opportunities'
import { formatDate } from '@/lib/format'
import { getErrorMessage } from '@/lib/http'
import { useVacancyReviewQueue } from '../api/moderationApi'
import { ModerationBoard } from '../components/ModerationBoard'
import { VacancyReviewDialog } from '../components/VacancyReviewDialog'

export default function VacancyApprovalsPage() {
  const { data: vacancies, isLoading, isError, error } = useVacancyReviewQueue()
  const [reviewId, setReviewId] = useState<number | null>(null)

  if (isLoading) return <PageSpinner label="Loading the review queue…" />
  if (isError || !vacancies) return <EmptyState icon={ListChecks} title="Review queue could not be loaded" description={getErrorMessage(error)} />

  return (
    <>
      <PageHeader
        title="Vacancy approvals"
        description="Check proposed internships before they appear in the career center: realistic roles, fair skill lists and a clear description."
      />
      <ModerationBoard
        items={vacancies}
        emptyHint="New and edited vacancies will appear here."
        onOpen={(vacancy) => setReviewId(vacancy.id)}
        renderTags={(vacancy) => (
          <>
            <Badge tone="primary">{vacancy.careerPathName}</Badge>
            <Badge>{WORK_MODE_LABEL[vacancy.workMode]}</Badge>
            <Badge>Closes {formatDate(vacancy.closingDate)}</Badge>
          </>
        )}
      />
      <VacancyReviewDialog vacancyId={reviewId} onClose={() => setReviewId(null)} />
    </>
  )
}
