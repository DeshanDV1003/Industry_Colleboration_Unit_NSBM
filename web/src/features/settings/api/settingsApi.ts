import { useMutation, useQueryClient } from '@tanstack/react-query'
import { http } from '@/lib/http'

const settingsApi = {
  updateProfile: (designation: string) => http.put('/settings/profile', { designation }),
  resetDemo: () => http.post('/settings/demo-reset'),
  sendTestEmail: () => http.post<{ sentTo: string }>('/settings/test-email').then((r) => r.data),
}

/** The designation appears in the user menu and dashboard, so all cached data is refreshed. */
export function useUpdateAdminProfile() {
  const queryClient = useQueryClient()
  return useMutation({ mutationFn: settingsApi.updateProfile, onSuccess: () => queryClient.invalidateQueries() })
}

/** A reset changes almost every record, so every cached query is dropped. */
export function useResetDemoData() {
  const queryClient = useQueryClient()
  return useMutation({ mutationFn: settingsApi.resetDemo, onSuccess: () => queryClient.invalidateQueries() })
}

export function useSendTestEmail() {
  return useMutation({ mutationFn: settingsApi.sendTestEmail })
}
