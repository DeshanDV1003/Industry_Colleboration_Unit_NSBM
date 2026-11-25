import { useEffect, useRef, useState } from 'react'
import { Link } from 'react-router-dom'
import { ChevronDown, KeyRound, LogOut, Settings } from 'lucide-react'
import { Avatar } from '@/components/ui/Avatar'
import { ROLE_META } from '@/config/roles'
import { ChangePasswordDialog, useCurrentUser, useSignOut } from '@/features/auth'
import { useSession } from '@/stores/authStore'

export function UserMenu() {
  const session = useSession()
  const { data: me } = useCurrentUser()
  const signOut = useSignOut()
  const [open, setOpen] = useState(false)
  const [changingPassword, setChangingPassword] = useState(false)
  const ref = useRef<HTMLDivElement>(null)

  useEffect(() => {
    if (!open) return
    const close = (e: MouseEvent) => !ref.current?.contains(e.target as Node) && setOpen(false)
    document.addEventListener('mousedown', close)
    return () => document.removeEventListener('mousedown', close)
  }, [open])

  if (!session) return null

  return (
    <div ref={ref} className="relative">
      <button
        onClick={() => setOpen((v) => !v)}
        aria-expanded={open}
        aria-haspopup="menu"
        className="flex items-center gap-2.5 rounded-xl py-1.5 pl-1.5 pr-2.5 hover:bg-surface-subtle"
      >
        <Avatar name={session.fullName} />
        <span className="hidden text-left leading-tight sm:block">
          <span className="block text-sm font-semibold text-heading">{session.fullName}</span>
          <span className="block text-xs text-muted">{ROLE_META[session.role].label}</span>
        </span>
        <ChevronDown className="size-4 text-muted" />
      </button>

      {open && (
        <div role="menu" className="animate-pop absolute right-0 z-50 mt-2 w-72 rounded-xl border border-line bg-surface p-1.5 shadow-pop">
          <div className="px-3 py-2.5">
            <p className="font-semibold text-heading">{session.fullName}</p>
            <p className="truncate text-xs text-muted">{session.email}</p>
            {me?.headline && <p className="mt-2 text-xs text-body">{me.headline}</p>}
            {me?.organisation && <p className="text-xs text-muted">{me.organisation}</p>}
          </div>
          <div className="my-1 border-t border-line" />
          <Link
            role="menuitem"
            to="/my-settings"
            onClick={() => setOpen(false)}
            className="flex w-full items-center gap-2.5 rounded-lg px-3 py-2 text-sm text-body hover:bg-surface-subtle hover:text-heading"
          >
            <Settings className="size-4" /> My settings
          </Link>
          <button
            role="menuitem"
            onClick={() => { setOpen(false); setChangingPassword(true) }}
            className="flex w-full items-center gap-2.5 rounded-lg px-3 py-2 text-sm text-body hover:bg-surface-subtle hover:text-heading"
          >
            <KeyRound className="size-4" /> Change password
          </button>
          <button
            role="menuitem"
            onClick={signOut}
            className="flex w-full items-center gap-2.5 rounded-lg px-3 py-2 text-sm text-body hover:bg-surface-subtle hover:text-heading"
          >
            <LogOut className="size-4" /> Sign out
          </button>
        </div>
      )}
      {changingPassword && <ChangePasswordDialog onClose={() => setChangingPassword(false)} />}
    </div>
  )
}
