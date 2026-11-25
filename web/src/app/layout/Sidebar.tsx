import { NavLink } from 'react-router-dom'
import { BrandLogo } from '@/components/brand/BrandLogo'
import { Badge } from '@/components/ui/Badge'
import { BRAND } from '@/config/brand'
import { NAVIGATION, type NavItem } from '@/config/navigation'
import { DemoRoleSwitcher } from '@/features/auth'
import { cn } from '@/lib/cn'
import { useSession } from '@/stores/authStore'
import { useUiStore } from '@/stores/uiStore'

const DEMO_MODE = import.meta.env.VITE_DEMO_MODE === 'true'

const itemBase = 'flex h-10 items-center gap-3 rounded-xl px-3 text-sm font-medium transition-colors'

function SidebarLink({ item, wide }: { item: NavItem; wide: boolean }) {
  const { icon: Icon, label, to, ready } = item

  if (!ready) {
    return (
      <span title={`${label} — coming soon`} className={cn(itemBase, 'cursor-not-allowed text-muted opacity-60')}>
        <Icon className="size-[18px] shrink-0" />
        {wide && (
          <>
            <span className="flex-1 truncate">{label}</span>
            <Badge className="px-2 text-[10px]">Soon</Badge>
          </>
        )}
      </span>
    )
  }

  return (
    <NavLink
      to={to}
      end
      title={label}
      className={({ isActive }) =>
        cn(itemBase, isActive ? 'bg-primary-soft text-primary' : 'text-body hover:bg-surface-subtle hover:text-heading')
      }
    >
      <Icon className="size-[18px] shrink-0" />
      {wide && <span className="truncate">{label}</span>}
    </NavLink>
  )
}

export function Sidebar() {
  const role = useSession()?.role
  const { sidebarCollapsed, mobileNavOpen, setMobileNavOpen } = useUiStore()
  const wide = !sidebarCollapsed || mobileNavOpen

  if (!role) return null

  return (
    <aside
      className={cn(
        'fixed top-0 z-40 flex h-screen shrink-0 flex-col border-r border-line bg-surface-sidebar transition-all duration-200 lg:sticky',
        mobileNavOpen ? 'translate-x-0' : '-translate-x-full lg:translate-x-0',
        wide ? 'w-[272px]' : 'w-[76px]',
      )}
    >
      <div className={cn('flex h-[72px] shrink-0 items-center gap-3 border-b border-line', wide ? 'px-5' : 'justify-center px-2')}>
        <BrandLogo className={wide ? 'h-10' : 'h-7'} />
        {wide && (
          <div className="min-w-0 border-l border-line pl-3 leading-tight">
            <p className="text-[12.5px] font-bold text-heading">{BRAND.unit}</p>
            <p className="mt-0.5 truncate text-[11px] text-muted">{BRAND.faculty}</p>
          </div>
        )}
      </div>

      <nav className="flex-1 space-y-5 overflow-y-auto px-3 py-5" onClick={() => setMobileNavOpen(false)}>
        {NAVIGATION[role].map((section, index) => (
          <div key={section.title ?? index}>
            {section.title &&
              (wide ? (
                <p className="mb-1.5 px-3 text-[11px] font-bold uppercase tracking-wider text-muted">{section.title}</p>
              ) : (
                <div className="mx-3 mb-2 border-t border-line" />
              ))}
            <div className="space-y-0.5">
              {section.items.map((item) => (
                <SidebarLink key={item.to} item={item} wide={wide} />
              ))}
            </div>
          </div>
        ))}
      </nav>

      {DEMO_MODE && wide && (
        <div className="border-t border-line p-4">
          <DemoRoleSwitcher />
        </div>
      )}
    </aside>
  )
}
