/** Mirrors Domain.Enums.VenueType. */
export const VENUE_TYPES = ['LectureHall', 'ComputerLab', 'Auditorium', 'EventHall'] as const
export type VenueType = (typeof VENUE_TYPES)[number]

/** Mirrors Domain.Enums.VenueBookingPurpose. */
export const BOOKING_PURPOSES = ['Lecture', 'IndustryEvent', 'UnitEvent'] as const
export type BookingPurpose = (typeof BOOKING_PURPOSES)[number]

/** Mirrors Features/Venues/Dtos/VenueDto. */
export interface Venue {
  id: number
  code: string
  name: string
  building: string
  type: VenueType
  capacity: number
  facilities: string | null
  isActive: boolean
  upcomingBookings: number
}

/** Mirrors Features/Venues/Models/VenueInput. */
export type VenueInput = Omit<Venue, 'id' | 'upcomingBookings'>

/** Mirrors Features/Venues/Dtos/VenueBookingDto. Dates are "yyyy-mm-dd", times "HH:mm:ss". */
export interface VenueBooking {
  id: number
  venueId: number
  venueCode: string
  venueName: string
  purpose: BookingPurpose
  title: string
  date: string
  startTime: string
  endTime: string
  programme: string | null
  academicYear: number | null
  industryEventId: number | null
  calendarEventId: number | null
  attendees: number
  note: string | null
  bookedByName: string
  isPast: boolean
}

/** Mirrors Features/Venues/Models/VenueBookingInput. */
export interface VenueBookingInput {
  venueId: number
  purpose: BookingPurpose
  title: string | null
  date: string
  startTime: string
  endTime: string
  programme: string | null
  academicYear: number | null
  industryEventId: number | null
  calendarEventId: number | null
  expectedAttendees: number | null
  note: string | null
  repeatWeeks: number
}

export interface CohortOption { programme: string; academicYear: number; studentCount: number }

export interface EventOption {
  id: number
  title: string
  host: string | null
  startDate: string
  endDate: string
  capacity: number | null
  hasHall: boolean
}

/** Mirrors Features/Venues/Dtos/VenueBookingOptionsDto. */
export interface VenueBookingOptions {
  cohorts: CohortOption[]
  industryEvents: EventOption[]
  unitEvents: EventOption[]
}
