import { useState } from 'react'
import { CalendarHeart } from 'lucide-react'
import { PageHeader } from '@/components/layout/PageHeader'
import { EmptyState } from '@/components/ui/EmptyState'
import { LinkButton } from '@/components/ui/LinkButton'
import { SegmentedControl } from '@/components/ui/SegmentedControl'
import { PageSpinner } from '@/components/ui/Spinner'
import { getErrorMessage } from '@/lib/http'
import { useEventHub } from '../api/eventHubApi'
import { EventCardView } from '../components/EventCardView'
import type { EventCard } from '../types'

type Filter = 'upcoming' | 'mine' | 'past'

const FILTERS: Record<Filter, { label: string; match: (e: EventCard) => boolean }> = {
  upcoming: { label: 'Upcoming', match: (e) => !e.hasEnded },
  mine: { label: 'My events', match: (e) => e.isRegistered },
  past: { label: 'Past', match: (e) => e.hasEnded },
}

/** Partner talks, workshops, challenges and hackathons students can join. */
export default function EventHubPage() {
  const { data: events, isLoading, isError, error } = useEventHub()
  const [filter, setFilter] = useState<Filter>('upcoming')

  if (isLoading) return <PageSpinner label="Loading events…" />
  if (isError || !events) return <EmptyState icon={CalendarHeart} title="Events could not be loaded" description={getErrorMessage(error)} />

  const visible = events.filter(FILTERS[filter].match)

  return (
    <>
      <PageHeader
        title="Events and hackathons"
        description="Meet partner companies at talks, workshops, challenges and hackathons. Take part to earn a verifiable certificate."
        actions={<LinkButton to="/certificates" variant="secondary">My certificates</LinkButton>}
      />
      <div className="mb-4">
        <SegmentedControl
          ariaLabel="Filter events"
          value={filter}
          onChange={setFilter}
          options={(Object.keys(FILTERS) as Filter[]).map((f) => ({ value: f, label: FILTERS[f].label, count: events.filter(FILTERS[f].match).length }))}
        />
      </div>
      {visible.length === 0 ? (
        <EmptyState icon={CalendarHeart} title="No events here" description={filter === 'mine' ? 'Register for an upcoming event to see it here.' : 'Check back soon for new partner events.'} />
      ) : (
        <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
          {visible.map((e) => <EventCardView key={e.id} event={e} />)}
        </div>
      )}
    </>
  )
}
