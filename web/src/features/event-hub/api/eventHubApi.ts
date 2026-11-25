import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { http } from '@/lib/http'
import { ME } from '@/lib/queryKeys'
import type { Certificate, EventCard, EventDetail } from '../types'

const EVENTS = [...ME, 'events'] as const

const eventHubApi = {
  list: () => http.get<EventCard[]>('/events').then((r) => r.data),
  get: (id: number) => http.get<EventDetail>(`/events/${id}`).then((r) => r.data),
  certificates: () => http.get<Certificate[]>('/events/certificates').then((r) => r.data),
  verify: (code: string) => http.get<Certificate>(`/certificates/${encodeURIComponent(code)}`).then((r) => r.data),
  register: (id: number) => http.post(`/events/${id}/registration`),
  cancel: (id: number) => http.delete(`/events/${id}/registration`),
  createTeam: ({ id, name }: { id: number; name: string }) => http.post<{ joinCode: string }>(`/events/${id}/teams`, { name }).then((r) => r.data),
  joinTeam: ({ id, joinCode }: { id: number; joinCode: string }) => http.post(`/events/${id}/teams/join`, { joinCode }),
  leaveTeam: (id: number) => http.delete(`/events/${id}/team`),
}

export function useEventHub() {
  return useQuery({ queryKey: EVENTS, queryFn: eventHubApi.list })
}

export function useEventDetail(id: number) {
  return useQuery({ queryKey: [...EVENTS, id], queryFn: () => eventHubApi.get(id) })
}

export function useMyCertificates() {
  return useQuery({ queryKey: [...EVENTS, 'certificates'], queryFn: eventHubApi.certificates })
}

/** Public: works without signing in, so employers can check a shared code. */
export function useVerifyCertificate(code: string) {
  return useQuery({ queryKey: ['certificate', code], queryFn: () => eventHubApi.verify(code), retry: false })
}

function useEventHubMutation<T, R>(fn: (input: T) => Promise<R>) {
  const queryClient = useQueryClient()
  // Registration changes appear on the calendar too, so both are refreshed.
  return useMutation({
    mutationFn: fn,
    onSuccess: () => Promise.all([
      queryClient.invalidateQueries({ queryKey: EVENTS }),
      queryClient.invalidateQueries({ queryKey: ['calendar'] }),
    ]),
  })
}

export const useRegisterForEvent = () => useEventHubMutation(eventHubApi.register)
export const useCancelRegistration = () => useEventHubMutation(eventHubApi.cancel)
export const useCreateTeam = () => useEventHubMutation(eventHubApi.createTeam)
export const useJoinTeam = () => useEventHubMutation(eventHubApi.joinTeam)
export const useLeaveTeam = () => useEventHubMutation(eventHubApi.leaveTeam)
