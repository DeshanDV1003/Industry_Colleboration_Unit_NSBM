import { Building2, CalendarClock, MapPin } from 'lucide-react'
import { ApplicationStatusBadge } from '@/components/ApplicationStatusBadge'
import { MatchMeter } from '@/components/evidence/MatchMeter'
import { Badge } from '@/components/ui/Badge'
import { Button } from '@/components/ui/Button'
import { Card } from '@/components/ui/Card'
import { WORK_MODE_LABEL } from '@/config/opportunities'
import { formatDate } from '@/lib/format'
import type { VacancyMatch } from '../types'

export function VacancyMatchCard({ vacancy, onOpen }: { vacancy: VacancyMatch; onOpen: () => void }) {
  const missing = vacancy.required.filter((r) => r.evidence === 'Missing')

  return (
    <Card className="flex flex-col p-5">
      <div className="flex items-start justify-between gap-3">
        <div className="min-w-0">
          <p className="flex items-center gap-1 text-xs font-semibold text-muted"><Building2 className="size-3.5" /> {vacancy.companyName}</p>
          <h3 className="mt-1 text-base font-semibold">{vacancy.title}</h3>
        </div>
        {vacancy.myApplicationStatus && <ApplicationStatusBadge status={vacancy.myApplicationStatus} />}
      </div>

      <div className="mt-2 flex flex-wrap items-center gap-2 text-xs text-muted">
        <Badge tone="primary">{vacancy.careerPathName}</Badge>
        <Badge>{WORK_MODE_LABEL[vacancy.workMode]}</Badge>
        <span className="flex items-center gap-1"><MapPin className="size-3.5" /> {vacancy.location}</span>
        <span className="flex items-center gap-1"><CalendarClock className="size-3.5" /> Closes {formatDate(vacancy.closingDate)}</span>
      </div>

      <MatchMeter percent={vacancy.matchPercent} hideNote className="mt-4" />

      <div className="mt-3 text-sm">
        {missing.length === 0 ? (
          <p className="text-success">Every required skill appears in your profile.</p>
        ) : (
          <>
            <p className="text-xs font-semibold text-muted">Missing required skills</p>
            <div className="mt-1 flex flex-wrap gap-1">{missing.map((m) => <Badge key={m.skillId} tone="danger">{m.skillName}</Badge>)}</div>
          </>
        )}
      </div>

      <div className="mt-auto pt-4">
        <Button variant={vacancy.myApplicationStatus ? 'secondary' : 'primary'} className="w-full" onClick={onOpen}>
          {vacancy.myApplicationStatus ? 'View details' : 'View and apply'}
        </Button>
      </div>
    </Card>
  )
}
