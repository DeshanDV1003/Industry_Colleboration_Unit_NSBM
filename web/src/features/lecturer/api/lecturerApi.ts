import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import type { SupportSignal } from '@/config/support'
import type { InterviewReport } from '@/features/interviews'
import { http } from '@/lib/http'
import type { LearningStage } from '@/types/learning'

export interface DirectoryStudent {
  id: number
  name: string
  studentNo: string
  programme: string
  careerPathName: string | null
  tasksAttempted: number
  roadmapPercent: number
  lastActivityAt: string | null
  signals: SupportSignal[]
}

/** Mirrors Features/Lecturing/Dtos/StudentInsightsDto. */
export interface StudentInsights {
  id: number
  name: string
  studentNo: string
  programme: string
  batch: string
  careerPathName: string | null
  baselinePercent: number | null
  roadmapCompleted: number
  roadmapTotal: number
  signals: SupportSignal[]
  practisedSkills: string[]
  listedOnlySkills: string[]
  attempts: { taskId: number; taskTitle: string; attempts: number; bestPassedChecks: number; totalChecks: number; lastSubmittedAt: string; knowledgeCheckDone: boolean }[]
  knowledgeChecks: { taskTitle: string; submittedAt: string; answers: { prompt: string; answer: string }[] }[]
  assignments: { id: number; taskId: number; taskTitle: string; lecturerName: string; note: string | null; assignedAt: string }[]
  notes: { id: number; lecturerName: string; text: string; createdAt: string }[]
  recentActivity: { at: string; description: string }[]
}

export interface AssignableTask {
  id: number
  title: string
  careerPathName: string
  level: LearningStage
  companyName: string
}

const LECTURER = ['lecturer'] as const

const lecturerApi = {
  directory: (search: string, needsFollowUp: boolean) =>
    http.get<DirectoryStudent[]>('/lecturer/students', { params: { search: search || undefined, needsFollowUp } }).then((r) => r.data),
  insights: (id: number) => http.get<StudentInsights>(`/lecturer/students/${id}`).then((r) => r.data),
  interviewReport: (id: number) => http.get<InterviewReport>(`/lecturer/students/${id}/interview-report`).then((r) => r.data),
  assignableTasks: () => http.get<AssignableTask[]>('/lecturer/assignable-tasks').then((r) => r.data),
  addNote: ({ studentId, text }: { studentId: number; text: string }) => http.post(`/lecturer/students/${studentId}/notes`, { text }),
  assign: ({ studentId, taskId, note }: { studentId: number; taskId: number; note: string | null }) =>
    http.post(`/lecturer/students/${studentId}/assignments`, { taskId, note }),
}

export function useStudentDirectory(search: string, needsFollowUp: boolean) {
  return useQuery({ queryKey: [...LECTURER, 'directory', search, needsFollowUp], queryFn: () => lecturerApi.directory(search, needsFollowUp) })
}

export const useStudentInsights = (id: number | null) =>
  useQuery({ queryKey: [...LECTURER, 'insights', id], queryFn: () => lecturerApi.insights(id!), enabled: id !== null })
export const useStudentInterviewReport = (id: number) => useQuery({ queryKey: [...LECTURER, 'interview', id], queryFn: () => lecturerApi.interviewReport(id) })
export const useAssignableTasks = () => useQuery({ queryKey: [...LECTURER, 'assignable-tasks'], queryFn: lecturerApi.assignableTasks })

function useLecturerMutation<T>(fn: (input: T) => Promise<unknown>) {
  const queryClient = useQueryClient()
  return useMutation({ mutationFn: fn, onSuccess: () => queryClient.invalidateQueries({ queryKey: LECTURER }) })
}

export const useAddSupportNote = () => useLecturerMutation(lecturerApi.addNote)
export const useAssignTask = () => useLecturerMutation(lecturerApi.assign)
