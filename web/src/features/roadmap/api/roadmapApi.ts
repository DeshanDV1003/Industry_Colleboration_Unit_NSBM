import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { http } from '@/lib/http'
import { ME } from '@/lib/queryKeys'
import type { Roadmap } from '../types'

const roadmapKey = [...ME, 'roadmap'] as const

interface StepCompletion {
  stepId: number
  completed: boolean
}

const roadmapApi = {
  get: () => http.get<Roadmap>('/me/roadmap').then((r) => r.data),
  setCompletion: ({ stepId, completed }: StepCompletion) =>
    http.put(`/me/roadmap/steps/${stepId}/completion`, { completed }),
}

export function useRoadmap() {
  return useQuery({ queryKey: roadmapKey, queryFn: roadmapApi.get })
}

/** Ticks a step immediately in the cache, rolls back if the server rejects it. */
export function useSetStepCompletion() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: roadmapApi.setCompletion,
    onMutate: async ({ stepId, completed }) => {
      await queryClient.cancelQueries({ queryKey: roadmapKey })
      const previous = queryClient.getQueryData<Roadmap>(roadmapKey)
      if (previous) {
        const steps = previous.steps.map((s) => (s.id === stepId ? { ...s, isCompleted: completed } : s))
        const completedCount = steps.filter((s) => s.isCompleted).length
        queryClient.setQueryData<Roadmap>(roadmapKey, {
          ...previous,
          steps,
          completedCount,
          progressPercent: Math.round((100 * completedCount) / Math.max(1, steps.length)),
        })
      }
      return { previous }
    },
    onError: (_error, _input, context) => {
      if (context?.previous) queryClient.setQueryData(roadmapKey, context.previous)
    },
    onSettled: () => queryClient.invalidateQueries({ queryKey: ME }),
  })
}
