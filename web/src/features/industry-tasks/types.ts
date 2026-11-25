import type { LearningStage } from '@/types/learning'
import type { ModerationStatus, WorkSituation } from '@/types/tasks'

/** Mirrors Features/IndustryTasks/Dtos/IndustryTaskListItemDto. */
export interface IndustryTaskListItem {
  id: number
  title: string
  companyName: string
  careerPathName: string
  level: LearningStage
  situation: WorkSituation
  status: ModerationStatus
  createdAt: string
  reviewedAt: string | null
  reviewNote: string | null
  attemptCount: number
}

export interface TaskCheck {
  id: number
  description: string
  pattern: string
  shouldMatch: boolean
  hint: string | null
}

/** Mirrors Features/IndustryTasks/Dtos/IndustryTaskDto. */
export interface IndustryTask {
  id: number
  title: string
  summary: string
  instructions: string
  careerPathId: number
  careerPathName: string
  level: LearningStage
  situation: WorkSituation
  estimatedMinutes: number
  starterLanguage: string | null
  starterCode: string | null
  companyName: string
  status: ModerationStatus
  createdAt: string
  reviewedAt: string | null
  reviewedBy: string | null
  reviewNote: string | null
  canBeEdited: boolean
  canBeReviewed: boolean
  skills: { id: number; name: string }[]
  checks: TaskCheck[]
  knowledgeQuestions: { id: number; prompt: string }[]
}

/** Mirrors Features/IndustryTasks/Models/IndustryTaskInput. */
export interface IndustryTaskInput {
  title: string
  summary: string
  instructions: string
  careerPathId: number
  level: LearningStage
  situation: WorkSituation
  estimatedMinutes: number
  starterLanguage: string | null
  starterCode: string | null
  skillIds: number[]
  checks: { description: string; pattern: string; shouldMatch: boolean; hint: string | null }[]
  knowledgeQuestions: string[]
}
