import { useState } from 'react'
import { Link } from 'react-router-dom'
import { CalendarHeart, ClipboardList, Pencil, Plus, Users } from 'lucide-react'
import { ModerationStatusBadge } from '@/components/ModerationStatusBadge'
import { PageHeader } from '@/components/layout/PageHeader'
import { Badge } from '@/components/ui/Badge'
import { Button } from '@/components/ui/Button'
import { Card } from '@/components/ui/Card'
import { Dialog } from '@/components/ui/Dialog'
import { EmptyState } from '@/components/ui/EmptyState'
import { LinkButton } from '@/components/ui/LinkButton'
import { PageSpinner, Spinner } from '@/components/ui/Spinner'
import { EVENT_FORMAT_META } from '@/config/events'
import { formatRange } from '@/features/calendar'
import { getErrorMessage } from '@/lib/http'
import { useCompanyEvents, useIndustryEvent } from '../api/industryEventsApi'
import { EventDetailView } from '../components/EventDetailView'

function EventPreviewDialog({ eventId, onClose }: { eventId: number | null; onClose: () => void }) {
  const { data: event } = useIndustryEvent(eventId)
  return (
    <Dialog
      open={eventId !== null}
      onClose={onClose}
      title={event?.title ?? 'Event'}
      description={event && <ModerationStatusBadge status={event.status} />}
      footer={event?.canBeEdited && <LinkButton to={`/industry-events/${event.id}/edit`}><Pencil className="size-4" /> Edit</LinkButton>}
    >
      {event ? <EventDetailView event={event} /> : <Spinner className="py-10" />}
    </Dialog>
  )
}

/** The partner's hosted events: propose new ones, then run attendance and certificates for approved ones. */
export default function IndustryEventsPage() {
  const { data: events, isLoading, isError, error } = useCompanyEvents()
  const [previewId, setPreviewId] = useState<number | null>(null)

  if (isLoading) return <PageSpinner label="Loading events…" />
  if (isError || !events) return <EmptyState icon={CalendarHeart} title="Events could not be loaded" description={getErrorMessage(error)} />

  const newButton = <LinkButton to="/industry-events/new"><Plus className="size-4" /> Host an event</LinkButton>

  return (
    <>
      <PageHeader
        title="Events and hackathons"
        description="Host tech talks, workshops, challenges and hackathons. Students can register after the Unit approves an event."
        actions={newButton}
      />
      {events.length === 0 ? (
        <EmptyState icon={CalendarHeart} title="No events yet" description="Host a talk or challenge to meet students early." action={newButton} />
      ) : (
        <Card className="overflow-hidden">
          <table className="w-full text-sm">
            <thead className="border-b border-line bg-surface-subtle text-left text-xs font-semibold uppercase tracking-wider text-muted">
              <tr>
                <th className="px-5 py-3">Event</th>
                <th className="hidden px-5 py-3 md:table-cell">Dates</th>
                <th className="px-5 py-3">Status</th>
                <th className="hidden px-5 py-3 lg:table-cell">Registered</th>
                <th className="px-5 py-3"><span className="sr-only">Actions</span></th>
              </tr>
            </thead>
            <tbody className="divide-y divide-line">
              {events.map((e) => {
                const meta = EVENT_FORMAT_META[e.format]
                return (
                  <tr key={e.id} className="align-top">
                    <td className="px-5 py-4">
                      <button onClick={() => setPreviewId(e.id)} className="text-left font-semibold text-heading hover:text-primary">{e.title}</button>
                      <div className="mt-1 flex flex-wrap gap-1.5">
                        <Badge tone={meta.tone}>{meta.label}</Badge>
                        {e.isTeamEvent && <Badge>Teams</Badge>}
                      </div>
                      {e.status === 'Rejected' && e.reviewNote && <p className="mt-2 text-xs text-danger">Reviewer: {e.reviewNote}</p>}
                    </td>
                    <td className="hidden px-5 py-4 md:table-cell">{formatRange(e.startDate, e.endDate)}</td>
                    <td className="px-5 py-4"><ModerationStatusBadge status={e.status} /></td>
                    <td className="hidden px-5 py-4 lg:table-cell">
                      <span className="flex items-center gap-1.5"><Users className="size-4 text-muted" /> {e.registeredCount}/{e.capacity}</span>
                    </td>
                    <td className="px-5 py-4 text-right">
                      {e.status === 'Approved'
                        ? <Link to={`/industry-events/${e.id}/attendance`} className="inline-flex items-center gap-1 text-sm font-semibold"><ClipboardList className="size-4" /> Attendance</Link>
                        : e.canBeEdited
                          ? <Link to={`/industry-events/${e.id}/edit`} className="text-sm font-semibold">Edit</Link>
                          : <Button variant="ghost" size="sm" onClick={() => setPreviewId(e.id)}>View</Button>}
                    </td>
                  </tr>
                )
              })}
            </tbody>
          </table>
        </Card>
      )}
      <EventPreviewDialog eventId={previewId} onClose={() => setPreviewId(null)} />
    </>
  )
}
