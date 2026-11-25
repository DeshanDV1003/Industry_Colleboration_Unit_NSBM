import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { http } from '@/lib/http'

/** Mirrors Features/Mentoring/Dtos/MentorQuestionDto. */
export interface MentorQuestion {
  id: number
  title: string
  body: string
  careerPathName: string | null
  studentName: string
  askedAt: string
  reply: string | null
  repliedBy: string | null
  repliedByCompany: string | null
  repliedAt: string | null
}

const MENTORING = ['mentoring'] as const

const mentoringApi = {
  mine: () => http.get<MentorQuestion[]>('/mentoring/mine').then((r) => r.data),
  ask: (body: { title: string; body: string }) => http.post<{ id: number }>('/mentoring/questions', body).then((r) => r.data),
  queue: () => http.get<MentorQuestion[]>('/mentoring/queue').then((r) => r.data),
  reply: ({ id, reply }: { id: number; reply: string }) => http.post(`/mentoring/questions/${id}/reply`, { reply }),
}

export const useMyMentorQuestions = () => useQuery({ queryKey: [...MENTORING, 'mine'], queryFn: mentoringApi.mine })
export const useMentorQueue = () => useQuery({ queryKey: [...MENTORING, 'queue'], queryFn: mentoringApi.queue })

export function useAskMentor() {
  const queryClient = useQueryClient()
  return useMutation({ mutationFn: mentoringApi.ask, onSuccess: () => queryClient.invalidateQueries({ queryKey: MENTORING }) })
}

export function useReplyToMentorQuestion() {
  const queryClient = useQueryClient()
  return useMutation({ mutationFn: mentoringApi.reply, onSuccess: () => queryClient.invalidateQueries({ queryKey: MENTORING }) })
}
