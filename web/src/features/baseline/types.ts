import type { SkillSignal } from '@/types/learning'

export interface BaselineOption {
  id: number
  text: string
}

export interface BaselineQuestion {
  id: number
  prompt: string
  skillName: string
  options: BaselineOption[]
}

export interface SkillSnapshot {
  skillId: number
  skillName: string
  correct: number
  total: number
  signal: SkillSignal
}

export interface QuestionReview {
  questionId: number
  prompt: string
  skillName: string
  isCorrect: boolean
  selectedOption: string
  correctOption: string
  explanation: string | null
}

/** Mirrors Features/Baseline/Dtos/BaselineResultDto. */
export interface BaselineResult {
  attemptId: number
  completedAt: string
  correctCount: number
  questionCount: number
  scorePercent: number
  skills: SkillSnapshot[]
  review: QuestionReview[]
}

/** Mirrors Features/Baseline/Dtos/BaselineDto. `careerPathId` is null until a path is chosen. */
export interface Baseline {
  careerPathId: number | null
  careerPathName: string | null
  questions: BaselineQuestion[]
  latestResult: BaselineResult | null
}

export interface BaselineAnswerInput {
  questionId: number
  optionId: number
}
