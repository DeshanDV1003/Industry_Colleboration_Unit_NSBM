import type { CalendarEntry } from './types'

/** ISO date helpers that work in local calendar days, never through UTC time conversion. */
export const toIso = (year: number, month: number, day: number) =>
  `${year}-${String(month + 1).padStart(2, '0')}-${String(day).padStart(2, '0')}`

export const todayIso = () => {
  const d = new Date()
  return toIso(d.getFullYear(), d.getMonth(), d.getDate())
}

export const daysInMonth = (year: number, month: number) => new Date(year, month + 1, 0).getDate()

/** Monday-first column (0–6) of the month's first day. */
export const firstWeekday = (year: number, month: number) => (new Date(year, month, 1).getDay() + 6) % 7

export const MONTH_NAMES = Array.from({ length: 12 }, (_, m) => new Intl.DateTimeFormat('en-GB', { month: 'long' }).format(new Date(2000, m, 1)))
export const WEEKDAY_INITIALS = ['M', 'T', 'W', 'T', 'F', 'S', 'S']

const longDate = new Intl.DateTimeFormat('en-GB', { weekday: 'long', day: 'numeric', month: 'long', year: 'numeric' })
const shortDate = new Intl.DateTimeFormat('en-GB', { day: 'numeric', month: 'short' })
const parse = (iso: string) => { const [y, m, d] = iso.split('-').map(Number); return new Date(y, m - 1, d) }

export const formatLongDay = (iso: string) => longDate.format(parse(iso))
export const formatRange = (start: string, end: string) =>
  start === end ? shortDate.format(parse(start)) : `${shortDate.format(parse(start))} – ${shortDate.format(parse(end))}`

/** Groups entries by every ISO day they cover, so multi-day events appear on each of their days. */
export function indexByDay(entries: CalendarEntry[]): Map<string, CalendarEntry[]> {
  const byDay = new Map<string, CalendarEntry[]>()
  for (const entry of entries) {
    for (let d = parse(entry.startDate); d <= parse(entry.endDate); d.setDate(d.getDate() + 1)) {
      const key = toIso(d.getFullYear(), d.getMonth(), d.getDate())
      byDay.set(key, [...(byDay.get(key) ?? []), entry])
    }
  }
  return byDay
}
