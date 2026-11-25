import type { Role } from '@/config/roles'

export interface LoginRequest {
  email: string
  password: string
}

/** Mirrors Features/Auth/Dtos/AuthResponseDto. */
export interface AuthResponse {
  accessToken: string
  expiresAt: string
  userId: string
  email: string
  fullName: string
  role: Role
}

/** Mirrors Features/Auth/Dtos/CurrentUserDto. */
export interface CurrentUser {
  userId: string
  email: string
  fullName: string
  role: Role
  profileId: number | null
  headline: string | null
  organisation: string | null
}
