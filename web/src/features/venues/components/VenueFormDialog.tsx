import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { toast } from '@/components/feedback/toast'
import { Button } from '@/components/ui/Button'
import { Dialog } from '@/components/ui/Dialog'
import { FormField } from '@/components/ui/FormField'
import { controlClass } from '@/components/ui/controlClass'
import { cn } from '@/lib/cn'
import { applyServerErrors } from '@/lib/formErrors'
import { getErrorMessage } from '@/lib/http'
import { useCreateVenue, useUpdateVenue } from '../api/venuesApi'
import { VENUE_TYPE_LABEL } from '../config'
import { VENUE_TYPES, type Venue, type VenueInput } from '../types'

const venueSchema = z.object({
  code: z.string().trim().min(1, 'Enter a code.').max(20),
  name: z.string().trim().min(1, 'Enter a name.').max(120),
  building: z.string().trim().min(1, 'Enter the building.').max(120),
  type: z.enum(VENUE_TYPES),
  capacity: z.string().regex(/^\d+$/, 'Enter a whole number.').refine((c) => Number(c) >= 1 && Number(c) <= 2000, 'Seats must be 1 to 2000.'),
  facilities: z.string().max(300),
  isActive: z.boolean(),
})
type VenueFormValues = z.infer<typeof venueSchema>

const toValues = (v: Venue | null): VenueFormValues => ({
  code: v?.code ?? '', name: v?.name ?? '', building: v?.building ?? '', type: v?.type ?? 'LectureHall',
  capacity: v ? String(v.capacity) : '', facilities: v?.facilities ?? '', isActive: v?.isActive ?? true,
})

const toInput = (v: VenueFormValues): VenueInput => ({
  code: v.code, name: v.name, building: v.building, type: v.type, capacity: Number(v.capacity),
  facilities: v.facilities.trim() || null, isActive: v.isActive,
})

/** Adds a venue, or edits one (including closing it for bookings). */
export function VenueFormDialog({ venue, onClose }: { venue: Venue | null; onClose: () => void }) {
  const create = useCreateVenue()
  const update = useUpdateVenue()
  const { register, handleSubmit, setError, formState: { errors } } = useForm<VenueFormValues>({
    resolver: zodResolver(venueSchema),
    defaultValues: toValues(venue),
  })
  const input = (invalid: boolean) => cn(controlClass(invalid), 'h-10')

  const submit = handleSubmit((values) => {
    const callbacks = {
      onSuccess: () => { toast.success(venue ? 'Venue updated.' : 'Venue added.'); onClose() },
      onError: (e: unknown) => { if (!applyServerErrors(e, setError, 'Venue.')) toast.error(getErrorMessage(e)) },
    }
    if (venue) update.mutate({ id: venue.id, input: toInput(values) }, callbacks)
    else create.mutate(toInput(values), callbacks)
  })

  return (
    <Dialog
      open
      onClose={onClose}
      title={venue ? `Edit ${venue.code}` : 'Add a venue'}
      footer={<>
        <Button variant="secondary" onClick={onClose}>Cancel</Button>
        <Button loading={create.isPending || update.isPending} onClick={submit}>{venue ? 'Save changes' : 'Add venue'}</Button>
      </>}
    >
      <form noValidate onSubmit={submit} className="grid gap-4 md:grid-cols-2">
        <FormField label="Code" htmlFor="vn-code" error={errors.code?.message}>
          <input id="vn-code" className={input(!!errors.code)} placeholder="LH-103" {...register('code')} />
        </FormField>
        <FormField label="Type" htmlFor="vn-type" error={errors.type?.message}>
          <select id="vn-type" className={input(!!errors.type)} {...register('type')}>
            {VENUE_TYPES.map((t) => <option key={t} value={t}>{VENUE_TYPE_LABEL[t]}</option>)}
          </select>
        </FormField>
        <FormField label="Name" htmlFor="vn-name" error={errors.name?.message}>
          <input id="vn-name" className={input(!!errors.name)} {...register('name')} />
        </FormField>
        <FormField label="Building" htmlFor="vn-building" error={errors.building?.message}>
          <input id="vn-building" className={input(!!errors.building)} {...register('building')} />
        </FormField>
        <FormField label="Seats" htmlFor="vn-capacity" error={errors.capacity?.message}>
          <input id="vn-capacity" inputMode="numeric" className={input(!!errors.capacity)} {...register('capacity')} />
        </FormField>
        <FormField label="Facilities (optional)" htmlFor="vn-facilities" error={errors.facilities?.message}>
          <input id="vn-facilities" className={input(!!errors.facilities)} placeholder="Projector, microphones" {...register('facilities')} />
        </FormField>
        <label className="flex items-center gap-2 text-sm text-heading md:col-span-2">
          <input type="checkbox" className="size-4 accent-[var(--color-primary)]" {...register('isActive')} />
          Taking bookings (untick while the venue is closed or under maintenance)
        </label>
      </form>
    </Dialog>
  )
}
