import { useEffect, useRef, useState } from 'react'
import { Link } from 'react-router-dom'
import { Bell } from 'lucide-react'
import { useMarkAllNotificationsRead, useNotifications } from '../api/notificationsApi'
import { NotificationItem } from './NotificationItem'

const SHOWN = 8

/** Top-bar bell with an unread count and the latest notifications. */
export function NotificationBell({ buttonClassName }: { buttonClassName: string }) {
  const { data } = useNotifications(SHOWN)
  const markAll = useMarkAllNotificationsRead()
  const [open, setOpen] = useState(false)
  const ref = useRef<HTMLDivElement>(null)
  const unread = data?.unreadCount ?? 0

  useEffect(() => {
    if (!open) return
    const close = (e: MouseEvent) => !ref.current?.contains(e.target as Node) && setOpen(false)
    document.addEventListener('mousedown', close)
    return () => document.removeEventListener('mousedown', close)
  }, [open])

  return (
    <div ref={ref} className="relative">
      <button
        className={`${buttonClassName} relative`}
        onClick={() => setOpen((v) => !v)}
        aria-expanded={open}
        aria-haspopup="dialog"
        aria-label={unread ? `Notifications, ${unread} unread` : 'Notifications'}
      >
        <Bell className="size-5" />
        {unread > 0 && (
          <span className="absolute right-1.5 top-1.5 grid min-w-4 place-items-center rounded-full bg-danger px-1 text-[10px] font-bold leading-4 text-white">
            {unread > 9 ? '9+' : unread}
          </span>
        )}
      </button>

      {open && (
        <div role="dialog" aria-label="Notifications" className="animate-pop absolute right-0 z-50 mt-2 w-[22rem] rounded-xl border border-line bg-surface shadow-pop">
          <div className="flex items-center justify-between border-b border-line px-4 py-3">
            <p className="font-semibold text-heading">Notifications</p>
            {unread > 0 && (
              <button type="button" className="text-xs font-semibold text-primary" disabled={markAll.isPending} onClick={() => markAll.mutate()}>
                Mark all read
              </button>
            )}
          </div>
          <div className="max-h-96 space-y-0.5 overflow-y-auto p-1.5">
            {data?.items.length
              ? data.items.map((n) => <NotificationItem key={n.id} notification={n} onOpen={() => setOpen(false)} />)
              : <p className="px-3 py-6 text-center text-sm text-muted">You're all caught up.</p>}
          </div>
          <Link to="/notifications" onClick={() => setOpen(false)} className="block border-t border-line px-4 py-2.5 text-center text-sm font-semibold">
            See all and your week
          </Link>
        </div>
      )}
    </div>
  )
}
