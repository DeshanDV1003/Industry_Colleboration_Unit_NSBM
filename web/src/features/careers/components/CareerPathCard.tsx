import { CheckCircle2 } from 'lucide-react'
import { Badge } from '@/components/ui/Badge'
import { Button } from '@/components/ui/Button'
import { Card } from '@/components/ui/Card'
import { cn } from '@/lib/cn'
import { CareerIcon } from './CareerIcon'
import type { CareerPathSummary } from '../types'

const MAX_SKILLS_SHOWN = 5

interface CareerPathCardProps {
  path: CareerPathSummary
  selecting: boolean
  onDetails: () => void
  onSelect: () => void
}

export function CareerPathCard({ path, selecting, onDetails, onSelect }: CareerPathCardProps) {
  const hiddenSkills = path.coreSkills.length - MAX_SKILLS_SHOWN

  return (
    <Card className={cn('flex flex-col p-6 transition-shadow hover:shadow-pop', path.isSelected && 'border-primary ring-4 ring-primary-soft')}>
      <div className="flex items-start justify-between gap-3">
        <span className="grid size-12 place-items-center rounded-xl bg-brand text-white">
          <CareerIcon slug={path.slug} className="size-6" />
        </span>
        {path.isSelected && (
          <Badge tone="primary">
            <CheckCircle2 className="size-3.5" /> Your path
          </Badge>
        )}
      </div>

      <h3 className="mt-4 text-lg font-semibold">{path.name}</h3>
      <p className="mt-1 text-muted">{path.tagline}</p>

      <p className="mt-5 text-xs font-bold uppercase tracking-wider text-muted">Entry-level roles</p>
      <ul className="mt-1.5 space-y-0.5 text-sm text-heading">
        {path.entryRoles.map((role) => <li key={role}>{role}</li>)}
      </ul>

      <p className="mt-5 text-xs font-bold uppercase tracking-wider text-muted">Core skills</p>
      <div className="mt-2 flex flex-wrap gap-1.5">
        {path.coreSkills.slice(0, MAX_SKILLS_SHOWN).map((skill) => <Badge key={skill}>{skill}</Badge>)}
        {hiddenSkills > 0 && <Badge>+{hiddenSkills} more</Badge>}
      </div>

      <div className="mt-auto flex gap-2 pt-6">
        <Button variant="secondary" className="flex-1" onClick={onDetails}>
          View details
        </Button>
        {!path.isSelected && (
          <Button className="flex-1" loading={selecting} onClick={onSelect}>
            Choose this path
          </Button>
        )}
      </div>
    </Card>
  )
}
