import { ROLES, ROLE_META } from '@/config/roles'
import { toast } from '@/components/feedback/toast'
import { getErrorMessage } from '@/lib/http'
import { cn } from '@/lib/cn'
import { useDemoLogin } from '../api/authApi'

/** One-click entry into each portal with its fictional demo account. Not a real login. */
export function DemoAccessPanel() {
  const demoLogin = useDemoLogin()

  return (
    <section aria-labelledby="demo-access">
      <div className="flex items-center gap-3">
        <span className="h-px flex-1 bg-line" />
        <h2 id="demo-access" className="text-xs font-semibold uppercase tracking-wider text-muted">
          Explore a demo portal
        </h2>
        <span className="h-px flex-1 bg-line" />
      </div>

      <div className="mt-4 grid grid-cols-2 gap-2.5">
        {ROLES.map((role) => {
          const { icon: Icon, label } = ROLE_META[role]
          const pending = demoLogin.isPending && demoLogin.variables === role
          return (
            <button
              key={role}
              type="button"
              disabled={demoLogin.isPending}
              onClick={() => demoLogin.mutate(role, { onError: (e) => toast.error(getErrorMessage(e)) })}
              className={cn(
                'flex items-center gap-2.5 rounded-control border border-line bg-surface px-3 py-2.5 text-left text-sm font-medium text-heading transition-colors',
                'hover:border-primary hover:bg-primary-soft disabled:cursor-wait disabled:opacity-60',
              )}
            >
              <span className="grid size-8 place-items-center rounded-lg bg-primary-soft text-primary">
                <Icon className={cn('size-4', pending && 'animate-pulse')} />
              </span>
              {label}
            </button>
          )
        })}
      </div>
      <p className="mt-3 text-center text-xs text-muted">Demo accounts use fictional data only.</p>
    </section>
  )
}
