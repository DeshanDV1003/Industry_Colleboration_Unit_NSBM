import { create } from 'zustand'
import { persist } from 'zustand/middleware'
import type { Role } from '@/config/roles'

export interface Session {
  accessToken: string
  expiresAt: string
  userId: string
  email: string
  fullName: string
  role: Role
}

interface AuthState {
  session: Session | null
  signIn: (session: Session) => void
  signOut: () => void
}

const isExpired = (session: Session) => new Date(session.expiresAt).getTime() <= Date.now()

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      session: null,
      signIn: (session) => set({ session }),
      signOut: () => set({ session: null }),
    }),
    {
      name: 'nsbm-icu-auth',
      onRehydrateStorage: () => (state) => {
        if (state?.session && isExpired(state.session)) state.signOut()
      },
    },
  ),
)

export const useSession = () => useAuthStore((s) => s.session)
