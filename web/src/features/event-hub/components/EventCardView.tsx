import { Link } from 'react-router-dom'
import { Award, Building2, CalendarDays, CheckCircle2, MapPin, Users } from 'lucide-react'
import { Badge } from '@/components/ui/Badge'
import { Card } from '@/components/ui/Card'
import { EVENT_FORMAT_META, teamSizeLabel } from '@/config/events'
import { formatRange } from '@/features/calendar'
import type { EventCard } from '../types'

function StatusBadge({ event }: { event: EventCard }) {
  if (event.hasCertificate) return <Badge tone="success"><Award className="size-3" /> Certificate</Badge>
  if (event.isRegistered) return <Badge tone="success"><CheckCircle2 className="size-3" /> Registered</Badge>
  if (event.hasEnded) return <Badge>Ended</Badge>
  if (event.isOpen) return <Badge tone="primary">Open</Badge>
  return <Badge tone="warning">{event.registeredCount >= event.capacity ? 'Full' : 'Registration closed'}</Badge>
}

export function EventCardView({ event }: { event: EventCard }) {
  const meta = EVENT_FORMAT_META[event.format]
  return (
    <Card className="flex flex-col p-5 transition-shadow hover:shadow-pop">
      <div className="flex flex-wrap items-center gap-1.5">
        <Badge tone={meta.tone}><meta.icon className="size-3" /> {meta.label}</Badge>
        <Badge>{teamSizeLabel(event.minTeamSize, event.maxTeamSize)}</Badge>
        <span className="ml-auto"><StatusBadge event={event} /></span>
      </div>
      <Link to={`/events/${event.id}`} className="mt-3 text-base font-semibold text-heading hover:text-primary">{event.title}</Link>
      <div className="mt-2 space-y-1 text-xs text-muted">
        <p className="flex items-center gap-1.5"><Building2 className="size-3.5" /> {event.companyName}</p>
        <p className="flex items-center gap-1.5"><CalendarDays className="size-3.5" /> {formatRange(event.startDate, event.endDate)}</p>
        <p className="flex items-center gap-1.5"><MapPin className="size-3.5" /> {event.location}</p>
      </div>
      <p className="mt-auto flex items-center gap-1.5 border-t border-line pt-3 text-xs text-muted">
        <Users className="size-3.5" /> {event.registeredCount}/{event.capacity} places taken
      </p>
    </Card>
  )
}
