import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { http } from '@/lib/http'
import { ME } from '@/lib/queryKeys'
import type { AnswerFeedback, AnswerInputMode, InterviewReport, InterviewSession, InterviewStage } from '../types'

const interviewKeys = {
  all: [...ME, 'interviews'] as const,
  session: (stage: InterviewStage, careerPathId: number | null) => [...ME, 'interviews', stage, careerPathId] as const,
  report: [...ME, 'interviews', 'report'] as const,
}

const interviewsApi = {
  session: (stage: InterviewStage, careerPathId: number | null) =>
    http.get<InterviewSession>(`/me/interviews/${stage}`, { params: careerPathId ? { careerPathId } : {} }).then((r) => r.data),
  answer: (body: { questionId: number; text: string; inputMode: AnswerInputMode }) =>
    http.post<AnswerFeedback>('/me/interviews/answers', body).then((r) => r.data),
  report: () => http.get<InterviewReport>('/me/interviews/report').then((r) => r.data),
}

export function useInterviewSession(stage: InterviewStage, careerPathId: number | null = null) {
  return useQuery({ queryKey: interviewKeys.session(stage, careerPathId), queryFn: () => interviewsApi.session(stage, careerPathId) })
}

export function useSubmitInterviewAnswer() {
  const queryClient = useQueryClient()
  return useMutation({ mutationFn: interviewsApi.answer, onSuccess: () => queryClient.invalidateQueries({ queryKey: interviewKeys.all }) })
}

export function useInterviewReport() {
  return useQuery({ queryKey: interviewKeys.report, queryFn: interviewsApi.report })
}
