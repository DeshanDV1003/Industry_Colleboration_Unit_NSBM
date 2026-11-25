import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { http } from '@/lib/http'
import type { Announcement, AnnouncementInput } from '../types'

const ANNOUNCEMENTS = ['announcements'] as const

const announcementsApi = {
  list: () => http.get<Announcement[]>('/announcements').then((r) => r.data),
  send: (input: AnnouncementInput) => http.post<{ id: number }>('/announcements', input).then((r) => r.data),
}

export function useAnnouncements() {
  return useQuery({ queryKey: ANNOUNCEMENTS, queryFn: announcementsApi.list })
}

export function useSendAnnouncement() {
  const queryClient = useQueryClient()
  return useMutation({ mutationFn: announcementsApi.send, onSuccess: () => queryClient.invalidateQueries({ queryKey: ANNOUNCEMENTS }) })
}
