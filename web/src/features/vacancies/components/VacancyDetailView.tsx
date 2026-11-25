import { CalendarClock, MapPin, MessageSquareText, Timer } from 'lucide-react'
import { Badge } from '@/components/ui/Badge'
import { WORK_MODE_LABEL } from '@/config/opportunities'
import { formatDate } from '@/lib/format'
import type { Vacancy } from '../types'

/** Read-only view of a complete vacancy. Used by partners and reviewers. */
export function VacancyDetailView({ vacancy }: { vacancy: Vacancy }) {
  return (
    <div className="space-y-5">
      <div className="flex flex-wrap items-center gap-2 text-sm">
        <Badge tone="primary">{vacancy.careerPathName}</Badge>
        <Badge>{WORK_MODE_LABEL[vacancy.workMode]}</Badge>
        <span className="flex items-center gap-1 text-muted"><MapPin className="size-3.5" /> {vacancy.location}</span>
        <span className="flex items-center gap-1 text-muted"><Timer className="size-3.5" /> {vacancy.durationMonths} months</span>
        <span className="flex items-center gap-1 text-muted"><CalendarClock className="size-3.5" /> Closes {formatDate(vacancy.closingDate)}</span>
      </div>

      {vacancy.reviewNote && (
        <div className="flex gap-2.5 rounded-xl bg-warning-soft px-4 py-3 text-sm text-heading">
          <MessageSquareText className="mt-0.5 size-4 shrink-0 text-warning" />
          <div>
            <p className="font-semibold">Reviewer note{vacancy.reviewedBy && ` · ${vacancy.reviewedBy}`}</p>
            <p className="mt-0.5">{vacancy.reviewNote}</p>
          </div>
        </div>
      )}

      <section>
        <h3 className="mb-2 text-xs font-bold uppercase tracking-wider text-muted">About the internship</h3>
        <p className="whitespace-pre-line">{vacancy.description}</p>
      </section>
      <section>
        <h3 className="mb-2 text-xs font-bold uppercase tracking-wider text-muted">Required skills</h3>
        <div className="flex flex-wrap gap-1.5">{vacancy.requiredSkills.map((s) => <Badge key={s.id} tone="primary">{s.name}</Badge>)}</div>
      </section>
      <section>
        <h3 className="mb-2 text-xs font-bold uppercase tracking-wider text-muted">Preferred skills</h3>
        {vacancy.preferredSkills.length === 0
          ? <p className="text-sm text-muted">None.</p>
          : <div className="flex flex-wrap gap-1.5">{vacancy.preferredSkills.map((s) => <Badge key={s.id}>{s.name}</Badge>)}</div>}
      </section>
    </div>
  )
}
