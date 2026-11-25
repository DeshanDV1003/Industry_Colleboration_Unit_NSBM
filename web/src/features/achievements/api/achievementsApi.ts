import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { http } from '@/lib/http'
import { ME } from '@/lib/queryKeys'
import type { ReviewDecision } from '@/types/review'
import type { Achievements, BadgeReviewItem, ProfileCard, ProfileCardSettings } from '../types'

const ACHIEVEMENTS = [...ME, 'achievements'] as const
const BADGE_REVIEWS = ['badge-reviews'] as const

const achievementsApi = {
  mine: () => http.get<Achievements>('/me/achievements').then((r) => r.data),
  requestBadge: ({ skillId, note }: { skillId: number; note: string | null }) => http.post('/me/achievements/badges', { skillId, note }),
  setSharing: (share: boolean) => http.put<ProfileCardSettings>('/me/achievements/profile-card', { share }).then((r) => r.data),
  reviewQueue: () => http.get<BadgeReviewItem[]>('/badge-reviews').then((r) => r.data),
  review: ({ id, decision, note }: { id: number; decision: ReviewDecision; note: string | null }) => http.post(`/badge-reviews/${id}`, { decision, note }),
  card: (slug: string) => http.get<ProfileCard>(`/profile-cards/${encodeURIComponent(slug)}`).then((r) => r.data),
}

export function useMyAchievements() {
  return useQuery({ queryKey: ACHIEVEMENTS, queryFn: achievementsApi.mine })
}

function useAchievementsMutation<T, R>(fn: (input: T) => Promise<R>) {
  const queryClient = useQueryClient()
  return useMutation({ mutationFn: fn, onSuccess: () => queryClient.invalidateQueries({ queryKey: ACHIEVEMENTS }) })
}

export const useRequestBadge = () => useAchievementsMutation(achievementsApi.requestBadge)
export const useSetProfileCardSharing = () => useAchievementsMutation(achievementsApi.setSharing)

export function useBadgeReviewQueue() {
  return useQuery({ queryKey: BADGE_REVIEWS, queryFn: achievementsApi.reviewQueue })
}

export function useReviewBadge() {
  const queryClient = useQueryClient()
  return useMutation({ mutationFn: achievementsApi.review, onSuccess: () => queryClient.invalidateQueries({ queryKey: BADGE_REVIEWS }) })
}

/** Public: works without signing in. */
export function usePublicProfileCard(slug: string) {
  return useQuery({ queryKey: ['profile-card', slug], queryFn: () => achievementsApi.card(slug), retry: false })
}
