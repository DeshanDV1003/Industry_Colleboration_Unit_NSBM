import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { http } from '@/lib/http'
import type { Vacancy, VacancyInput, VacancyListItem } from '../types'

/** Root key for every vacancy query, so posting, moderation and candidate views stay in sync. */
export const VACANCIES = ['vacancies'] as const

const keys = {
  mine: [...VACANCIES, 'mine'] as const,
  detail: (id: number) => [...VACANCIES, 'detail', id] as const,
}

const vacanciesApi = {
  mine: () => http.get<VacancyListItem[]>('/vacancies/mine').then((r) => r.data),
  detail: (id: number) => http.get<Vacancy>(`/vacancies/${id}`).then((r) => r.data),
  create: (input: VacancyInput) => http.post<{ id: number }>('/vacancies', input).then((r) => r.data),
  update: ({ id, input }: { id: number; input: VacancyInput }) => http.put(`/vacancies/${id}`, input),
}

export function useCompanyVacancies() {
  return useQuery({ queryKey: keys.mine, queryFn: vacanciesApi.mine })
}

export function useVacancy(id: number | null) {
  return useQuery({ queryKey: keys.detail(id ?? 0), queryFn: () => vacanciesApi.detail(id!), enabled: id !== null })
}

export function useCreateVacancy() {
  const queryClient = useQueryClient()
  return useMutation({ mutationFn: vacanciesApi.create, onSuccess: () => queryClient.invalidateQueries({ queryKey: VACANCIES }) })
}

export function useUpdateVacancy() {
  const queryClient = useQueryClient()
  return useMutation({ mutationFn: vacanciesApi.update, onSuccess: () => queryClient.invalidateQueries({ queryKey: VACANCIES }) })
}
