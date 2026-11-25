import { useForm, useWatch } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { toast } from '@/components/feedback/toast'
import { Button } from '@/components/ui/Button'
import { Dialog } from '@/components/ui/Dialog'
import { FormField } from '@/components/ui/FormField'
import { SegmentedControl } from '@/components/ui/SegmentedControl'
import { Spinner } from '@/components/ui/Spinner'
import { controlClass } from '@/components/ui/controlClass'
import { cn } from '@/lib/cn'
import { applyServerErrors } from '@/lib/formErrors'
import { getErrorMessage } from '@/lib/http'
import { useBookVenue, useBookingOptions, useVenues } from '../api/venuesApi'
import { bookingFormSchema, emptyBookingForm, toBookingInput, type BookingFormValues } from '../bookingForm'
import { MAX_REPEAT_WEEKS, PURPOSE_META, VENUE_TYPE_LABEL, cohortKey, cohortLabel } from '../config'
import { BOOKING_PURPOSES, type EventOption, type Venue, type VenueBookingOptions } from '../types'
import { formatDay } from '../weekDates'

interface BookingFormDialogProps {
  initialDate: string
  onClose: () => void
}

/** The registrar allocates a hall. Mounts the form only once venues and options are loaded. */
export function BookingFormDialog({ initialDate, onClose }: BookingFormDialogProps) {
  const venues = useVenues()
  const options = useBookingOptions()

  if (!venues.data || !options.data) {
    const error = venues.error ?? options.error
    return (
      <Dialog open onClose={onClose} title="Allocate a hall">
        {error ? <p className="text-sm text-danger">{getErrorMessage(error)}</p> : <Spinner />}
      </Dialog>
    )
  }
  return <BookingForm initialDate={initialDate} onClose={onClose} venues={venues.data.filter((v) => v.isActive)} options={options.data} />
}

const eventLabel = (e: EventOption) =>
  `${e.title}${e.host ? ` — ${e.host}` : ''} (${formatDay(e.startDate)})${e.hasHall ? ' · has a hall' : ''}`

