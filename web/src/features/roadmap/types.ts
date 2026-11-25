import type { LearningStage } from '@/types/learning'

export interface RoadmapStep {
  id: number
  order: number
  stage: LearningStage
  title: string
  description: string
  outcome: string | null
  skillName: string | null
  isCompleted: boolean
  completedAt: string | null
}

/** Mirrors Features/Roadmap/Dtos/RoadmapDto. `careerPathId` is null until a path is chosen. */
export interface Roadmap {
  careerPathId: number | null
  careerPathName: string | null
  completedCount: number
  totalCount: number
  progressPercent: number
  steps: RoadmapStep[]
}
