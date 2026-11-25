import { CalendarDays, MapPin, Users } from 'lucide-react'
import { Badge } from '@/components/ui/Badge'
import { EVENT_FORMAT_META, teamSizeLabel } from '@/config/events'
import { formatRange } from '@/features/calendar'
import { formatDate } from '@/lib/format'
import type { IndustryEvent } from '../types'

/** Read-only event details, used in the partner preview and the Unit's review dialog. */
export function EventDetailView({ event }: { event: IndustryEvent }) {
  const meta = EVENT_FORMAT_META[event.format]
  return (
    <div className="space-y-4 text-sm">
      <div className="flex flex-wrap gap-1.5">
        <Badge tone={meta.tone}><meta.icon className="size-3" /> {meta.label}</Badge>
        <Badge>{teamSizeLabel(event.minTeamSize, event.maxTeamSize)}</Badge>
        <Badge>{event.companyName}</Badge>
      </div>
      <dl className="grid gap-2 sm:grid-cols-2">
        <div className="flex items-center gap-2"><CalendarDays className="size-4 text-muted" /> {formatRange(event.startDate, event.endDate)}</div>
        <div className="flex items-center gap-2"><MapPin className="size-4 text-muted" /> {event.location}</div>
        <div className="flex items-center gap-2"><Users className="size-4 text-muted" /> {event.registeredCount}/{event.capacity} registered</div>
        <div className="text-muted">Registration closes {formatDate(event.registrationClosesOn)}</div>
      </dl>
      <p className="whitespace-pre-line text-body">{event.description}</p>
      {event.reviewNote && <p className="rounded-lg bg-danger-soft p-3 text-xs text-danger">Reviewer: {event.reviewNote}</p>}
    </div>
  )
}
