import type { ModerationStatus } from '@/types/tasks'
import type { WorkMode } from '@/types/opportunities'

/** Mirrors Features/Vacancies/Dtos/VacancyListItemDto. */
export interface VacancyListItem {
  id: number
  title: string
  companyName: string
  careerPathName: string
  workMode: WorkMode
  location: string
  closingDate: string
  status: ModerationStatus
  createdAt: string
  reviewedAt: string | null
  reviewNote: string | null
  applicationCount: number
}

/** Mirrors Features/Vacancies/Dtos/VacancyDto. */
export interface Vacancy {
  id: number
  title: string
  description: string
  careerPathId: number
  careerPathName: string
  workMode: WorkMode
  location: string
  durationMonths: number
  closingDate: string
  companyName: string
  status: ModerationStatus
  createdAt: string
  reviewedAt: string | null
  reviewedBy: string | null
  reviewNote: string | null
  canBeEdited: boolean
  canBeReviewed: boolean
  requiredSkills: { id: number; name: string }[]
  preferredSkills: { id: number; name: string }[]
}

/** Mirrors Features/Vacancies/Models/VacancyInput. */
export interface VacancyInput {
  title: string
  description: string
  careerPathId: number
  workMode: WorkMode
  location: string
  durationMonths: number
  closingDate: string
  requiredSkillIds: number[]
  preferredSkillIds: number[]
}
