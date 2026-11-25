/** Week helpers on ISO "yyyy-mm-dd" days, computed in local calendar days (never through UTC). */
const parse = (iso: string) => { const [y, m, d] = iso.split('-').map(Number); return new Date(y, m - 1, d) }
const format = (d: Date) => `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`

export const todayIso = () => format(new Date())

export const addDays = (iso: string, days: number) => {
  const d = parse(iso)
  d.setDate(d.getDate() + days)
  return format(d)
}

/** The Monday of the week containing the day. */
export const mondayOf = (iso: string) => addDays(iso, -((parse(iso).getDay() + 6) % 7))

export const weekDays = (monday: string) => Array.from({ length: 7 }, (_, i) => addDays(monday, i))

const dayLabel = new Intl.DateTimeFormat('en-GB', { weekday: 'long', day: 'numeric', month: 'short' })
const rangeLabel = new Intl.DateTimeFormat('en-GB', { day: 'numeric', month: 'short', year: 'numeric' })

export const formatDay = (iso: string) => dayLabel.format(parse(iso))
export const formatWeek = (monday: string) => `${rangeLabel.format(parse(monday))} – ${rangeLabel.format(parse(addDays(monday, 6)))}`
