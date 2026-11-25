import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import type { Role } from '@/config/roles'
import { http } from '@/lib/http'
import { useAuthStore, useSession } from '@/stores/authStore'
import type { AuthResponse, CurrentUser, LoginRequest } from '../types'

export const authKeys = {
  me: ['auth', 'me'] as const,
}

const authApi = {
  login: (body: LoginRequest) => http.post<AuthResponse>('/auth/login', body).then((r) => r.data),
  demoLogin: (role: Role) => http.post<AuthResponse>('/auth/demo-login', { role }).then((r) => r.data),
  me: () => http.get<CurrentUser>('/auth/me').then((r) => r.data),
}

/** Stores the new session, drops every cached query from a previous account and opens the dashboard. */
function useStartSession() {
  const signIn = useAuthStore((s) => s.signIn)
  const queryClient = useQueryClient()
  const navigate = useNavigate()

  return (response: AuthResponse) => {
    queryClient.clear()
    signIn(response)
    navigate('/dashboard', { replace: true })
  }
}

export function useLogin() {
  const startSession = useStartSession()
  return useMutation({ mutationFn: authApi.login, onSuccess: startSession })
}

export function useDemoLogin() {
  const startSession = useStartSession()
  return useMutation({ mutationFn: authApi.demoLogin, onSuccess: startSession })
}

export function useCurrentUser() {
  const session = useSession()
  return useQuery({
    queryKey: [...authKeys.me, session?.userId],
    queryFn: authApi.me,
    enabled: !!session,
    staleTime: 10 * 60 * 1000,
  })
}

export function useSignOut() {
  const signOut = useAuthStore((s) => s.signOut)
  const queryClient = useQueryClient()
  const navigate = useNavigate()

  return () => {
    signOut()
    queryClient.clear()
    navigate('/login', { replace: true })
  }
}
