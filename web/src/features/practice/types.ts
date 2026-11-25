import type { LearningStage } from '@/types/learning'
import type { WorkSituation } from '@/types/tasks'

/** Mirrors Features/Practice/Dtos/TaskLibraryItemDto. */
export interface TaskLibraryItem {
  id: number
  title: string
  summary: string
  companyName: string
  careerPathId: number
  careerPathName: string
  level: LearningStage
  situation: WorkSituation
  estimatedMinutes: number
  skills: string[]
  checkCount: number
  myAttemptCount: number
  myBestPassedChecks: number | null
  knowledgeCheckDone: boolean
  assignedBy: string | null
}

export interface TaskLibraryFilters {
  careerPathId?: number
  level?: LearningStage
  situation?: WorkSituation
  search?: string
}

export interface CheckResult {
  checkId: number
  description: string
  passed: boolean
  hint: string | null
}

/** Mirrors Features/Practice/Dtos/TaskAttemptDto. */
export interface TaskAttempt {
  id: number
  attemptNumber: number
  submittedAt: string
  passedChecks: number
  totalChecks: number
  code: string
  results: CheckResult[]
  reviews: AttemptReview[]
}

/** A review comment from the company that set the task. */
export interface AttemptReview {
  reviewerName: string
  comment: string
  improvements: string | null
  reviewedAt: string
}

/** Mirrors Features/Practice/Dtos/WorkbenchDto. */
export interface Workbench {
  task: {
    id: number
    title: string
    summary: string
    instructions: string
    companyName: string
    careerPathName: string
    level: LearningStage
    situation: WorkSituation
    estimatedMinutes: number
    starterLanguage: string | null
    starterCode: string | null
    skills: string[]
    checks: { id: number; description: string }[]
    knowledgeQuestionCount: number
  }
  attempts: TaskAttempt[]
  knowledgeCheckSubmittedAt: string | null
}

/** Mirrors Features/Practice/Dtos/KnowledgeCheckDto. */
export interface KnowledgeCheck {
  taskId: number
  taskTitle: string
  hasAttempt: boolean
  submittedAt: string | null
  questions: { id: number; prompt: string; answer: string | null }[]
}
