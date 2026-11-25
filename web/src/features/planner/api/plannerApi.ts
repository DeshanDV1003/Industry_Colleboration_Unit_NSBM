import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { http } from '@/lib/http'
import { ME } from '@/lib/queryKeys'

export type PlannerItemKind = 'Goal' | 'Task' | 'Deadline'

/** Mirrors Features/Planner/Dtos/PlannerItemDto. */
export interface PlannerItem {
  id: number
  kind: PlannerItemKind
  title: string
  notes: string | null
  dueDate: string | null
  isDone: boolean
  completedAt: string | null
}

export interface PlannerItemInput {
  kind: PlannerItemKind
  title: string
  notes: string | null
  dueDate: string | null
}

const plannerKey = [...ME, 'planner'] as const

const plannerApi = {
  list: () => http.get<PlannerItem[]>('/me/planner').then((r) => r.data),
  add: (input: PlannerItemInput) => http.post<{ id: number }>('/me/planner', input).then((r) => r.data),
  setDone: ({ id, done }: { id: number; done: boolean }) => http.put(`/me/planner/${id}/done`, { done }),
  remove: (id: number) => http.delete(`/me/planner/${id}`),
}

export function usePlanner() {
  return useQuery({ queryKey: plannerKey, queryFn: plannerApi.list })
}

function usePlannerMutation<T>(fn: (input: T) => Promise<unknown>) {
  const queryClient = useQueryClient()
  return useMutation({ mutationFn: fn, onSuccess: () => queryClient.invalidateQueries({ queryKey: plannerKey }) })
}

export const useAddPlannerItem = () => usePlannerMutation(plannerApi.add)
export const useSetPlannerItemDone = () => usePlannerMutation(plannerApi.setDone)
export const useDeletePlannerItem = () => usePlannerMutation(plannerApi.remove)
