import { Check, Target } from 'lucide-react'
import { Badge } from '@/components/ui/Badge'
import { cn } from '@/lib/cn'
import type { RoadmapStep } from '../types'

interface RoadmapStepItemProps {
  step: RoadmapStep
  isLast: boolean
  onToggle: (completed: boolean) => void
}

export function RoadmapStepItem({ step, isLast, onToggle }: RoadmapStepItemProps) {
  return (
    <li className="relative flex gap-4 pb-6 last:pb-0">
      {!isLast && <span aria-hidden className={cn('absolute left-[17px] top-10 h-[calc(100%-2.5rem)] w-0.5', step.isCompleted ? 'bg-accent' : 'bg-line')} />}

      <button
        type="button"
        role="checkbox"
        aria-checked={step.isCompleted}
        aria-label={`Mark "${step.title}" as ${step.isCompleted ? 'not done' : 'done'}`}
        onClick={() => onToggle(!step.isCompleted)}
        className={cn(
          'relative z-[1] grid size-9 shrink-0 place-items-center rounded-full border-2 text-sm font-bold transition-colors',
          step.isCompleted
            ? 'border-accent bg-accent text-white'
            : 'border-line-strong bg-surface text-muted hover:border-primary hover:text-primary',
        )}
      >
        {step.isCompleted ? <Check className="size-4" /> : step.order}
      </button>

      <div className={cn('flex-1 rounded-xl border border-line p-4 transition-colors', step.isCompleted && 'bg-accent-soft')}>
        <div className="flex flex-wrap items-center justify-between gap-2">
          <h4 className={cn('font-semibold', step.isCompleted && 'text-accent')}>{step.title}</h4>
          {step.skillName && <Badge>{step.skillName}</Badge>}
        </div>
        <p className="mt-1 text-sm">{step.description}</p>
        {step.outcome && (
          <p className="mt-2 flex items-start gap-1.5 text-xs text-muted">
            <Target className="mt-0.5 size-3.5 shrink-0" /> {step.outcome}
          </p>
        )}
      </div>
    </li>
  )
}
