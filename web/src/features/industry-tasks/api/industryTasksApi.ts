import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { http } from '@/lib/http'
import type { IndustryTask, IndustryTaskInput, IndustryTaskListItem } from '../types'

/** Root key for every industry-task query, so authoring and moderation stay in sync after any change. */
export const INDUSTRY_TASKS = ['industry-tasks'] as const

const keys = {
  mine: [...INDUSTRY_TASKS, 'mine'] as const,
  detail: (id: number) => [...INDUSTRY_TASKS, 'detail', id] as const,
}

const industryTasksApi = {
  mine: () => http.get<IndustryTaskListItem[]>('/industry-tasks/mine').then((r) => r.data),
  detail: (id: number) => http.get<IndustryTask>(`/industry-tasks/${id}`).then((r) => r.data),
  create: (input: IndustryTaskInput) => http.post<{ id: number }>('/industry-tasks', input).then((r) => r.data),
  update: ({ id, input }: { id: number; input: IndustryTaskInput }) => http.put(`/industry-tasks/${id}`, input),
}

export function useCompanyTasks() {
  return useQuery({ queryKey: keys.mine, queryFn: industryTasksApi.mine })
}

export function useIndustryTask(id: number | null) {
  return useQuery({
    queryKey: keys.detail(id ?? 0),
    queryFn: () => industryTasksApi.detail(id!),
    enabled: id !== null,
  })
}

export function useCreateIndustryTask() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: industryTasksApi.create,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: INDUSTRY_TASKS }),
  })
}

export function useUpdateIndustryTask() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: industryTasksApi.update,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: INDUSTRY_TASKS }),
  })
}
