import { z } from 'zod'
import { DAY_ENDS, DAY_STARTS, MAX_REPEAT_WEEKS, parseCohortKey } from './config'
import { BOOKING_PURPOSES, type VenueBookingInput } from './types'

/** Select and number inputs are held as strings ('' = none) and converted on submit. */
export const bookingFormSchema = z.object({
  purpose: z.enum(BOOKING_PURPOSES),
  venueId: z.string().min(1, 'Choose a venue.'),
  title: z.string().max(150),
  cohort: z.string(),
  industryEventId: z.string(),
  calendarEventId: z.string(),
  date: z.string().min(1, 'Choose a date.'),
  startTime: z.string().min(1, 'Choose a start time.'),
  endTime: z.string().min(1, 'Choose an end time.'),
  expectedAttendees: z.string().regex(/^\d*$/, 'Enter a whole number.'),
  repeatWeeks: z.string().regex(/^\d+$/, 'Enter a whole number.')
    .refine((w) => Number(w) >= 1 && Number(w) <= MAX_REPEAT_WEEKS, `Repeat for 1 to ${MAX_REPEAT_WEEKS} weeks.`),
  note: z.string().max(500),
})
  .refine((v) => v.endTime > v.startTime, { path: ['endTime'], message: 'The end time must be after the start time.' })
  .refine((v) => v.startTime >= DAY_STARTS && v.endTime <= DAY_ENDS, { path: ['startTime'], message: `Book between ${DAY_STARTS} and ${DAY_ENDS}.` })
  .refine((v) => v.purpose !== 'Lecture' || v.title.trim() !== '', { path: ['title'], message: 'Enter the module or lecture name.' })
  .refine((v) => v.purpose !== 'Lecture' || v.cohort !== '', { path: ['cohort'], message: 'Choose the cohort.' })
  .refine((v) => v.purpose !== 'IndustryEvent' || v.industryEventId !== '', { path: ['industryEventId'], message: 'Choose the partner event.' })
  .refine((v) => v.purpose !== 'UnitEvent' || v.calendarEventId !== '', { path: ['calendarEventId'], message: 'Choose the calendar event.' })
  .refine((v) => v.purpose !== 'UnitEvent' || v.expectedAttendees !== '', { path: ['expectedAttendees'], message: 'Enter how many people are expected.' })

export type BookingFormValues = z.infer<typeof bookingFormSchema>

export const emptyBookingForm = (date: string): BookingFormValues => ({
  purpose: 'Lecture', venueId: '', title: '', cohort: '', industryEventId: '', calendarEventId: '',
  date, startTime: '09:00', endTime: '11:00', expectedAttendees: '', repeatWeeks: '1', note: '',
})

const seconds = (time: string) => (time.length === 5 ? `${time}:00` : time)

/** Only the fields that belong to the chosen purpose are sent; the rest go as null. */
export function toBookingInput(v: BookingFormValues): VenueBookingInput {
  const cohort = v.purpose === 'Lecture' ? parseCohortKey(v.cohort) : null
  return {
    venueId: Number(v.venueId),
    purpose: v.purpose,
    title: v.title.trim() || null,
    date: v.date,
    startTime: seconds(v.startTime),
    endTime: seconds(v.endTime),
    programme: cohort?.programme ?? null,
    academicYear: cohort?.academicYear ?? null,
    industryEventId: v.purpose === 'IndustryEvent' ? Number(v.industryEventId) : null,
    calendarEventId: v.purpose === 'UnitEvent' ? Number(v.calendarEventId) : null,
    expectedAttendees: v.expectedAttendees ? Number(v.expectedAttendees) : null,
    note: v.note.trim() || null,
    repeatWeeks: v.purpose === 'Lecture' ? Number(v.repeatWeeks) : 1,
  }
}
