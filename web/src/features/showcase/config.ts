import { EVENT_FORMAT_META, type EventFormat } from '@/config/events'
import { EVENT_TYPE_META } from '@/features/calendar'
import type { PartnerInterest } from './types'

export const PARTNER_INTEREST_LABEL: Record<PartnerInterest, string> = {
  IndustryTasks: 'Setting industry tasks',
  Internships: 'Offering internships',
  Events: 'Hosting talks or hackathons',
  Mentoring: 'Mentoring students',
  Other: 'Something else',
}

/** Public events are Unit calendar types or partner event formats; both have display labels elsewhere. */
export function eventKindLabel(kind: string): string {
  if (kind in EVENT_FORMAT_META) return EVENT_FORMAT_META[kind as EventFormat].label
  if (kind in EVENT_TYPE_META) return EVENT_TYPE_META[kind as keyof typeof EVENT_TYPE_META].label
  return kind
}
