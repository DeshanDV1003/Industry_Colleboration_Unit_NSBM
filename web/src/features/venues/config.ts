import type { BadgeTone } from '@/components/ui/Badge'
import type { BookingPurpose, CohortOption, VenueType } from './types'

export const VENUE_TYPE_LABEL: Record<VenueType, string> = {
  LectureHall: 'Lecture hall',
  ComputerLab: 'Computer lab',
  Auditorium: 'Auditorium',
  EventHall: 'Event hall',
}

export const PURPOSE_META: Record<BookingPurpose, { label: string; tone: BadgeTone }> = {
  Lecture: { label: 'Lecture', tone: 'primary' },
  IndustryEvent: { label: 'Partner event', tone: 'success' },
  UnitEvent: { label: 'Unit event', tone: 'accent' },
}

/** Mirrors VenueBookingInputValidator.MaxRepeatWeeks and VenueBookingRules teaching hours on the API. */
export const MAX_REPEAT_WEEKS = 15
export const DAY_STARTS = '07:00'
export const DAY_ENDS = '21:00'

/** "09:00:00" → "09:00". */
export const clock = (time: string) => time.slice(0, 5)

export const cohortLabel = (programme: string, academicYear: number) => `${programme} · Year ${academicYear}`

/** A cohort is one <select> value: "programme|year". */
export const cohortKey = (c: Pick<CohortOption, 'programme' | 'academicYear'>) => `${c.programme}|${c.academicYear}`

export function parseCohortKey(key: string): { programme: string; academicYear: number } | null {
  const at = key.lastIndexOf('|')
  return at < 0 ? null : { programme: key.slice(0, at), academicYear: Number(key.slice(at + 1)) }
}
