import { keepPreviousData, useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { http } from '@/lib/http'
import { ME } from '@/lib/queryKeys'
import type { MyApplication, VacancyMatch, VacancySearch } from '../types'

const careerCenterKeys = {
  all: [...ME, 'career-center'] as const,
  search: (filters: VacancySearch) => [...ME, 'career-center', 'vacancies', filters] as const,
  applications: [...ME, 'career-center', 'applications'] as const,
}

const careerCenterApi = {
  search: (filters: VacancySearch) => http.get<VacancyMatch[]>('/career-center/vacancies', { params: filters }).then((r) => r.data),
  apply: ({ vacancyId, coverNote }: { vacancyId: number; coverNote: string }) =>
    http.post<{ id: number }>(`/career-center/vacancies/${vacancyId}/applications`, { coverNote }).then((r) => r.data),
  applications: () => http.get<MyApplication[]>('/career-center/applications').then((r) => r.data),
}

export function useVacancySearch(filters: VacancySearch) {
  return useQuery({ queryKey: careerCenterKeys.search(filters), queryFn: () => careerCenterApi.search(filters), placeholderData: keepPreviousData })
}

export function useApplyToVacancy() {
  const queryClient = useQueryClient()
  return useMutation({ mutationFn: careerCenterApi.apply, onSuccess: () => queryClient.invalidateQueries({ queryKey: careerCenterKeys.all }) })
}

export function useMyApplications() {
  return useQuery({ queryKey: careerCenterKeys.applications, queryFn: careerCenterApi.applications })
}
