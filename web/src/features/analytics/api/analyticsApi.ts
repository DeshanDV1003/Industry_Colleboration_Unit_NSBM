import { keepPreviousData, useQuery } from '@tanstack/react-query'
import { http } from '@/lib/http'
import type { Analytics } from '../types'

const analyticsApi = {
  get: (weeks: number) => http.get<Analytics>('/analytics', { params: { weeks } }).then((r) => r.data),
}

/** Keeps the current charts on screen while another period loads. */
export function useAnalytics(weeks: number) {
  return useQuery({ queryKey: ['analytics', weeks], queryFn: () => analyticsApi.get(weeks), placeholderData: keepPreviousData })
}
