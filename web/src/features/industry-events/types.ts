import type { EventFormat } from '@/config/events'
import type { ModerationStatus } from '@/types/tasks'

/** Mirrors Features/IndustryEvents/Dtos/IndustryEventListItemDto. */
export interface IndustryEventListItem {
  id: number
  title: string
  companyName: string
  format: EventFormat
  startDate: string
  endDate: string
  capacity: number
  registeredCount: number
  isTeamEvent: boolean
  status: ModerationStatus
  createdAt: string
  reviewedAt: string | null
  reviewNote: string | null
  canBeEdited: boolean
}

/** Mirrors Features/IndustryEvents/Dtos/IndustryEventDto. */
export interface IndustryEvent {
  id: number
  title: string
  description: string
  companyName: string
  format: EventFormat
  startDate: string
  endDate: string
  location: string
  capacity: number
  registrationClosesOn: string
  minTeamSize: number | null
  maxTeamSize: number | null
  registeredCount: number
  status: ModerationStatus
  reviewedBy: string | null
  reviewNote: string | null
  canBeEdited: boolean
  canBeReviewed: boolean
}

/** Mirrors Features/IndustryEvents/Models/IndustryEventInput. */
export interface IndustryEventInput {
  title: string
  description: string
  format: EventFormat
  startDate: string
  endDate: string
  location: string
  capacity: number
  registrationClosesOn: string
  minTeamSize: number | null
  maxTeamSize: number | null
}

export interface EventAttendee {
  registrationId: number
  studentName: string
  studentNo: string
  programme: string
  teamName: string | null
  registeredAt: string
  attended: boolean
  certificateCode: string | null
}

export interface EventAttendance {
  eventId: number
  title: string
  startDate: string
  endDate: string
  hasStarted: boolean
  hasEnded: boolean
  attendees: EventAttendee[]
}
