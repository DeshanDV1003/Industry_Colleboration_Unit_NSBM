import axios, { isAxiosError } from 'axios'
import { useAuthStore } from '@/stores/authStore'

/** Shared API client. Every feature's api module goes through this instance. */
export const http = axios.create({
  baseURL: `${import.meta.env.VITE_API_URL ?? ''}/api/v1`,
  headers: { 'Content-Type': 'application/json' },
})

http.interceptors.request.use((config) => {
  const token = useAuthStore.getState().session?.accessToken
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

http.interceptors.response.use(
  (response) => response,
  (error) => {
    const isAuthEndpoint = isAxiosError(error) && error.config?.url?.startsWith('/auth/') && !error.config.url.endsWith('/me')
    if (isAxiosError(error) && error.response?.status === 401 && !isAuthEndpoint) {
      useAuthStore.getState().signOut()
    }
    return Promise.reject(error)
  },
)

/** RFC 7807 problem body returned by the API for every error. */
interface ProblemDetails {
  title?: string
  detail?: string
  errors?: Record<string, string[]>
}

/** Turns any thrown value into a single human-readable sentence. */
export function getErrorMessage(error: unknown, fallback = 'Something went wrong. Please try again.'): string {
  if (!isAxiosError<ProblemDetails>(error)) return error instanceof Error ? error.message : fallback
  if (!error.response) return 'Unable to reach the server. Check that the API is running and try again.'

  const problem = error.response.data
  const firstFieldError = problem?.errors ? Object.values(problem.errors).flat()[0] : undefined
  return firstFieldError ?? problem?.detail ?? problem?.title ?? fallback
}
