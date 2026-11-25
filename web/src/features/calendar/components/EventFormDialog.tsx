import { useState } from 'react'
import { useForm, useWatch } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { Trash2 } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { Button } from '@/components/ui/Button'
import { Dialog } from '@/components/ui/Dialog'
import { Spinner } from '@/components/ui/Spinner'
import { FormField } from '@/components/ui/FormField'
import { controlClass } from '@/components/ui/controlClass'
import { cn } from '@/lib/cn'
import { applyServerErrors } from '@/lib/formErrors'
import { getErrorMessage } from '@/lib/http'
import { useCalendarOptions, useCreateCalendarEvent, useDeleteCalendarEvent, useUpdateCalendarEvent } from '../api/calendarApi'
import { AUDIENCE_LABEL, EVENT_TYPE_META } from '../config'
import { eventFormSchema, linksFor, toEventInput, type EventFormValues } from '../eventForm'
import { CALENDAR_AUDIENCES, CALENDAR_EVENT_TYPES, type CalendarOptions } from '../types'

interface EventFormDialogProps {
  /** The event being edited, or null to create one. */
  eventId: number | null
  defaultValues: EventFormValues
  onClose: () => void
}

/**
 * Admin form to plan an event, release a task to a cohort, or schedule an interview drive.
 * The form mounts only after the link options load, so an edited event's selects keep their values.
 */
export function EventFormDialog(props: EventFormDialogProps) {
  const { data: options, isError, error } = useCalendarOptions(true)

  if (!options) {
    return (
      <Dialog open onClose={props.onClose} title={props.eventId ? 'Edit calendar event' : 'Add calendar event'}>
        {isError ? <p className="text-sm text-danger">{getErrorMessage(error)}</p> : <Spinner />}
      </Dialog>
    )
  }
  return <EventForm {...props} options={options} />
}

function EventForm({ eventId, defaultValues, onClose, options }: EventFormDialogProps & { options: CalendarOptions }) {
  const create = useCreateCalendarEvent()
  const update = useUpdateCalendarEvent()
  const remove = useDeleteCalendarEvent()
  const [confirmDelete, setConfirmDelete] = useState(false)

  const { register, control, handleSubmit, setError, formState: { errors } } = useForm<EventFormValues>({
    resolver: zodResolver(eventFormSchema),
    defaultValues,
  })
  const [type, audience] = useWatch({ control, name: ['type', 'audience'] })
  const links = linksFor({ type, audience })
  const input = (invalid: boolean) => cn(controlClass(invalid), 'h-10')

  const submit = handleSubmit((values) => {
    const body = toEventInput(values)
    const callbacks = {
      onSuccess: () => { toast.success(eventId ? 'Event updated.' : 'Event added to the calendar.'); onClose() },
      onError: (e: unknown) => { if (!applyServerErrors(e, setError, 'Event.')) toast.error(getErrorMessage(e)) },
    }
    if (eventId) update.mutate({ id: eventId, input: body }, callbacks)
    else create.mutate(body, callbacks)
  })

  const deleteEvent = () => eventId && remove.mutate(eventId, {
    onSuccess: () => { toast.success('Event removed.'); onClose() },
    onError: (e) => toast.error(getErrorMessage(e)),
  })

  return (
    <Dialog
      open
      onClose={onClose}
      title={eventId ? 'Edit calendar event' : 'Add calendar event'}
      description="Plan a programme, release a task to a cohort, or schedule an interview drive."
      footer={<>
        {eventId && (confirmDelete
          ? <Button variant="danger" className="mr-auto" loading={remove.isPending} onClick={deleteEvent}>Confirm remove</Button>
          : <Button variant="ghost" className="mr-auto text-danger" onClick={() => setConfirmDelete(true)}><Trash2 className="size-4" /> Remove</Button>)}
        <Button variant="secondary" onClick={onClose}>Cancel</Button>
        <Button loading={create.isPending || update.isPending} onClick={submit}>{eventId ? 'Save changes' : 'Add event'}</Button>
      </>}
    >
      <form noValidate onSubmit={submit} className="grid gap-4 md:grid-cols-2">
        <FormField label="Title" htmlFor="ev-title" error={errors.title?.message} className="md:col-span-2">
          <input id="ev-title" className={input(!!errors.title)} {...register('title')} />
        </FormField>
        <FormField label="Type" htmlFor="ev-type" error={errors.type?.message}>
          <select id="ev-type" className={input(!!errors.type)} {...register('type')}>
            {CALENDAR_EVENT_TYPES.map((t) => <option key={t} value={t}>{EVENT_TYPE_META[t].label}</option>)}
          </select>
        </FormField>
        <FormField label="Who it is for" htmlFor="ev-audience" error={errors.audience?.message}>
          <select id="ev-audience" className={input(!!errors.audience)} {...register('audience')}>
            {CALENDAR_AUDIENCES.map((a) => <option key={a} value={a}>{AUDIENCE_LABEL[a]}</option>)}
          </select>
        </FormField>
        <FormField label="Start date" htmlFor="ev-start" error={errors.startDate?.message}>
          <input id="ev-start" type="date" className={input(!!errors.startDate)} {...register('startDate')} />
        </FormField>
        <FormField label="End date" htmlFor="ev-end" error={errors.endDate?.message}>
          <input id="ev-end" type="date" className={input(!!errors.endDate)} {...register('endDate')} />
        </FormField>
        {links.careerPath && (
          <FormField label="Career path" htmlFor="ev-path" hint="Leave empty for all students." error={errors.careerPathId?.message}>
            <select id="ev-path" className={input(!!errors.careerPathId)} {...register('careerPathId')}>
              <option value="">All career paths</option>
              {options.careerPaths.map((p) => <option key={p.id} value={p.id}>{p.name}</option>)}
            </select>
          </FormField>
        )}
        {links.task && (
          <FormField label="Industry task released" htmlFor="ev-task" error={errors.industryTaskId?.message}>
            <select id="ev-task" className={input(!!errors.industryTaskId)} {...register('industryTaskId')}>
              <option value="">Choose a task…</option>
              {options.tasks.map((t) => <option key={t.id} value={t.id}>{t.name}</option>)}
            </select>
          </FormField>
        )}
        {links.vacancy && (
          <FormField label="Vacancy (optional)" htmlFor="ev-vacancy" error={errors.vacancyId?.message}>
            <select id="ev-vacancy" className={input(!!errors.vacancyId)} {...register('vacancyId')}>
              <option value="">No specific vacancy</option>
              {options.vacancies.map((v) => <option key={v.id} value={v.id}>{v.name}</option>)}
            </select>
          </FormField>
        )}
        <FormField label="Location" htmlFor="ev-location" error={errors.location?.message} className="md:col-span-2">
          <input id="ev-location" className={input(!!errors.location)} placeholder="NSBM Main Auditorium, or Online" {...register('location')} />
        </FormField>
        <FormField label="Description" htmlFor="ev-desc" error={errors.description?.message} className="md:col-span-2">
          <textarea id="ev-desc" rows={3} className={cn(controlClass(!!errors.description), 'py-2')} {...register('description')} />
        </FormField>
      </form>
    </Dialog>
  )
}
