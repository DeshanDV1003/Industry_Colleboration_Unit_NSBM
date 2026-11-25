import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { http } from '@/lib/http'
import type { EventAttendance, IndustryEvent, IndustryEventInput, IndustryEventListItem } from '../types'

export const INDUSTRY_EVENTS = ['industry-events'] as const

const industryEventsApi = {
  mine: () => http.get<IndustryEventListItem[]>('/industry-events/mine').then((r) => r.data),
  get: (id: number) => http.get<IndustryEvent>(`/industry-events/${id}`).then((r) => r.data),
  create: (input: IndustryEventInput) => http.post<{ id: number }>('/industry-events', input).then((r) => r.data),
  update: ({ id, input }: { id: number; input: IndustryEventInput }) => http.put(`/industry-events/${id}`, input),
  attendance: (id: number) => http.get<EventAttendance>(`/industry-events/${id}/attendance`).then((r) => r.data),
  setAttendance: ({ eventId, registrationId, attended }: { eventId: number; registrationId: number; attended: boolean }) =>
    http.put(`/industry-events/${eventId}/attendance/${registrationId}`, { attended }),
  issueCertificates: (eventId: number) => http.post<{ issued: number }>(`/industry-events/${eventId}/certificates`).then((r) => r.data),
}

export function useCompanyEvents() {
  return useQuery({ queryKey: [...INDUSTRY_EVENTS, 'mine'], queryFn: industryEventsApi.mine })
}

export function useIndustryEvent(id: number | null) {
  return useQuery({ queryKey: [...INDUSTRY_EVENTS, id], queryFn: () => industryEventsApi.get(id!), enabled: id !== null })
}

export function useEventAttendance(id: number) {
  return useQuery({ queryKey: [...INDUSTRY_EVENTS, id, 'attendance'], queryFn: () => industryEventsApi.attendance(id) })
}

function useEventMutation<T, R>(fn: (input: T) => Promise<R>) {
  const queryClient = useQueryClient()
  return useMutation({ mutationFn: fn, onSuccess: () => queryClient.invalidateQueries({ queryKey: INDUSTRY_EVENTS }) })
}

export const useCreateIndustryEvent = () => useEventMutation(industryEventsApi.create)
export const useUpdateIndustryEvent = () => useEventMutation(industryEventsApi.update)
export const useSetAttendance = () => useEventMutation(industryEventsApi.setAttendance)
export const useIssueCertificates = () => useEventMutation(industryEventsApi.issueCertificates)
