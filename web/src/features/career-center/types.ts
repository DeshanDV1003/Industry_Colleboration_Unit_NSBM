import type { ApplicationStatus, SkillEvidenceRow, WorkMode } from '@/types/opportunities'

/** Mirrors Features/CareerCenter/Dtos/VacancyMatchDto. */
export interface VacancyMatch {
  id: number
  title: string
  description: string
  companyName: string
  careerPathName: string
  workMode: WorkMode
  location: string
  durationMonths: number
  closingDate: string
  matchPercent: number
  required: SkillEvidenceRow[]
  preferred: SkillEvidenceRow[]
  myApplicationStatus: ApplicationStatus | null
}

export interface VacancySearch {
  careerPathId?: number
  workMode?: WorkMode
  search?: string
}

/** Mirrors Features/CareerCenter/Dtos/MyApplicationDto. */
export interface MyApplication {
  id: number
  vacancyId: number
  vacancyTitle: string
  companyName: string
  workMode: WorkMode
  status: ApplicationStatus
  submittedAt: string
  statusChangedAt: string
  matchPercentAtSubmission: number
  coverNote: string
}
