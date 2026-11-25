import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { http } from '@/lib/http'
import { ME } from '@/lib/queryKeys'
import type { Baseline, BaselineAnswerInput, BaselineResult } from '../types'

const baselineKey = [...ME, 'baseline'] as const

const baselineApi = {
  get: () => http.get<Baseline>('/me/baseline').then((r) => r.data),
  submit: (answers: BaselineAnswerInput[]) =>
    http.post<BaselineResult>('/me/baseline/attempts', { answers }).then((r) => r.data),
}

export function useBaseline() {
  return useQuery({ queryKey: baselineKey, queryFn: baselineApi.get })
}

export function useSubmitBaseline() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: baselineApi.submit,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ME }),
  })
}
