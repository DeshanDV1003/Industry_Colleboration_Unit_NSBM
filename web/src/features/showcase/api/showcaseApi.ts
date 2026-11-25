import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { http } from '@/lib/http'
import type { PartnerEnquiry, PartnerEnquiryInput, Showcase } from '../types'

const PARTNER_ENQUIRIES = ['partner-enquiries'] as const

const showcaseApi = {
  showcase: () => http.get<Showcase>('/showcase').then((r) => r.data),
  submitEnquiry: (input: PartnerEnquiryInput) => http.post('/showcase/partner-enquiries', input),
  enquiries: () => http.get<PartnerEnquiry[]>('/partner-enquiries').then((r) => r.data),
  markHandled: (id: number) => http.post(`/partner-enquiries/${id}/handled`),
}

/** Public: no sign-in needed. */
export function useShowcase() {
  return useQuery({ queryKey: ['showcase'], queryFn: showcaseApi.showcase })
}

export function useSubmitPartnerEnquiry() {
  return useMutation({ mutationFn: showcaseApi.submitEnquiry })
}

export function usePartnerEnquiries() {
  return useQuery({ queryKey: PARTNER_ENQUIRIES, queryFn: showcaseApi.enquiries })
}

export function useMarkEnquiryHandled() {
  const queryClient = useQueryClient()
  return useMutation({ mutationFn: showcaseApi.markHandled, onSuccess: () => queryClient.invalidateQueries({ queryKey: PARTNER_ENQUIRIES }) })
}
