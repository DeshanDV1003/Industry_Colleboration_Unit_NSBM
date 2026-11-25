import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { INDUSTRY_EVENTS, type IndustryEventListItem } from '@/features/industry-events'
import { INDUSTRY_TASKS, type IndustryTaskListItem } from '@/features/industry-tasks'
import { VACANCIES, type VacancyListItem } from '@/features/vacancies'
import { http } from '@/lib/http'

import type { ReviewDecision } from '@/types/review'

export type { ReviewDecision }

export interface ReviewInput {
  id: number
  decision: ReviewDecision
  note: string | null
}

const moderationApi = {
  taskQueue: () => http.get<IndustryTaskListItem[]>('/industry-tasks/review-queue').then((r) => r.data),
  reviewTask: ({ id, decision, note }: ReviewInput) => http.post(`/industry-tasks/${id}/review`, { decision, note }),
  vacancyQueue: () => http.get<VacancyListItem[]>('/vacancies/review-queue').then((r) => r.data),
  reviewVacancy: ({ id, decision, note }: ReviewInput) => http.post(`/vacancies/${id}/review`, { decision, note }),
  eventQueue: () => http.get<IndustryEventListItem[]>('/industry-events/review-queue').then((r) => r.data),
  reviewEvent: ({ id, decision, note }: ReviewInput) => http.post(`/industry-events/${id}/review`, { decision, note }),
}

/** Every task across companies; the board filters by status locally so tab counts stay accurate. */
export function useTaskReviewQueue() {
  return useQuery({ queryKey: [...INDUSTRY_TASKS, 'review-queue'], queryFn: moderationApi.taskQueue })
}

export function useReviewTask() {
  const queryClient = useQueryClient()
  return useMutation({ mutationFn: moderationApi.reviewTask, onSuccess: () => queryClient.invalidateQueries({ queryKey: INDUSTRY_TASKS }) })
}

export function useVacancyReviewQueue() {
  return useQuery({ queryKey: [...VACANCIES, 'review-queue'], queryFn: moderationApi.vacancyQueue })
}

export function useReviewVacancy() {
  const queryClient = useQueryClient()
  return useMutation({ mutationFn: moderationApi.reviewVacancy, onSuccess: () => queryClient.invalidateQueries({ queryKey: VACANCIES }) })
}

export function useEventReviewQueue() {
  return useQuery({ queryKey: [...INDUSTRY_EVENTS, 'review-queue'], queryFn: moderationApi.eventQueue })
}

export function useReviewEvent() {
  const queryClient = useQueryClient()
  return useMutation({ mutationFn: moderationApi.reviewEvent, onSuccess: () => queryClient.invalidateQueries({ queryKey: INDUSTRY_EVENTS }) })
}
