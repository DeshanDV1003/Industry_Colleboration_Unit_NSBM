import { cn } from '@/lib/cn'

interface ProgressBarProps {
  value: number
  label: string
  className?: string
}

/** Horizontal progress from 0–100. `label` describes the measure for assistive technology. */
export function ProgressBar({ value, label, className }: ProgressBarProps) {
  const clamped = Math.min(100, Math.max(0, value))

  return (
    <div
      role="progressbar"
      aria-label={label}
      aria-valuenow={clamped}
      aria-valuemin={0}
      aria-valuemax={100}
      className={cn('h-2 overflow-hidden rounded-full bg-surface-subtle', className)}
    >
      <div className="h-full rounded-full bg-brand transition-all duration-500" style={{ width: `${clamped}%` }} />
    </div>
  )
}
