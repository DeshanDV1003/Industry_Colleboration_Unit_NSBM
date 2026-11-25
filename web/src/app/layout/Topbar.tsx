import { Menu, Moon, PanelLeft, Sun } from 'lucide-react'
import { Badge } from '@/components/ui/Badge'
import { ROLE_META } from '@/config/roles'
import { NotificationBell } from '@/features/notifications'
import { useSession } from '@/stores/authStore'
import { useThemeStore } from '@/stores/themeStore'
import { useUiStore } from '@/stores/uiStore'
import { UserMenu } from './UserMenu'

const iconButton = 'grid size-10 place-items-center rounded-xl text-muted transition-colors hover:bg-surface-subtle hover:text-heading'

export function Topbar() {
  const role = useSession()?.role
  const { theme, toggleTheme } = useThemeStore()
  const { toggleSidebar, setMobileNavOpen } = useUiStore()

  return (
    <header className="sticky top-0 z-30 flex h-[72px] items-center gap-3 border-b border-line bg-surface px-4 lg:px-8">
      <button className={`${iconButton} lg:hidden`} onClick={() => setMobileNavOpen(true)} aria-label="Open navigation">
        <Menu className="size-5" />
      </button>
      <button className={`${iconButton} hidden lg:grid`} onClick={toggleSidebar} aria-label="Collapse or expand navigation">
        <PanelLeft className="size-5" />
      </button>

      {role && <Badge tone="accent">{ROLE_META[role].portal}</Badge>}

      <div className="ml-auto flex items-center gap-1.5">
        <button
          className={iconButton}
          onClick={toggleTheme}
          aria-label={theme === 'light' ? 'Switch to dark theme' : 'Switch to light theme'}
        >
          {theme === 'light' ? <Moon className="size-5" /> : <Sun className="size-5" />}
        </button>
        <NotificationBell buttonClassName={iconButton} />
        <UserMenu />
      </div>
    </header>
  )
}
