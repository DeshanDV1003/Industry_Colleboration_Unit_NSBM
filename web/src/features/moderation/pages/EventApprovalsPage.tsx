import { useState } from 'react'
import { CalendarHeart } from 'lucide-react'
import { PageHeader } from '@/components/layout/PageHeader'
import { Badge } from '@/components/ui/Badge'
import { EmptyState } from '@/components/ui/EmptyState'
import { PageSpinner } from '@/components/ui/Spinner'
import { EVENT_FORMAT_META } from '@/config/events'
import { formatRange } from '@/features/calendar'
import { getErrorMessage } from '@/lib/http'
import { useEventReviewQueue } from '../api/moderationApi'
import { EventReviewDialog } from '../components/EventReviewDialog'
import { ModerationBoard } from '../components/ModerationBoard'

export default function EventApprovalsPage() {
  const { data: events, isLoading, isError, error } = useEventReviewQueue()
  const [reviewId, setReviewId] = useState<number | null>(null)

  if (isLoading) return <PageSpinner label="Loading the review queue…" />
  if (isError || !events) return <EmptyState icon={CalendarHeart} title="Review queue could not be loaded" description={getErrorMessage(error)} />

  return (
    <>
      <PageHeader
        title="Event approvals"
        description="Check partner talks, workshops and hackathons before students can register: a clear purpose, realistic dates and a safe location."
      />
      <ModerationBoard
        items={events}
        emptyHint="New and edited events will appear here."
        onOpen={(event) => setReviewId(event.id)}
        renderTags={(event) => (
          <>
            <Badge tone={EVENT_FORMAT_META[event.format].tone}>{EVENT_FORMAT_META[event.format].label}</Badge>
            <Badge>{formatRange(event.startDate, event.endDate)}</Badge>
            {event.isTeamEvent && <Badge>Teams</Badge>}
          </>
        )}
      />
      <EventReviewDialog eventId={reviewId} onClose={() => setReviewId(null)} />
    </>
  )
}