function BookingForm({ initialDate, onClose, venues, options }: BookingFormDialogProps & { venues: Venue[]; options: VenueBookingOptions }) {
  const book = useBookVenue()
  const { register, control, handleSubmit, setError, setValue, formState: { errors } } = useForm<BookingFormValues>({
    resolver: zodResolver(bookingFormSchema),
    defaultValues: emptyBookingForm(initialDate),
  })
  const [purpose, industryEventId, calendarEventId] = useWatch({ control, name: ['purpose', 'industryEventId', 'calendarEventId'] })
  const input = (invalid: boolean) => cn(controlClass(invalid), 'h-10')

  const chosenEvent = purpose === 'IndustryEvent'
    ? options.industryEvents.find((e) => String(e.id) === industryEventId)
    : purpose === 'UnitEvent' ? options.unitEvents.find((e) => String(e.id) === calendarEventId) : undefined

  /** Picking an event moves the date to its first day, which is almost always the day being booked. */
  const pickEvent = (field: 'industryEventId' | 'calendarEventId', list: EventOption[]) => (id: string) => {
    setValue(field, id, { shouldValidate: true })
    const event = list.find((e) => String(e.id) === id)
    if (event) setValue('date', event.startDate < initialDate ? initialDate : event.startDate)
  }

  const submit = handleSubmit((values) => {
    book.mutate(toBookingInput(values), {
      onSuccess: ({ ids }) => {
        toast.success(ids.length > 1 ? `Hall allocated for ${ids.length} weeks. Everyone affected was notified.` : 'Hall allocated. Everyone affected was notified.')
        onClose()
      },
      onError: (e) => { if (!applyServerErrors(e, setError, 'Booking.')) toast.error(getErrorMessage(e)) },
    })
  })

  return (
    <Dialog
      open
      onClose={onClose}
      title="Allocate a hall"
      description="Capacity and clashes are checked before anything is saved."
      footer={<>
        <Button variant="secondary" onClick={onClose}>Cancel</Button>
        <Button loading={book.isPending} onClick={submit}>Allocate hall</Button>
      </>}
    >
      <form noValidate onSubmit={submit} className="grid gap-4 md:grid-cols-2">
        <div className="md:col-span-2">
          <SegmentedControl ariaLabel="What the hall is for" value={purpose}
            onChange={(p) => setValue('purpose', p, { shouldValidate: false })}
            options={BOOKING_PURPOSES.map((p) => ({ value: p, label: PURPOSE_META[p].label }))} />
        </div>

        {purpose === 'Lecture' && (<>
          <FormField label="Module or lecture" htmlFor="bk-title" error={errors.title?.message}>
            <input id="bk-title" className={input(!!errors.title)} placeholder="Software Architecture" {...register('title')} />
          </FormField>
          <FormField label="Cohort" htmlFor="bk-cohort" error={errors.cohort?.message}>
            <select id="bk-cohort" className={input(!!errors.cohort)} {...register('cohort')}>
              <option value="">Choose a cohort…</option>
              {options.cohorts.map((c) => (
                <option key={cohortKey(c)} value={cohortKey(c)}>{cohortLabel(c.programme, c.academicYear)} ({c.studentCount} student{c.studentCount === 1 ? '' : 's'})</option>
              ))}
            </select>
          </FormField>
        </>)}

        {purpose === 'IndustryEvent' && (
          <FormField label="Partner event" htmlFor="bk-event" error={errors.industryEventId?.message} className="md:col-span-2"
            hint={options.industryEvents.length === 0 ? 'No upcoming approved partner events.' : undefined}>
            <select id="bk-event" className={input(!!errors.industryEventId)} value={industryEventId}
              onChange={(e) => pickEvent('industryEventId', options.industryEvents)(e.target.value)}>
              <option value="">Choose an event…</option>
              {options.industryEvents.map((e) => <option key={e.id} value={e.id}>{eventLabel(e)}</option>)}
            </select>
          </FormField>
        )}

        {purpose === 'UnitEvent' && (
          <FormField label="Calendar event" htmlFor="bk-unit" error={errors.calendarEventId?.message} className="md:col-span-2">
            <select id="bk-unit" className={input(!!errors.calendarEventId)} value={calendarEventId}
              onChange={(e) => pickEvent('calendarEventId', options.unitEvents)(e.target.value)}>
              <option value="">Choose an event…</option>
              {options.unitEvents.map((e) => <option key={e.id} value={e.id}>{eventLabel(e)}</option>)}
            </select>
          </FormField>
        )}

        {purpose !== 'Lecture' && (
          <FormField label="Title on the booking (optional)" htmlFor="bk-etitle" hint="Leave empty to use the event's title." error={errors.title?.message} className="md:col-span-2">
            <input id="bk-etitle" className={input(!!errors.title)} {...register('title')} />
          </FormField>
        )}

        <FormField label="Venue" htmlFor="bk-venue" error={errors.venueId?.message} className="md:col-span-2">
          <select id="bk-venue" className={input(!!errors.venueId)} {...register('venueId')}>
            <option value="">Choose a venue…</option>
            {venues.map((v) => <option key={v.id} value={v.id}>{v.code} · {v.name} — {VENUE_TYPE_LABEL[v.type]}, seats {v.capacity}</option>)}
          </select>
        </FormField>

        <FormField label="Date" htmlFor="bk-date" error={errors.date?.message}
          hint={chosenEvent ? `The event runs ${formatDay(chosenEvent.startDate)} – ${formatDay(chosenEvent.endDate)}.` : undefined}>
          <input id="bk-date" type="date" min={initialDate} className={input(!!errors.date)} {...register('date')} />
        </FormField>
        <div className="grid grid-cols-2 gap-3">
          <FormField label="From" htmlFor="bk-start" error={errors.startTime?.message}>
            <input id="bk-start" type="time" step={900} className={input(!!errors.startTime)} {...register('startTime')} />
          </FormField>
          <FormField label="To" htmlFor="bk-end" error={errors.endTime?.message}>
            <input id="bk-end" type="time" step={900} className={input(!!errors.endTime)} {...register('endTime')} />
          </FormField>
        </div>

        <FormField label={purpose === 'UnitEvent' ? 'Expected attendees' : 'Expected attendees (optional)'} htmlFor="bk-att"
          hint={purpose === 'UnitEvent' ? undefined : purpose === 'Lecture' ? 'Defaults to the cohort size.' : 'Defaults to the event capacity.'}
          error={errors.expectedAttendees?.message}>
          <input id="bk-att" inputMode="numeric" className={input(!!errors.expectedAttendees)} {...register('expectedAttendees')} />
        </FormField>
        {purpose === 'Lecture' && (
          <FormField label="Repeat weekly for" htmlFor="bk-repeat" hint={`1 to ${MAX_REPEAT_WEEKS} weeks.`} error={errors.repeatWeeks?.message}>
            <input id="bk-repeat" type="number" min={1} max={MAX_REPEAT_WEEKS} className={input(!!errors.repeatWeeks)} {...register('repeatWeeks')} />
          </FormField>
        )}

        <FormField label="Note (optional)" htmlFor="bk-note" error={errors.note?.message} className="md:col-span-2">
          <textarea id="bk-note" rows={2} className={cn(controlClass(!!errors.note), 'py-2')} {...register('note')} />
        </FormField>
      </form>
    </Dialog>
  )
}
