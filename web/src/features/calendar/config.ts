import type { BadgeTone } from '@/components/ui/Badge'
import type { Role } from '@/config/roles'
import type { CalendarAudience, CalendarEntry, CalendarEntrySource, CalendarEventType } from './types'

interface KindMeta { label: string; tone: BadgeTone; dot: string }

export const EVENT_TYPE_META: Record<CalendarEventType, KindMeta> = {
  Programme: { label: 'Programme', tone: 'primary', dot: 'bg-primary' },
  Workshop: { label: 'Workshop', tone: 'accent', dot: 'bg-accent' },
  IndustryTalk: { label: 'Industry talk', tone: 'accent', dot: 'bg-accent' },
  Hackathon: { label: 'Hackathon', tone: 'success', dot: 'bg-success' },
  TaskRelease: { label: 'Task release', tone: 'success', dot: 'bg-success' },
  CareerPlanMilestone: { label: 'Career-plan milestone', tone: 'primary', dot: 'bg-primary' },
  InterviewDrive: { label: 'Interview drive', tone: 'warning', dot: 'bg-warning' },
  ApplicationWindow: { label: 'Application window', tone: 'warning', dot: 'bg-warning' },
  CareerFair: { label: 'Career fair', tone: 'primary', dot: 'bg-primary' },
}

/** Entries that are not Unit events: the viewer's own dates and partner events. */
const SOURCE_META: Record<Exclude<CalendarEntrySource, 'UnitEvent'>, KindMeta> = {
  PlannerDeadline: { label: 'My deadline', tone: 'danger', dot: 'bg-danger' },
  VacancyClosing: { label: 'Vacancy closes', tone: 'neutral', dot: 'bg-muted' },
  AssignedTask: { label: 'Assigned task', tone: 'accent', dot: 'bg-accent' },
  InterviewInvite: { label: 'Interview invite', tone: 'warning', dot: 'bg-warning' },
  IndustryEvent: { label: 'Partner event', tone: 'success', dot: 'bg-success' },
  HallBooking: { label: 'My hall', tone: 'primary', dot: 'bg-primary' },
}

export const entryMeta = (entry: CalendarEntry): KindMeta =>
  entry.source === 'UnitEvent' && entry.eventType ? EVENT_TYPE_META[entry.eventType] : SOURCE_META[entry.source as keyof typeof SOURCE_META]

export const AUDIENCE_LABEL: Record<CalendarAudience, string> = {
  Everyone: 'Everyone',
  Students: 'Students',
  Lecturers: 'Lecturers',
  Partners: 'Industry partners',
}

/** Where an entry leads in the viewer's own portal, or null when there is no page for it. */
export function entryLink(entry: CalendarEntry, role: Role): string | null {
  switch (entry.source) {
    case 'PlannerDeadline': return '/planner'
    case 'InterviewInvite': return '/applications'
    case 'AssignedTask': return entry.industryTaskId ? `/tasks/${entry.industryTaskId}` : null
    case 'VacancyClosing':
      return role === 'Admin' ? '/admin-vacancies' : role === 'Industry' ? '/industry-hub' : '/career-center'
    case 'IndustryEvent':
      return role === 'Student' ? `/events/${entry.industryEventId}` : role === 'Industry' ? '/industry-events' : role === 'Admin' ? '/admin-events' : null
    case 'HallBooking': return entry.industryEventId ? `/events/${entry.industryEventId}` : null
    case 'UnitEvent':
      if (role === 'Student' && entry.industryTaskId) return `/tasks/${entry.industryTaskId}`
      if (role === 'Student' && entry.vacancyId) return '/career-center'
      return null
  }
}
