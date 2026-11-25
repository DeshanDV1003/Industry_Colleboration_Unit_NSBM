import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { ME } from '@/lib/queryKeys'
import { http } from '@/lib/http'
import type { EmailPreferences, EmailPreferencesInput } from '../types'

const EMAIL_PREFERENCES = [...ME, 'email-preferences'] as const

const mySettingsApi = {
  emailPreferences: () => http.get<EmailPreferences>('/me/email-preferences').then((r) => r.data),
  saveEmailPreferences: (input: EmailPreferencesInput) => http.put('/me/email-preferences', input),
}

export function useEmailPreferences() {
  return useQuery({ queryKey: EMAIL_PREFERENCES, queryFn: mySettingsApi.emailPreferences })
}

/** Optimistic: the checkbox changes at once, and goes back if the save fails. */
export function useSaveEmailPreferences() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: mySettingsApi.saveEmailPreferences,
    onMutate: async (input) => {
      await queryClient.cancelQueries({ queryKey: EMAIL_PREFERENCES })
      const previous = queryClient.getQueryData<EmailPreferences>(EMAIL_PREFERENCES)
      if (previous) queryClient.setQueryData<EmailPreferences>(EMAIL_PREFERENCES, { ...previous, ...input })
      return { previous }
    },
    onError: (_error, _input, context) => {
      if (context?.previous) queryClient.setQueryData(EMAIL_PREFERENCES, context.previous)
    },
    onSettled: () => queryClient.invalidateQueries({ queryKey: EMAIL_PREFERENCES }),
  })
}
