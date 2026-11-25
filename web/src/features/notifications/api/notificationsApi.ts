import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { http } from '@/lib/http'
import { ME } from '@/lib/queryKeys'
import type { NotificationList, WeeklyDigest } from '../types'

const NOTIFICATIONS = [...ME, 'notifications'] as const
const DIGEST = [...ME, 'digest'] as const

/** New notifications arrive from other people's actions, so the bell checks in the background. */
const POLL_MS = 60_000

const notificationsApi = {
  list: (take: number) => http.get<NotificationList>('/me/notifications', { params: { take } }).then((r) => r.data),
  digest: () => http.get<WeeklyDigest>('/me/digest').then((r) => r.data),
  markRead: (id: number) => http.post(`/me/notifications/${id}/read`),
  markAllRead: () => http.post('/me/notifications/read-all'),
}

export function useNotifications(take: number) {
  return useQuery({
    queryKey: [...NOTIFICATIONS, take],
    queryFn: () => notificationsApi.list(take),
    refetchInterval: POLL_MS,
  })
}

export function useWeeklyDigest() {
  return useQuery({ queryKey: DIGEST, queryFn: notificationsApi.digest })
}

function useReadMutation<T>(fn: (input: T) => Promise<unknown>) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: fn,
    onSuccess: () => Promise.all([
      queryClient.invalidateQueries({ queryKey: NOTIFICATIONS }),
      queryClient.invalidateQueries({ queryKey: DIGEST }),
    ]),
  })
}

export const useMarkNotificationRead = () => useReadMutation(notificationsApi.markRead)
export const useMarkAllNotificationsRead = () => useReadMutation(notificationsApi.markAllRead)
