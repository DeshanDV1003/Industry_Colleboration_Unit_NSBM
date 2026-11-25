import { Repeat } from 'lucide-react'
import { ROLES, ROLE_META, type Role } from '@/config/roles'
import { toast } from '@/components/feedback/toast'
import { getErrorMessage } from '@/lib/http'
import { useSession } from '@/stores/authStore'
import { useDemoLogin } from '../api/authApi'

/** Sidebar control that swaps the session to another portal's demo account. It is not a real login. */
export function DemoRoleSwitcher() {
  const session = useSession()
  const demoLogin = useDemoLogin()

  const switchTo = (role: Role) => {
    if (role === session?.role) return
    demoLogin.mutate(role, {
      onSuccess: () => toast.info(`Switched to the ${ROLE_META[role].portal.toLowerCase()}`),
      onError: (e) => toast.error(getErrorMessage(e)),
    })
  }

  return (
    <label className="block">
      <span className="mb-1.5 flex items-center gap-1.5 text-[11px] font-semibold uppercase tracking-wider text-muted">
        <Repeat className="size-3.5" /> Demo role switcher
      </span>
      <select
        value={session?.role}
        disabled={demoLogin.isPending}
        onChange={(e) => switchTo(e.target.value as Role)}
        className="h-9 w-full rounded-control border border-line-strong bg-[var(--color-bg-input)] px-2.5 text-sm font-medium text-heading outline-none focus:border-primary"
      >
        {ROLES.map((role) => (
          <option key={role} value={role}>
            {ROLE_META[role].portal}
          </option>
        ))}
      </select>
    </label>
  )
}
