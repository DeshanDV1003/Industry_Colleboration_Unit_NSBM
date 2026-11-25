import { cn } from '@/lib/cn'

interface SegmentedControlProps<T extends string> {
  value: T
  options: { value: T; label: string; count?: number }[]
  onChange: (value: T) => void
  ariaLabel: string
}

export function SegmentedControl<T extends string>({ value, options, onChange, ariaLabel }: SegmentedControlProps<T>) {
  return (
    <div role="tablist" aria-label={ariaLabel} className="inline-flex rounded-xl border border-line bg-surface-subtle p-1">
      {options.map((option) => {
        const active = option.value === value
        return (
          <button
            key={option.value}
            role="tab"
            aria-selected={active}
            onClick={() => onChange(option.value)}
            className={cn(
              'flex items-center gap-2 rounded-lg px-3.5 py-1.5 text-sm font-medium transition-colors',
              active ? 'bg-surface text-heading shadow-card' : 'text-muted hover:text-heading',
            )}
          >
            {option.label}
            {option.count !== undefined && (
              <span className={cn('rounded-full px-1.5 text-xs', active ? 'bg-primary-soft text-primary' : 'bg-surface text-muted')}>{option.count}</span>
            )}
          </button>
        )
      })}
    </div>
  )
}
