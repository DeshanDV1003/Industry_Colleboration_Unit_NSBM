import { z } from 'zod'
import type { Vacancy, VacancyInput } from './types'

const today = () => new Date().toISOString().slice(0, 10)

/** Client-side mirror of VacancyInputValidator. */
export const vacancyFormSchema = z.object({
  title: z.string().trim().min(1, 'Enter a title').max(150),
  description: z.string().trim().min(1, 'Describe the internship').max(4000),
  careerPathId: z.number({ message: 'Choose a job role' }).int().positive('Choose a job role'),
  workMode: z.enum(['OnSite', 'Hybrid', 'Remote']),
  location: z.string().trim().min(1, 'Enter a location').max(150),
  durationMonths: z.number({ message: 'Enter months' }).int().min(1, 'At least 1 month').max(12, 'At most 12 months'),
  closingDate: z.string().refine((d) => d > today(), 'The closing date must be in the future'),
  requiredSkillIds: z.array(z.number()).min(1, 'Pick at least one required skill'),
  preferredSkillIds: z.array(z.number()),
})

export type VacancyFormValues = z.infer<typeof vacancyFormSchema>

const inDays = (days: number) => new Date(Date.now() + days * 86_400_000).toISOString().slice(0, 10)

export const emptyVacancyForm: VacancyFormValues = {
  title: '',
  description: '',
  careerPathId: 0,
  workMode: 'Hybrid',
  location: '',
  durationMonths: 6,
  closingDate: inDays(30),
  requiredSkillIds: [],
  preferredSkillIds: [],
}

export const toVacancyFormValues = (v: Vacancy): VacancyFormValues => ({
  title: v.title,
  description: v.description,
  careerPathId: v.careerPathId,
  workMode: v.workMode,
  location: v.location,
  durationMonths: v.durationMonths,
  closingDate: v.closingDate,
  requiredSkillIds: v.requiredSkills.map((s) => s.id),
  preferredSkillIds: v.preferredSkills.map((s) => s.id),
})

export const toVacancyInput = (values: VacancyFormValues): VacancyInput => values
