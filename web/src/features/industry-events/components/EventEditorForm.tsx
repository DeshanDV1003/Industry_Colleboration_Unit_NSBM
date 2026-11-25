import { useForm, useWatch, type UseFormSetError } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { Button } from '@/components/ui/Button'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { FormField } from '@/components/ui/FormField'
import { controlClass } from '@/components/ui/controlClass'
import { EVENT_FORMATS, EVENT_FORMAT_META } from '@/config/events'
import { cn } from '@/lib/cn'
import { eventFormSchema, type EventFormValues } from '../eventForm'

interface EventEditorFormProps {
  defaultValues: EventFormValues
  submitLabel: string
  submitting: boolean
  onSubmit: (values: EventFormValues, setError: UseFormSetError<EventFormValues>) => void
  onCancel: () => void
}

export function EventEditorForm({ defaultValues, submitLabel, submitting, onSubmit, onCancel }: EventEditorFormProps) {
  const { register, control, handleSubmit, setError, formState: { errors } } = useForm<EventFormValues>({
    resolver: zodResolver(eventFormSchema),
    defaultValues,
  })
  const isTeamEvent = useWatch({ control, name: 'isTeamEvent' })
  const input = (invalid: boolean) => cn(controlClass(invalid), 'h-10')

  return (
    <form noValidate onSubmit={handleSubmit((values) => onSubmit(values, setError))} className="max-w-4xl space-y-6">
      <Card>
        <CardHeader title="About the event" description="What students will do and learn." />
        <CardBody className="grid gap-4 md:grid-cols-2">
          <FormField label="Title" htmlFor="title" error={errors.title?.message} className="md:col-span-2">
            <input id="title" className={input(!!errors.title)} placeholder="Green Hack 48: build for sustainability" {...register('title')} />
          </FormField>
          <FormField label="Format" htmlFor="format" error={errors.format?.message}>
            <select id="format" className={input(!!errors.format)} {...register('format')}>
              {EVENT_FORMATS.map((f) => <option key={f} value={f}>{EVENT_FORMAT_META[f].label}</option>)}
            </select>
          </FormField>
          <FormField label="Location" htmlFor="location" error={errors.location?.message}>
            <input id="location" className={input(!!errors.location)} placeholder="NSBM Innovation Hub, or Online" {...register('location')} />
          </FormField>
          <FormField label="Description" htmlFor="description" error={errors.description?.message} className="md:col-span-2">
            <textarea id="description" rows={5} className={cn(controlClass(!!errors.description), 'py-2')} {...register('description')} />
          </FormField>
        </CardBody>
      </Card>

      <Card>
        <CardHeader title="Dates and places" description="Registration must close on or before the start date." />
        <CardBody className="grid gap-4 md:grid-cols-2">
          <FormField label="Start date" htmlFor="startDate" error={errors.startDate?.message}>
            <input id="startDate" type="date" className={input(!!errors.startDate)} {...register('startDate')} />
          </FormField>
          <FormField label="End date" htmlFor="endDate" error={errors.endDate?.message}>
            <input id="endDate" type="date" className={input(!!errors.endDate)} {...register('endDate')} />
          </FormField>
          <FormField label="Registration closes" htmlFor="registrationClosesOn" error={errors.registrationClosesOn?.message}>
            <input id="registrationClosesOn" type="date" className={input(!!errors.registrationClosesOn)} {...register('registrationClosesOn')} />
          </FormField>
          <FormField label="Places" htmlFor="capacity" error={errors.capacity?.message}>
            <input id="capacity" type="number" min={1} max={1000} className={input(!!errors.capacity)} {...register('capacity', { valueAsNumber: true })} />
          </FormField>
          <label className="flex items-center gap-2 text-sm font-medium text-heading md:col-span-2">
            <input type="checkbox" className="size-4 accent-[var(--color-primary)]" {...register('isTeamEvent')} /> Students take part in teams
          </label>
          {isTeamEvent && (
            <>
              <FormField label="Smallest team" htmlFor="minTeamSize" error={errors.minTeamSize?.message}>
                <input id="minTeamSize" type="number" min={1} max={10} className={input(!!errors.minTeamSize)} {...register('minTeamSize', { valueAsNumber: true })} />
              </FormField>
              <FormField label="Largest team" htmlFor="maxTeamSize" error={errors.maxTeamSize?.message}>
                <input id="maxTeamSize" type="number" min={2} max={10} className={input(!!errors.maxTeamSize)} {...register('maxTeamSize', { valueAsNumber: true })} />
              </FormField>
            </>
          )}
        </CardBody>
      </Card>

      <div className="flex justify-end gap-2">
        <Button type="button" variant="secondary" onClick={onCancel}>Cancel</Button>
        <Button type="submit" loading={submitting}>{submitLabel}</Button>
      </div>
    </form>
  )
}
