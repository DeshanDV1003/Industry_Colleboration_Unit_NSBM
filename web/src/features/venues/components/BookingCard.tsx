import { useState } from 'react'
import { Clock, DoorOpen, Users, X } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { Badge } from '@/components/ui/Badge'
import { Button } from '@/components/ui/Button'
import { cn } from '@/lib/cn'
import { getErrorMessage } from '@/lib/http'
import { useCancelBooking } from '../api/venuesApi'
import { PURPOSE_META, clock, cohortLabel } from '../config'
import type { VenueBooking } from '../types'

/** One allocation. The registrar can cancel an upcoming one after confirming. */
export function BookingCard({ booking, canManage }: { booking: VenueBooking; canManage: boolean }) {
  const cancel = useCancelBooking()
  const [confirming, setConfirming] = useState(false)
  const meta = PURPOSE_META[booking.purpose]

  const confirmCancel = () => cancel.mutate(booking.id, {
    onSuccess: () => toast.success('Booking cancelled. Everyone affected was notified.'),
    onError: (e) => { toast.error(getErrorMessage(e)); setConfirming(false) },
  })

  return (
    <div className={cn('rounded-xl border border-line p-3 text-sm', booking.isPast && 'opacity-70')}>
      <div className="flex flex-wrap items-start justify-between gap-2">
        <div className="min-w-0">
          <p className="font-semibold text-heading">{booking.title}</p>
          <p className="mt-0.5 flex flex-wrap items-center gap-x-3 gap-y-1 text-xs text-muted">
            <span className="flex items-center gap-1"><Clock className="size-3.5" /> {clock(booking.startTime)}–{clock(booking.endTime)}</span>
            <span className="flex items-center gap-1"><DoorOpen className="size-3.5" /> {booking.venueCode} · {booking.venueName}</span>
            <span className="flex items-center gap-1"><Users className="size-3.5" /> {booking.attendees}</span>
          </p>
        </div>
        <Badge tone={meta.tone}>{meta.label}</Badge>
      </div>
      {booking.programme && booking.academicYear && (
        <p className="mt-2 text-xs text-body">{cohortLabel(booking.programme, booking.academicYear)}</p>
      )}
      {booking.note && <p className="mt-1 text-xs text-muted">{booking.note}</p>}
      <div className="mt-2 flex items-center justify-between gap-2 text-xs text-muted">
        <span>Allocated by {booking.bookedByName}</span>
        {canManage && !booking.isPast && (confirming ? (
          <span className="flex gap-1">
            <Button size="sm" variant="ghost" onClick={() => setConfirming(false)}>Keep</Button>
            <Button size="sm" variant="danger" loading={cancel.isPending} onClick={confirmCancel}>Confirm cancel</Button>
          </span>
        ) : (
          <Button size="sm" variant="ghost" className="text-danger" onClick={() => setConfirming(true)}><X className="size-4" /> Cancel</Button>
        ))}
      </div>
    </div>
  )
}
