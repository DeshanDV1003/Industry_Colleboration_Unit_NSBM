import { z } from 'zod'
import { CALENDAR_AUDIENCES, CALENDAR_EVENT_TYPES, type CalendarEntry, type CalendarEventInput } from './types'

/** Link ids are held as strings ('' = none) because they come from <select> elements. */
export const eventFormSchema = z.object({
  title: z.string().trim().min(1, 'Enter a title.').max(150),
  description: z.string().max(2000),
  type: z.enum(CALENDAR_EVENT_TYPES),
  audience: z.enum(CALENDAR_AUDIENCES),
  startDate: z.string().min(1, 'Choose a start date.'),
  endDate: z.string().min(1, 'Choose an end date.'),
  location: z.string().max(150),
  careerPathId: z.string(),
  industryTaskId: z.string(),
  vacancyId: z.string(),
})
  .refine((v) => v.endDate >= v.startDate, { path: ['endDate'], message: 'The end date cannot be before the start date.' })
  .refine((v) => v.type !== 'TaskRelease' || v.industryTaskId !== '', { path: ['industryTaskId'], message: 'Choose the industry task being released.' })

export type EventFormValues = z.infer<typeof eventFormSchema>

/** Which optional links make sense for the chosen type and audience; hidden links are cleared on submit. */
export const linksFor = (v: Pick<EventFormValues, 'type' | 'audience'>) => ({
  careerPath: v.audience === 'Students',
  task: v.type === 'TaskRelease',
  vacancy: v.type === 'InterviewDrive' || v.type === 'ApplicationWindow',
})

export const emptyEventForm = (day: string): EventFormValues => ({
  title: '', description: '', type: 'Programme', audience: 'Everyone', startDate: day, endDate: day,
  location: '', careerPathId: '', industryTaskId: '', vacancyId: '',
})

export const eventFormFromEntry = (e: CalendarEntry): EventFormValues => ({
  title: e.title,
  description: e.description ?? '',
  type: e.eventType ?? 'Programme',
  audience: e.audience ?? 'Everyone',
  startDate: e.startDate,
  endDate: e.endDate,
  location: e.location ?? '',
  careerPathId: e.careerPathId ? String(e.careerPathId) : '',
  industryTaskId: e.industryTaskId ? String(e.industryTaskId) : '',
  vacancyId: e.vacancyId ? String(e.vacancyId) : '',
})

const idOrNull = (value: string, allowed: boolean) => (allowed && value ? Number(value) : null)

export function toEventInput(v: EventFormValues): CalendarEventInput {
  const links = linksFor(v)
  return {
    title: v.title.trim(),
    description: v.description.trim() || null,
    type: v.type,
    audience: v.audience,
    startDate: v.startDate,
    endDate: v.endDate,
    location: v.location.trim() || null,
    careerPathId: idOrNull(v.careerPathId, links.careerPath),
    industryTaskId: idOrNull(v.industryTaskId, links.task),
    vacancyId: idOrNull(v.vacancyId, links.vacancy),
  }
}
