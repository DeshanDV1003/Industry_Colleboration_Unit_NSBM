/** Mirrors Domain.Enums.CalendarEventType. */
export const CALENDAR_EVENT_TYPES = [
  'Programme', 'Workshop', 'IndustryTalk', 'Hackathon', 'TaskRelease',
  'CareerPlanMilestone', 'InterviewDrive', 'ApplicationWindow', 'CareerFair',
] as const
export type CalendarEventType = (typeof CALENDAR_EVENT_TYPES)[number]

/** Mirrors Domain.Enums.CalendarAudience. */
export const CALENDAR_AUDIENCES = ['Everyone', 'Students', 'Lecturers', 'Partners'] as const
export type CalendarAudience = (typeof CALENDAR_AUDIENCES)[number]

/** Mirrors Features/Calendar/Models/CalendarEntrySource. */
export type CalendarEntrySource = 'UnitEvent' | 'PlannerDeadline' | 'VacancyClosing' | 'AssignedTask' | 'InterviewInvite' | 'IndustryEvent' | 'HallBooking'

/** Mirrors Features/Calendar/Dtos/CalendarEntryDto. Dates are ISO "yyyy-mm-dd". */
export interface CalendarEntry {
  key: string
  source: CalendarEntrySource
  title: string
  description: string | null
  startDate: string
  endDate: string
  eventId: number | null
  eventType: CalendarEventType | null
  audience: CalendarAudience | null
  location: string | null
  careerPathId: number | null
  careerPathName: string | null
  industryTaskId: number | null
  vacancyId: number | null
  industryEventId: number | null
}

/** Mirrors Features/Calendar/Models/CalendarEventInput. */
export interface CalendarEventInput {
  title: string
  description: string | null
  type: CalendarEventType
  audience: CalendarAudience
  startDate: string
  endDate: string
  location: string | null
  careerPathId: number | null
  industryTaskId: number | null
  vacancyId: number | null
}

export interface CalendarOption { id: number; name: string }

export interface CalendarOptions {
  careerPaths: CalendarOption[]
  tasks: CalendarOption[]
  vacancies: CalendarOption[]
}
