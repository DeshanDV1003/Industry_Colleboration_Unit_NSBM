import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import type { Role } from '@/config/roles'
import { http } from '@/lib/http'
import type { CreateUserInput, StudentImportResult, UserDetail, UserOptions, UserProfile, UserSummary } from '../types'

const USERS = ['users'] as const

const usersApi = {
  list: () => http.get<UserSummary[]>('/users').then((r) => r.data),
  get: (userId: string) => http.get<UserDetail>(`/users/${userId}`).then((r) => r.data),
  options: () => http.get<UserOptions>('/users/options').then((r) => r.data),
  create: (input: CreateUserInput) => http.post<{ userId: string }>('/users', input).then((r) => r.data),
  update: ({ userId, role, profile }: { userId: string; role: Role; profile: UserProfile }) => http.put(`/users/${userId}`, { role, profile }),
  setActive: ({ userId, active }: { userId: string; active: boolean }) => http.post(`/users/${userId}/${active ? 'activate' : 'deactivate'}`),
  resetPassword: ({ userId, newPassword }: { userId: string; newPassword: string }) => http.post(`/users/${userId}/reset-password`, { newPassword }),
  importStudents: (body: { csv: string; temporaryPassword: string }) =>
    http.post<StudentImportResult>('/users/import-students', body).then((r) => r.data),
}

/** The whole list is small enough to filter in the browser, so role and search changes are instant. */
export function useUsers() {
  return useQuery({ queryKey: [...USERS, 'list'], queryFn: usersApi.list })
}

export function useUser(userId: string | null) {
  return useQuery({ queryKey: [...USERS, 'detail', userId], queryFn: () => usersApi.get(userId!), enabled: !!userId })
}

export function useUserOptions() {
  return useQuery({ queryKey: [...USERS, 'options'], queryFn: usersApi.options })
}

function useUsersMutation<T, R>(fn: (input: T) => Promise<R>) {
  const queryClient = useQueryClient()
  return useMutation({ mutationFn: fn, onSuccess: () => queryClient.invalidateQueries({ queryKey: USERS }) })
}

export const useCreateUser = () => useUsersMutation(usersApi.create)
export const useUpdateUser = () => useUsersMutation(usersApi.update)
export const useSetUserActive = () => useUsersMutation(usersApi.setActive)
export const useImportStudents = () => useUsersMutation(usersApi.importStudents)
export const useResetPassword = () => useMutation({ mutationFn: usersApi.resetPassword })
