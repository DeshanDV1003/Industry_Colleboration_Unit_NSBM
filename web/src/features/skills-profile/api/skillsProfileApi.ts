import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { http } from '@/lib/http'
import { ME } from '@/lib/queryKeys'

/** Mirrors Features/StudentProfile/Dtos/MySkillProfileDto. */
export interface MySkillProfile {
  skillIds: number[]
  shareProfileWithPartners: boolean
}

const skillsProfileApi = {
  get: () => http.get<MySkillProfile>('/me/profile/skills').then((r) => r.data),
  updateSkills: (skillIds: number[]) => http.put('/me/profile/skills', { skillIds }),
  setSharing: (share: boolean) => http.put('/me/profile/sharing', { share }),
}

export function useMySkillProfile() {
  return useQuery({ queryKey: [...ME, 'skill-profile'], queryFn: skillsProfileApi.get })
}

/** Listed skills change readiness and match results, so every query about the student is refreshed. */
export function useUpdateMySkills() {
  const queryClient = useQueryClient()
  return useMutation({ mutationFn: skillsProfileApi.updateSkills, onSuccess: () => queryClient.invalidateQueries({ queryKey: ME }) })
}

export function useSetProfileSharing() {
  const queryClient = useQueryClient()
  return useMutation({ mutationFn: skillsProfileApi.setSharing, onSuccess: () => queryClient.invalidateQueries({ queryKey: ME }) })
}
