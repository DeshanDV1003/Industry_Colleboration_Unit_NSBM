import { toast } from '@/components/feedback/toast'
import { VacancyDetailView, useVacancy } from '@/features/vacancies'
import { getErrorMessage } from '@/lib/http'
import { useReviewVacancy } from '../api/moderationApi'
import { ReviewDialog } from './ReviewDialog'

export function VacancyReviewDialog({ vacancyId, onClose }: { vacancyId: number | null; onClose: () => void }) {
  const { data: vacancy } = useVacancy(vacancyId)
  const review = useReviewVacancy()

  return (
    <ReviewDialog
      open={vacancyId !== null}
      item={vacancy}
      pendingDecision={review.isPending ? review.variables.decision : null}
      onClose={onClose}
      onDecide={(decision, note, done) =>
        review.mutate({ id: vacancyId!, decision, note }, {
          onSuccess: () => {
            toast.success(decision === 'Approve' ? 'Vacancy published in the career center.' : 'Vacancy rejected and returned to the company.')
            done()
          },
          onError: (e) => toast.error(getErrorMessage(e)),
        })}
    >
      {vacancy && <VacancyDetailView vacancy={vacancy} />}
    </ReviewDialog>
  )
}
