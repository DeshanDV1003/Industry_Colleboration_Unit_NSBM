import { toast } from '@/components/feedback/toast'
import { EventDetailView, useIndustryEvent } from '@/features/industry-events'
import { getErrorMessage } from '@/lib/http'
import { useReviewEvent } from '../api/moderationApi'
import { ReviewDialog } from './ReviewDialog'

export function EventReviewDialog({ eventId, onClose }: { eventId: number | null; onClose: () => void }) {
  const { data: event } = useIndustryEvent(eventId)
  const review = useReviewEvent()

  return (
    <ReviewDialog
      open={eventId !== null}
      item={event}
      pendingDecision={review.isPending ? review.variables.decision : null}
      onClose={onClose}
      onDecide={(decision, note, done) =>
        review.mutate({ id: eventId!, decision, note }, {
          onSuccess: () => {
            toast.success(decision === 'Approve' ? 'Event published. Students have been told about it.' : 'Event returned to the company.')
            done()
          },
          onError: (e) => toast.error(getErrorMessage(e)),
        })}
    >
      {event && <EventDetailView event={event} />}
    </ReviewDialog>
  )
}
