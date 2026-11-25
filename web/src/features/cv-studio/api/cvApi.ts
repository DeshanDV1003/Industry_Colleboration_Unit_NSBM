import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { http } from '@/lib/http'
import { ME } from '@/lib/queryKeys'

/** Mirrors Features/CvStudio/Dtos/CvDto. */
export interface Cv {
  fullName: string
  email: string
  studentNo: string
  programme: string
  careerPathName: string | null
  headline: string
  summary: string
  education: string
  projects: string
  gitHubUrl: string | null
  linkedInUrl: string | null
  skills: string[]
}

export type CvInput = Pick<Cv, 'headline' | 'summary' | 'education' | 'projects' | 'gitHubUrl' | 'linkedInUrl'>

export interface CvKeywordMatch {
  skillId: number
  name: string
  alreadyListed: boolean
}

const cvKey = [...ME, 'cv'] as const

const cvApi = {
  get: () => http.get<Cv>('/me/cv').then((r) => r.data),
  save: (input: CvInput) => http.put('/me/cv', input),
  findKeywords: (text: string) => http.post<CvKeywordMatch[]>('/me/cv/keywords', { text }).then((r) => r.data),
}

export function useMyCv() {
  return useQuery({ queryKey: cvKey, queryFn: cvApi.get })
}

export function useSaveCv() {
  const queryClient = useQueryClient()
  return useMutation({ mutationFn: cvApi.save, onSuccess: () => queryClient.invalidateQueries({ queryKey: ME }) })
}

export function useFindCvKeywords() {
  return useMutation({ mutationFn: cvApi.findKeywords })
}
