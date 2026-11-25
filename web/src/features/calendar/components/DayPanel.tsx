import { Link } from 'react-router-dom'
import { ArrowRight, CalendarDays, MapPin, Pencil, Plus, Users } from 'lucide-react'
import { Badge } from '@/components/ui/Badge'
import { Button } from '@/components/ui/Button'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import type { Role } from '@/config/roles'
import { AUDIENCE_LABEL, entryLink, entryMeta } from '../config'
import { formatLongDay, formatRange } from '../calendarDates'
import type { CalendarEntry } from '../types'

interface DayPanelProps {
  day: string
  entries: CalendarEntry[]
  role: Role
  onAdd?: () => void
  onEdit?: (entry: CalendarEntry) => void
}

/** The full picture for one day: every event and personal date on it. */
export function DayPanel({ day, entries, role, onAdd, onEdit }: DayPanelProps) {
  return (
    <Card>
      <CardHeader
        title={formatLongDay(day)}
        description={entries.length === 0 ? 'Nothing planned.' : `${entries.length} item${entries.length === 1 ? '' : 's'}`}
        action={onAdd && <Button size="sm" onClick={onAdd}><Plus className="size-4" /> Add event</Button>}
      />
      <CardBody className="space-y-3">
        {entries.length === 0 && <p className="flex items-center gap-2 text-sm text-muted"><CalendarDays className="size-4" /> Pick another day, or add an event here.</p>}
        {entries.map((entry) => {
          const meta = entryMeta(entry)
          const link = entryLink(entry, role)
          return (
            <article key={entry.key} className="rounded-xl border border-line p-3">
              <div className="flex flex-wrap items-center gap-1.5">
                <Badge tone={meta.tone}>{meta.label}</Badge>
                {entry.audience && <Badge><Users className="size-3" /> {AUDIENCE_LABEL[entry.audience]}</Badge>}
                {entry.careerPathName && <Badge tone="primary">{entry.careerPathName}</Badge>}
              </div>
              <h4 className="mt-2 font-semibold text-heading">{entry.title}</h4>
              <p className="text-xs text-muted">
                {formatRange(entry.startDate, entry.endDate)}
                {entry.location && <> · <MapPin className="inline size-3" /> {entry.location}</>}
              </p>
              {entry.description && <p className="mt-1.5 text-sm">{entry.description}</p>}
              {(link || (onEdit && entry.eventId)) && (
                <div className="mt-2 flex gap-2">
                  {link && <Link to={link} className="inline-flex items-center gap-1 text-xs font-semibold">Open <ArrowRight className="size-3" /></Link>}
                  {onEdit && entry.eventId && (
                    <button type="button" onClick={() => onEdit(entry)} className="inline-flex items-center gap-1 text-xs font-semibold text-muted hover:text-heading">
                      <Pencil className="size-3" /> Edit
                    </button>
                  )}
                </div>
              )}
            </article>
          )
        })}
      </CardBody>
    </Card>
  )
}
