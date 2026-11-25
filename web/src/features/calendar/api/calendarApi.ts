import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { http } from '@/lib/http'
import type { CalendarEntry, CalendarEventInput, CalendarOptions } from '../types'

export const CALENDAR = ['calendar'] as const

const calendarApi = {
  year: (year: number) => http.get<CalendarEntry[]>(`/calendar/${year}`).then((r) => r.data),
  options: () => http.get<CalendarOptions>('/calendar/options').then((r) => r.data),
  create: (input: CalendarEventInput) => http.post<{ id: number }>('/calendar/events', input).then((r) => r.data),
  update: ({ id, input }: { id: number; input: CalendarEventInput }) => http.put(`/calendar/events/${id}`, input),
  remove: (id: number) => http.delete(`/calendar/events/${id}`),
}

export function useCalendarYear(year: number) {
  return useQuery({ queryKey: [...CALENDAR, year], queryFn: () => calendarApi.year(year) })
}

export function useCalendarOptions(enabled: boolean) {
  return useQuery({ queryKey: [...CALENDAR, 'options'], queryFn: calendarApi.options, enabled })
}

function useCalendarMutation<T>(fn: (input: T) => Promise<unknown>) {
  const queryClient = useQueryClient()
  return useMutation({ mutationFn: fn, onSuccess: () => queryClient.invalidateQueries({ queryKey: CALENDAR }) })
}

export const useCreateCalendarEvent = () => useCalendarMutation(calendarApi.create)
export const useUpdateCalendarEvent = () => useCalendarMutation(calendarApi.update)
export const useDeleteCalendarEvent = () => useCalendarMutation(calendarApi.remove)
