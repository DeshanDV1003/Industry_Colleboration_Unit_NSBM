import { keepPreviousData, useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { http } from '@/lib/http'
import { ME } from '@/lib/queryKeys'
import type { KnowledgeCheck, TaskAttempt, TaskLibraryFilters, TaskLibraryItem, Workbench } from '../types'

const practiceKeys = {
  all: [...ME, 'practice'] as const,
  library: (filters: TaskLibraryFilters) => [...ME, 'practice', 'library', filters] as const,
  workbench: (taskId: number) => [...ME, 'practice', 'workbench', taskId] as const,
  knowledgeCheck: (taskId: number) => [...ME, 'practice', 'knowledge-check', taskId] as const,
}

const practiceApi = {
  library: (filters: TaskLibraryFilters) => http.get<TaskLibraryItem[]>('/practice/tasks', { params: filters }).then((r) => r.data),
  workbench: (taskId: number) => http.get<Workbench>(`/practice/tasks/${taskId}`).then((r) => r.data),
  submitAttempt: ({ taskId, code }: { taskId: number; code: string }) =>
    http.post<TaskAttempt>(`/practice/tasks/${taskId}/attempts`, { code }).then((r) => r.data),
  knowledgeCheck: (taskId: number) => http.get<KnowledgeCheck>(`/practice/tasks/${taskId}/knowledge-check`).then((r) => r.data),
  submitKnowledgeCheck: ({ taskId, answers }: { taskId: number; answers: { questionId: number; text: string }[] }) =>
    http.put(`/practice/tasks/${taskId}/knowledge-check`, { answers }),
}

export function useTaskLibrary(filters: TaskLibraryFilters) {
  return useQuery({ queryKey: practiceKeys.library(filters), queryFn: () => practiceApi.library(filters), placeholderData: keepPreviousData })
}

export function useWorkbench(taskId: number) {
  return useQuery({ queryKey: practiceKeys.workbench(taskId), queryFn: () => practiceApi.workbench(taskId) })
}

export function useSubmitAttempt() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: practiceApi.submitAttempt,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: practiceKeys.all }),
  })
}

export function useKnowledgeCheck(taskId: number) {
  return useQuery({ queryKey: practiceKeys.knowledgeCheck(taskId), queryFn: () => practiceApi.knowledgeCheck(taskId) })
}

export function useSubmitKnowledgeCheck() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: practiceApi.submitKnowledgeCheck,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: practiceKeys.all }),
  })
}
