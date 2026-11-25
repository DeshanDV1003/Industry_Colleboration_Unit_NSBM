import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { http } from '@/lib/http'
import { CAREER_PATHS, ME } from '@/lib/queryKeys'
import type { CareerPathDetail, CareerPathSummary, Skill } from '../types'

const careersKeys = {
  list: [...CAREER_PATHS, 'list'] as const,
  detail: (slug: string) => [...CAREER_PATHS, 'detail', slug] as const,
  skills: ['skills'] as const,
}

const careersApi = {
  list: () => http.get<CareerPathSummary[]>('/career-paths').then((r) => r.data),
  detail: (slug: string) => http.get<CareerPathDetail>(`/career-paths/${slug}`).then((r) => r.data),
  select: (id: number) => http.post(`/career-paths/${id}/selection`),
  skills: () => http.get<Skill[]>('/skills').then((r) => r.data),
}

export function useCareerPaths() {
  return useQuery({ queryKey: careersKeys.list, queryFn: careersApi.list })
}

/** The skills catalogue rarely changes, so it is cached for the whole session. */
export function useSkills() {
  return useQuery({ queryKey: careersKeys.skills, queryFn: careersApi.skills, staleTime: Infinity })
}

export function useCareerPath(slug: string | null) {
  return useQuery({
    queryKey: careersKeys.detail(slug ?? ''),
    queryFn: () => careersApi.detail(slug!),
    enabled: !!slug,
  })
}

/** Choosing a path changes every learning page, so all of the user's own data is refreshed. */
export function useSelectCareerPath() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: careersApi.select,
    onSuccess: () => Promise.all([
      queryClient.invalidateQueries({ queryKey: CAREER_PATHS }),
      queryClient.invalidateQueries({ queryKey: ME }),
    ]),
  })
}
