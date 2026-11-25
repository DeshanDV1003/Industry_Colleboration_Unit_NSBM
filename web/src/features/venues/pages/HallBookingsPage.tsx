import { useMemo, useState } from 'react'
import { CalendarClock, ChevronLeft, ChevronRight, Plus } from 'lucide-react'
import { PageHeader } from '@/components/layout/PageHeader'
import { Button } from '@/components/ui/Button'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { PageSpinner } from '@/components/ui/Spinner'
import { controlClass } from '@/components/ui/controlClass'
import { cn } from '@/lib/cn'
import { getErrorMessage } from '@/lib/http'
import { useSession } from '@/stores/authStore'
import { useBookingOptions, useVenueBookings, useVenues } from '../api/venuesApi'
import { BookingCard } from '../components/BookingCard'
import { BookingFormDialog } from '../components/BookingFormDialog'
import { addDays, formatDay, formatWeek, mondayOf, todayIso, weekDays } from '../weekDates'

/** A week of hall allocations. The registrar allocates and cancels; Unit admins follow read-only. */
export default function HallBookingsPage() {
  const session = useSession()
  const canManage = session?.role === 'Registrar'
  const today = todayIso()
  const [monday, setMonday] = useState(() => mondayOf(today))
  const [venueId, setVenueId] = useState('')
  const [allocating, setAllocating] = useState(false)

  const days = weekDays(monday)
  const { data: bookings, isLoading, isError, error } = useVenueBookings(days[0], days[6])
  const { data: venues } = useVenues()
  const { data: options } = useBookingOptions(canManage)

  const byDay = useMemo(() => {
    const visible = (bookings ?? []).filter((b) => !venueId || String(b.venueId) === venueId)
    return new Map(days.map((d) => [d, visible.filter((b) => b.date === d)]))
  }, [bookings, venueId, days])

  const eventsWithoutHall = options?.industryEvents.filter((e) => !e.hasHall) ?? []

  return (
    <>
      <PageHeader
        title="Hall allocations"
        description={canManage
          ? 'Allocate lecture halls to cohorts and halls to events. Students, host companies and Unit admins are notified automatically.'
          : 'Every hall allocation made by the Assistant Registrar.'}
        actions={canManage && <Button onClick={() => setAllocating(true)}><Plus className="size-4" /> Allocate hall</Button>}
      />

      {canManage && eventsWithoutHall.length > 0 && (
        <Card className="mb-4 border-warning">
          <CardHeader title="Approved events without a hall" description="Partner events students can already register for." />
          <CardBody>
            <ul className="space-y-1 text-sm">
              {eventsWithoutHall.map((e) => (
                <li key={e.id}><span className="font-medium text-heading">{e.title}</span> <span className="text-muted">— {e.host}, {formatDay(e.startDate)}, up to {e.capacity} people</span></li>
              ))}
            </ul>
          </CardBody>
        </Card>
      )}

      <div className="mb-4 flex flex-wrap items-center gap-2">
        <Button variant="secondary" size="sm" aria-label="Previous week" onClick={() => setMonday(addDays(monday, -7))}><ChevronLeft className="size-4" /></Button>
        <Button variant="secondary" size="sm" onClick={() => setMonday(mondayOf(today))}>This week</Button>
        <Button variant="secondary" size="sm" aria-label="Next week" onClick={() => setMonday(addDays(monday, 7))}><ChevronRight className="size-4" /></Button>
        <p className="ml-1 font-semibold text-heading">{formatWeek(monday)}</p>
        <select aria-label="Filter by venue" className={cn(controlClass(false), 'ml-auto h-9 w-auto')} value={venueId} onChange={(e) => setVenueId(e.target.value)}>
          <option value="">All venues</option>
          {venues?.map((v) => <option key={v.id} value={v.id}>{v.code} · {v.name}</option>)}
        </select>
      </div>

      {isLoading ? <PageSpinner label="Loading allocations…" />
        : isError ? <EmptyState icon={CalendarClock} title="Allocations could not be loaded" description={getErrorMessage(error)} />
        : (
          <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
            {days.map((day) => {
              const list = byDay.get(day) ?? []
              return (
                <Card key={day} className={cn(day === today && 'border-primary')}>
                  <CardHeader title={formatDay(day)} description={list.length === 0 ? 'No bookings' : `${list.length} booking${list.length === 1 ? '' : 's'}`} />
                  <CardBody className="space-y-2">
                    {list.map((b) => <BookingCard key={b.id} booking={b} canManage={canManage} />)}
                  </CardBody>
                </Card>
              )
            })}
          </div>
        )}

      {allocating && <BookingFormDialog initialDate={today} onClose={() => setAllocating(false)} />}
    </>
  )
}
