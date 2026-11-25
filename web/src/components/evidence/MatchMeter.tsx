import { Info } from 'lucide-react'
import { ProgressBar } from '@/components/ui/ProgressBar'
import { MATCH_DISCLAIMER } from '@/config/opportunities'
import { cn } from '@/lib/cn'

interface MatchMeterProps {
  percent: number
  className?: string
  /** Hides the explanatory note when it is already shown nearby. */
  hideNote?: boolean
}

/** Rule-based skill overlap, always presented with the responsible-use note. */
export function MatchMeter({ percent, className, hideNote }: MatchMeterProps) {
  return (
    <div className={className}>
      <div className="flex items-end justify-between gap-2">
        <span className="text-xs font-semibold uppercase tracking-wider text-muted">Skill match</span>
        <span className={cn('font-display text-2xl font-bold', percent >= 75 ? 'text-success' : percent >= 40 ? 'text-warning' : 'text-heading')}>
          {percent}%
        </span>
      </div>
      <ProgressBar className="mt-1.5" label="Required skills in your profile" value={percent} />
      {!hideNote && (
        <p className="mt-2 flex gap-1.5 text-xs text-muted">
          <Info className="mt-0.5 size-3.5 shrink-0" /> {MATCH_DISCLAIMER}
        </p>
      )}
    </div>
  )
}
