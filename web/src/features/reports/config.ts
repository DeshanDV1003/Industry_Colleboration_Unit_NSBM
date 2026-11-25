import { CalendarClock, CalendarHeart, ClipboardCheck, Users, type LucideIcon } from 'lucide-react'
import type { Role } from '@/config/roles'
import type { ReportParams } from './types'

export type ReportFilter = 'dateRange' | 'venue' | 'cohort' | 'event' | 'applicationStatus'

export interface ReportDefinition {
  key: string
  /** Path under /api/v1/reports. */
  endpoint: string
  title: string
  description: string
  icon: LucideIcon
  /** Mirrors the [Authorize] roles on ReportsController. */
  roles: Role[]
  filters: ReportFilter[]
  defaults: () => ReportParams
}

const iso = (d: Date) => `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`
const daysFromToday = (days: number) => { const d = new Date(); d.setDate(d.getDate() + days); return iso(d) }

export const REPORTS: ReportDefinition[] = [
  {
    key: 'hall-timetable',
    endpoint: 'hall-timetable',
    title: 'Hall timetable',
    description: 'Hall allocations for a period, for one venue or one cohort. Print it for notice boards or send it as Excel.',
    icon: CalendarClock,
    roles: ['Admin', 'Registrar'],
    filters: ['dateRange', 'venue', 'cohort'],
    defaults: () => ({ from: daysFromToday(0), to: daysFromToday(27), venueId: '', programme: '', academicYear: '' }),
  },
  {
    key: 'event-attendance',
    endpoint: 'event-attendance',
    title: 'Event attendance',
    description: 'Registrations, attendance and certificate codes for partner events.',
    icon: CalendarHeart,
    roles: ['Admin'],
    filters: ['event'],
    defaults: () => ({ eventId: '' }),
  },
  {
    key: 'applications',
    endpoint: 'applications',
    title: 'Internship applications',
    description: 'Applications submitted in a period and where each one stands, by company and vacancy.',
    icon: ClipboardCheck,
    roles: ['Admin'],
    filters: ['dateRange', 'applicationStatus'],
    defaults: () => ({ from: daysFromToday(-90), to: daysFromToday(0), status: '' }),
  },
  {
    key: 'student-participation',
    endpoint: 'student-participation',
    title: 'Student participation',
    description: 'What each student has taken part in, as counts for planning support. Never a score or a ranking.',
    icon: Users,
    roles: ['Admin', 'Lecturer'],
    filters: ['cohort'],
    defaults: () => ({ programme: '', academicYear: '' }),
  },
]

export const reportsFor = (role: Role) => REPORTS.filter((r) => r.roles.includes(role))

/** Mirrors Domain.Enums.ApplicationStatus. */
export const APPLICATION_STATUS_LABEL: Record<string, string> = {
  Submitted: 'Submitted',
  UnderReview: 'Under review',
  Shortlisted: 'Shortlisted',
  InterviewInvited: 'Interview invited',
  NotSelected: 'Not selected',
}
