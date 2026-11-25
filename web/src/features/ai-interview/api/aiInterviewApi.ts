import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { http } from '@/lib/http'
import { ME } from '@/lib/queryKeys'
import type { AiInterview, AiInterviewAvailability, AiInterviewSummary, InterviewStage } from '../types'

const AI_INTERVIEWS = [...ME, 'ai-interviews'] as const

const aiInterviewApi = {
  availability: () => http.get<AiInterviewAvailability>('/me/ai-interviews/availability').then((r) => r.data),
  list: () => http.get<AiInterviewSummary[]>('/me/ai-interviews').then((r) => r.data),
  get: (id: number) => http.get<AiInterview>(`/me/ai-interviews/${id}`).then((r) => r.data),
  start: (stage: InterviewStage) => http.post<AiInterview>('/me/ai-interviews', { stage, consentToAiProcessing: true }).then((r) => r.data),
  answer: ({ id, answer }: { id: number; answer: string }) => http.post<AiInterview>(`/me/ai-interviews/${id}/answers`, { answer }).then((r) => r.data),
  continue: (id: number) => http.post<AiInterview>(`/me/ai-interviews/${id}/continue`).then((r) => r.data),
  finish: (id: number) => http.post<AiInterview>(`/me/ai-interviews/${id}/finish`).then((r) => r.data),
}

export function useAiInterviewAvailability() {
  return useQuery({ queryKey: [...AI_INTERVIEWS, 'availability'], queryFn: aiInterviewApi.availability })
}

export function useMyAiInterviews() {
  return useQuery({ queryKey: AI_INTERVIEWS, queryFn: aiInterviewApi.list })
}

export function useAiInterview(id: number) {
  return useQuery({ queryKey: [...AI_INTERVIEWS, id], queryFn: () => aiInterviewApi.get(id) })
}

/** Every step returns the whole session, so it is written straight into the cache instead of refetched. */
function useSessionMutation<T>(fn: (input: T) => Promise<AiInterview>) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: fn,
    onSuccess: (session) => {
      queryClient.setQueryData([...AI_INTERVIEWS, session.id], session)
      return queryClient.invalidateQueries({ queryKey: AI_INTERVIEWS, exact: true })
    },
    // A failed AI reply can still have saved the answer, so the session is refreshed either way.
    onError: () => queryClient.invalidateQueries({ queryKey: AI_INTERVIEWS }),
  })
}

export const useStartAiInterview = () => useSessionMutation(aiInterviewApi.start)
export const useAnswerAiInterview = () => useSessionMutation(aiInterviewApi.answer)
export const useContinueAiInterview = () => useSessionMutation(aiInterviewApi.continue)
export const useFinishAiInterview = () => useSessionMutation(aiInterviewApi.finish)
