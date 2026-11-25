import { useQuery } from '@tanstack/react-query'
import { http } from '@/lib/http'
import { ME } from '@/lib/queryKeys'
import type { SkillEvidenceRow } from '@/types/opportunities'

/** Mirrors Features/Readiness/Dtos/ReadinessDto. `targetName` is null until a career path is chosen. */
export interface Readiness {
  targetName: string | null
  companyName: string | null
  vacancyId: number | null
  matchPercent: number
  required: SkillEvidenceRow[]
  preferred: SkillEvidenceRow[]
}

export function useReadiness(vacancyId: number | null) {
  return useQuery({
    queryKey: [...ME, 'readiness', vacancyId],
    queryFn: () => http.get<Readiness>('/me/readiness', { params: vacancyId ? { vacancyId } : {} }).then((r) => r.data),
  })
}
