import { useQuery } from '@tanstack/react-query'
import { http } from '@/lib/http'
import type { ApplicationStatus } from '@/types/opportunities'

/** Mirrors Features/Research/Dtos/ResearchOverviewDto. */
export interface ResearchOverview {
  students: number
  studentsWithCareerPath: number
  studentsByCareerPath: { label: string; count: number }[]
  baselineAttempts: number
  averageBaselinePercent: number
  approvedTasks: number
  taskAttempts: number
  taskAttemptsAllChecksPassed: number
  knowledgeChecks: number
  interviewAnswers: number
  approvedVacancies: number
  applicationsByStatus: { status: ApplicationStatus; count: number }[]
  mentorQuestions: number
  mentorQuestionsAnswered: number
  sprintItemsDone: number
  topSkillGaps: { skillName: string; careerPathName: string; studentsMissing: number; studentsOnPath: number }[]
}

export const useResearchOverview = () =>
  useQuery({ queryKey: ['research', 'overview'], queryFn: () => http.get<ResearchOverview>('/research/overview').then((r) => r.data) })
