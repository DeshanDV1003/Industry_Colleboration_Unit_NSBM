import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { http } from '@/lib/http'
import { ME } from '@/lib/queryKeys'
import type { SprintBoard, SprintStatus } from '../types'

const boardKey = [...ME, 'sprint'] as const

export function useSprintBoard() {
  return useQuery({ queryKey: boardKey, queryFn: () => http.get<SprintBoard>('/me/sprint').then((r) => r.data) })
}

export function useMoveSprintItem() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ id, status }: { id: number; status: SprintStatus }) => http.put(`/me/sprint/items/${id}`, { status }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: boardKey }),
  })
}
