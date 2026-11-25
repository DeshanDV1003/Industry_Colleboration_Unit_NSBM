import { Navigate, Outlet, useLocation } from 'react-router-dom'
import type { Role } from '@/config/roles'
import { useSession } from '@/stores/authStore'

/** Renders child routes only for a signed-in user; otherwise redirects to the login page. */
export function RequireAuth() {
  const session = useSession()
  const location = useLocation()

  if (!session) return <Navigate to="/login" replace state={{ from: location.pathname }} />
  return <Outlet />
}

/** Renders child routes only for the given roles; other signed-in users go back to their dashboard. */
export function RequireRole({ roles }: { roles: readonly Role[] }) {
  const role = useSession()?.role

  if (!role || !roles.includes(role)) return <Navigate to="/dashboard" replace />
  return <Outlet />
}
