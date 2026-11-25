import type { EventFormat } from '@/config/events'

/** Mirrors Features/EventHub/Dtos/EventCardDto. */
export interface EventCard {
  id: number
  title: string
  companyName: string
  format: EventFormat
  startDate: string
  endDate: string
  location: string
  capacity: number
  registeredCount: number
  registrationClosesOn: string
  minTeamSize: number | null
  maxTeamSize: number | null
  isRegistered: boolean
  /** Registrations and team changes are still allowed. */
  registrationOpen: boolean
  /** A new student can register now: open and not full. */
  isOpen: boolean
  hasEnded: boolean
  hasCertificate: boolean
}

export interface EventTeamSummary { id: number; name: string; memberCount: number; isFull: boolean }

export interface TeamMember { name: string; isCaptain: boolean; isMe: boolean }

export interface MyTeam { id: number; name: string; joinCode: string; isCaptain: boolean; members: TeamMember[] }

/** Mirrors Features/EventHub/Dtos/EventDetailDto. */
export interface EventDetail {
  event: EventCard
  description: string
  myTeam: MyTeam | null
  teams: EventTeamSummary[]
  myCertificateCode: string | null
}

/** Mirrors Features/EventHub/Dtos/CertificateDto. */
export interface Certificate {
  code: string
  studentName: string
  eventTitle: string
  companyName: string
  format: EventFormat
  startDate: string
  endDate: string
  teamName: string | null
  issuedAt: string
}
