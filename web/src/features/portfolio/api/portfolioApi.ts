import { useQuery } from '@tanstack/react-query'
import type { ReviewComment } from '@/components/ReviewComments'
import { http } from '@/lib/http'
import { ME } from '@/lib/queryKeys'

/** Mirrors Features/Portfolio/Dtos/PortfolioDto. */
export interface Portfolio {
  fullName: string
  studentNo: string
  programme: string
  careerPathName: string | null
  headline: string | null
  roadmapCompleted: number
  roadmapTotal: number
  baselineScorePercent: number | null
  practisedSkills: string[]
  listedSkills: string[]
  tasks: {
    taskId: number
    title: string
    companyName: string
    attempts: number
    bestPassedChecks: number
    totalChecks: number
    knowledgeCheckDone: boolean
    reviews: ReviewComment[]
  }[]
  interview: { softSkillsAnswered: number; technicalAnswered: number; averageCoveragePercent: number }
  sprintItemsDone: number
  applicationsSent: number
  lecturerNotes: { lecturerName: string; text: string; createdAt: string }[]
}

export function usePortfolio() {
  return useQuery({ queryKey: [...ME, 'portfolio'], queryFn: () => http.get<Portfolio>('/me/portfolio').then((r) => r.data) })
}
