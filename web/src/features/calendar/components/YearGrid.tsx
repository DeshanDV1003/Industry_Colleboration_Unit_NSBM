import { cn } from '@/lib/cn'
import { entryMeta } from '../config'
import { daysInMonth, firstWeekday, MONTH_NAMES, toIso, todayIso, WEEKDAY_INITIALS } from '../calendarDates'
import type { CalendarEntry } from '../types'

interface YearGridProps {
  year: number
  byDay: Map<string, CalendarEntry[]>
  selected: string | null
  onSelect: (iso: string) => void
}

/** Twelve month grids. Each day shows up to three coloured dots for what is on it. */
export function YearGrid({ year, byDay, selected, onSelect }: YearGridProps) {
  const today = todayIso()

  return (
    <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-4">
      {MONTH_NAMES.map((name, month) => (
        <section key={name} aria-label={`${name} ${year}`} className="rounded-card border border-line bg-surface p-3 shadow-card">
          <h3 className="mb-2 px-1 text-sm font-semibold text-heading">{name}</h3>
          <div className="grid grid-cols-7 gap-0.5 text-center">
            {WEEKDAY_INITIALS.map((w, i) => <span key={i} className="pb-1 text-[10px] font-semibold text-muted">{w}</span>)}
            {Array.from({ length: firstWeekday(year, month) }, (_, i) => <span key={`pad-${i}`} />)}
            {Array.from({ length: daysInMonth(year, month) }, (_, i) => {
              const iso = toIso(year, month, i + 1)
              const entries = byDay.get(iso) ?? []
              return (
                <button
                  key={iso}
                  type="button"
                  onClick={() => onSelect(iso)}
                  aria-label={`${i + 1} ${name}${entries.length ? `, ${entries.length} item${entries.length === 1 ? '' : 's'}` : ''}`}
                  aria-pressed={selected === iso}
                  className={cn(
                    'flex h-9 flex-col items-center justify-center rounded-lg text-xs transition-colors hover:bg-surface-subtle',
                    entries.length > 0 && 'font-semibold text-heading',
                    iso === today && 'ring-1 ring-primary',
                    selected === iso && 'bg-primary text-white hover:bg-primary',
                  )}
                >
                  {i + 1}
                  <span className="mt-0.5 flex h-1.5 gap-0.5">
                    {entries.slice(0, 3).map((e) => (
                      <span key={e.key} className={cn('size-1.5 rounded-full', selected === iso ? 'bg-white' : entryMeta(e).dot)} />
                    ))}
                  </span>
                </button>
              )
            })}
          </div>
        </section>
      ))}
    </div>
  )
}
