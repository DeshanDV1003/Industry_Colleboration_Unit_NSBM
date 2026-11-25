import { useMemo, useState } from 'react'
import { CalendarDays, ChevronLeft, ChevronRight } from 'lucide-react'
import { PageHeader } from '@/components/layout/PageHeader'
import { Button } from '@/components/ui/Button'
import { EmptyState } from '@/components/ui/EmptyState'
import { PageSpinner } from '@/components/ui/Spinner'
import { useCurrentUser } from '@/features/auth'
import { getErrorMessage } from '@/lib/http'
import { useCalendarYear } from '../api/calendarApi'
import { indexByDay, todayIso } from '../calendarDates'
import { DayPanel } from '../components/DayPanel'
import { EventFormDialog } from '../components/EventFormDialog'
import { YearGrid } from '../components/YearGrid'
import { emptyEventForm, eventFormFromEntry, type EventFormValues } from '../eventForm'

const DESCRIPTIONS = {
  Admin: 'Plan the Unit’s whole year. Click a day to see everything on it, or add an event, task release or interview drive.',
  Student: 'Unit events for you, plus your lecture halls, deadlines, assigned tasks, interview invitations and vacancy closing dates.',
  Registrar: 'Every Unit event across the year. Allocate halls for them from Hall allocations.',
  Lecturer: 'Unit programmes for students and lecturers across the year.',
  Industry: 'Unit events for partners, plus your company’s vacancy closing dates.',
} as const

interface Editing { eventId: number | null; values: EventFormValues }

export default function CalendarPage() {
  const { data: me } = useCurrentUser()
  const [year, setYear] = useState(() => new Date().getFullYear())
  const [selected, setSelected] = useState<string>(todayIso)
  const [editing, setEditing] = useState<Editing | null>(null)
  const { data: entries, isLoading, isError, error } = useCalendarYear(year)
  const byDay = useMemo(() => indexByDay(entries ?? []), [entries])

  if (!me) return null
  const isAdmin = me.role === 'Admin'

  const changeYear = (next: number) => {
    setYear(next)
    setSelected(`${next}-01-01`)
  }

  return (
    <>
      <PageHeader
        title={`Year calendar ${year}`}
        description={DESCRIPTIONS[me.role]}
        actions={
          <div className="flex items-center gap-2">
            <Button variant="secondary" size="sm" aria-label="Previous year" onClick={() => changeYear(year - 1)}><ChevronLeft className="size-4" /></Button>
            <Button variant="secondary" size="sm" onClick={() => { setYear(new Date().getFullYear()); setSelected(todayIso()) }}>Today</Button>
            <Button variant="secondary" size="sm" aria-label="Next year" onClick={() => changeYear(year + 1)}><ChevronRight className="size-4" /></Button>
          </div>
        }
      />

      {isLoading ? <PageSpinner label="Loading the calendar…" /> : isError ? (
        <EmptyState icon={CalendarDays} title="Calendar could not be loaded" description={getErrorMessage(error)} />
      ) : (
        <div className="grid gap-6 xl:grid-cols-[minmax(0,1fr)_380px]">
          <YearGrid year={year} byDay={byDay} selected={selected} onSelect={setSelected} />
          <div className="xl:sticky xl:top-6 xl:self-start">
            <DayPanel
              day={selected}
              entries={byDay.get(selected) ?? []}
              role={me.role}
              onAdd={isAdmin ? () => setEditing({ eventId: null, values: emptyEventForm(selected) }) : undefined}
              onEdit={isAdmin ? (e) => setEditing({ eventId: e.eventId, values: eventFormFromEntry(e) }) : undefined}
            />
          </div>
        </div>
      )}

      {editing && <EventFormDialog key={editing.eventId ?? 'new'} eventId={editing.eventId} defaultValues={editing.values} onClose={() => setEditing(null)} />}
    </>
  )
}
