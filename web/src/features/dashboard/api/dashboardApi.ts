import { useQuery } from '@tanstack/react-query'
import { http } from '@/lib/http'
import { ME } from '@/lib/queryKeys'
import type { StudentOverview } from '../types'

const dashboardApi = {
  studentOverview: () => http.get<StudentOverview>('/dashboard/student').then((r) => r.data),
}

export function useStudentOverview() {
  return useQuery({ queryKey: [...ME, 'dashboard', 'student'], queryFn: dashboardApi.studentOverview })
}
