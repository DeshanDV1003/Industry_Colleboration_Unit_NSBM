import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { VACANCIES } from '@/features/vacancies'
import { http } from '@/lib/http'
import type { ApplicationStatus } from '@/types/opportunities'
import type { CandidateEvidence, CompanyApplication, SharedProfile } from '../types'

const CANDIDATES = ['candidates'] as const

const candidatesApi = {
  applications: (vacancyId: number) =>
    http.get<CompanyApplication[]>('/candidates/applications', { params: { vacancyId } }).then((r) => r.data),
  sharedProfiles: () => http.get<SharedProfile[]>('/candidates/shared-profiles').then((r) => r.data),
  evidence: (studentId: number, vacancyId: number) =>
    http.get<CandidateEvidence>('/candidates/evidence', { params: { studentId, vacancyId } }).then((r) => r.data),
  updateStatus: ({ applicationId, status }: { applicationId: number; status: ApplicationStatus }) =>
    http.put(`/candidates/applications/${applicationId}/status`, { status }),
}

export function useCompanyApplications(vacancyId: number | null) {
  return useQuery({
    queryKey: [...CANDIDATES, 'applications', vacancyId],
    queryFn: () => candidatesApi.applications(vacancyId!),
    enabled: vacancyId !== null,
  })
}

export function useSharedProfiles() {
  return useQuery({ queryKey: [...CANDIDATES, 'shared-profiles'], queryFn: candidatesApi.sharedProfiles })
}

export function useCandidateEvidence(studentId: number | null, vacancyId: number | null) {
  return useQuery({
    queryKey: [...CANDIDATES, 'evidence', studentId, vacancyId],
    queryFn: () => candidatesApi.evidence(studentId!, vacancyId!),
    enabled: studentId !== null && vacancyId !== null,
  })
}

/** Status changes affect the applicant lists, the evidence panel and the vacancy applicant counts. */
export function useUpdateApplicationStatus() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: candidatesApi.updateStatus,
    onSuccess: () => Promise.all([
      queryClient.invalidateQueries({ queryKey: CANDIDATES }),
      queryClient.invalidateQueries({ queryKey: VACANCIES }),
    ]),
  })
}
