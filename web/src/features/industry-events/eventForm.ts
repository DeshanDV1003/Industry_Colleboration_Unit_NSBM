import { z } from 'zod'
import { EVENT_FORMATS } from '@/config/events'
import type { IndustryEvent, IndustryEventInput } from './types'

/** Form shape: team sizes are only sent when "team event" is ticked. */
export const eventFormSchema = z.object({
  title: z.string().trim().min(1, 'Enter a title.').max(150),
  description: z.string().trim().min(20, 'Describe the event in at least 20 characters.').max(4000),
  format: z.enum(EVENT_FORMATS),
  startDate: z.string().min(1, 'Choose a start date.'),
  endDate: z.string().min(1, 'Choose an end date.'),
  registrationClosesOn: z.string().min(1, 'Choose when registration closes.'),
  location: z.string().trim().min(1, 'Enter a location, or "Online".').max(150),
  capacity: z.number({ error: 'Enter a number.' }).int().min(1).max(1000),
  isTeamEvent: z.boolean(),
  minTeamSize: z.number({ error: 'Enter a number.' }).int().min(1).max(10),
  maxTeamSize: z.number({ error: 'Enter a number.' }).int().min(2).max(10),
})
  .refine((v) => v.endDate >= v.startDate, { path: ['endDate'], message: 'The end date cannot be before the start date.' })
  .refine((v) => v.registrationClosesOn <= v.startDate, { path: ['registrationClosesOn'], message: 'Registration must close on or before the start date.' })
  .refine((v) => !v.isTeamEvent || v.maxTeamSize >= v.minTeamSize, { path: ['maxTeamSize'], message: 'The largest team cannot be smaller than the smallest.' })

export type EventFormValues = z.infer<typeof eventFormSchema>

export const emptyEventForm: EventFormValues = {
  title: '', description: '', format: 'TechTalk', startDate: '', endDate: '', registrationClosesOn: '',
  location: '', capacity: 50, isTeamEvent: false, minTeamSize: 2, maxTeamSize: 4,
}

export const toEventFormValues = (e: IndustryEvent): EventFormValues => ({
  title: e.title,
  description: e.description,
  format: e.format,
  startDate: e.startDate,
  endDate: e.endDate,
  registrationClosesOn: e.registrationClosesOn,
  location: e.location,
  capacity: e.capacity,
  isTeamEvent: e.maxTeamSize !== null,
  minTeamSize: e.minTeamSize ?? 2,
  maxTeamSize: e.maxTeamSize ?? 4,
})

export const toEventInput = (v: EventFormValues): IndustryEventInput => ({
  title: v.title.trim(),
  description: v.description.trim(),
  format: v.format,
  startDate: v.startDate,
  endDate: v.endDate,
  registrationClosesOn: v.registrationClosesOn,
  location: v.location.trim(),
  capacity: v.capacity,
  minTeamSize: v.isTeamEvent ? v.minTeamSize : null,
  maxTeamSize: v.isTeamEvent ? v.maxTeamSize : null,
})
