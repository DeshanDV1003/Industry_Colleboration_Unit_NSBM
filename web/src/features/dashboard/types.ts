/** Mirrors Features/Dashboard/Dtos/StudentOverviewDto. */
export interface StudentOverview {
  careerPath: {
    id: number
    slug: string
    name: string
    tagline: string
    selectedAt: string | null
  } | null
  roadmap: {
    completed: number
    total: number
    percent: number
    nextStepTitle: string | null
  }
  baseline: {
    completedAt: string
    scorePercent: number
    strongSkills: string[]
    focusSkills: string[]
  } | null
}
