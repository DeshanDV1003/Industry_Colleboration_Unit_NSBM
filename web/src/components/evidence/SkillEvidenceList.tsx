import { Badge } from '@/components/ui/Badge'
import { SKILL_EVIDENCE_META } from '@/config/opportunities'
import { cn } from '@/lib/cn'
import type { SkillEvidenceRow } from '@/types/opportunities'

interface SkillEvidenceListProps {
  rows: SkillEvidenceRow[]
  /** Shows where each practice signal came from (used by readiness and candidate review). */
  showSources?: boolean
  compact?: boolean
}

/** One row per skill with its evidence badge. */
export function SkillEvidenceList({ rows, showSources, compact }: SkillEvidenceListProps) {
  if (rows.length === 0) return <p className="text-sm text-muted">None listed.</p>

  return (
    <ul className={cn('divide-y divide-line', compact && 'text-sm')}>
      {rows.map((row) => {
        const meta = SKILL_EVIDENCE_META[row.evidence]
        return (
          <li key={row.skillId} className={cn('flex items-start justify-between gap-3', compact ? 'py-1.5' : 'py-2.5')}>
            <div className="min-w-0">
              <p className="font-medium text-heading">{row.skillName}</p>
              {showSources && row.practiceSources.length > 0 && (
                <p className="mt-0.5 text-xs text-muted">{row.practiceSources.join(' · ')}</p>
              )}
            </div>
            <Badge tone={meta.tone} title={meta.description}>{meta.label}</Badge>
          </li>
        )
      })}
    </ul>
  )
}
