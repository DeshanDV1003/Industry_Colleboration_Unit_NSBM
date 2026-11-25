import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import type { ReviewComment } from '@/components/ReviewComments'
import { http } from '@/lib/http'

/** Mirrors Features/TaskReviews/Dtos/AttemptForReviewDto. */
export interface AttemptForReview {
  attemptId: number
  taskId: number
  taskTitle: string
  studentName: string
  studentNo: string
  attemptNumber: number
  submittedAt: string
  passedChecks: number
  totalChecks: number
  code: string
  reviews: ReviewComment[]
}

const TASK_REVIEWS = ['task-reviews'] as const

export function useAttemptsForReview() {
  return useQuery({ queryKey: TASK_REVIEWS, queryFn: () => http.get<AttemptForReview[]>('/task-reviews/attempts').then((r) => r.data) })
}

export function useReviewAttempt() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ attemptId, comment, improvements }: { attemptId: number; comment: string; improvements: string | null }) =>
      http.post(`/task-reviews/attempts/${attemptId}`, { comment, improvements }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: TASK_REVIEWS }),
  })
}
