import type { Role } from '@/config/roles'

/** Mirrors Features/Users/Dtos/UserDto. */
export interface UserSummary {
  userId: string
  role: Role
  fullName: string
  email: string
  isActive: boolean
  summary: string
}

/** Mirrors Features/Users/Models/UserProfileInput. Only the fields of the account's role are used. */
export interface UserProfile {
  firstName: string
  lastName: string
  studentNo: string | null
  programme: string | null
  academicYear: number | null
  batch: string | null
  staffNo: string | null
  designation: string | null
  department: string | null
  companyId: number | null
  jobTitle: string | null
  office: string | null
}

/** Mirrors Features/Users/Dtos/UserDetailDto. */
export interface UserDetail {
  userId: string
  role: Role
  email: string
  isActive: boolean
  profile: UserProfile
}

export interface CreateUserInput {
  role: Role
  email: string
  password: string
  profile: UserProfile
}

export interface UserOptions {
  companies: { id: number; name: string }[]
  programmes: string[]
}

/** Mirrors Features/Users/Dtos/StudentImportResultDto. */
export interface StudentImportResult {
  created: number
  errors: { row: number; studentNo: string | null; message: string }[]
}
