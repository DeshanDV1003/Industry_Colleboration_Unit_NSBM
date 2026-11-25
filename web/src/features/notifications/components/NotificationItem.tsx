import { useNavigate } from 'react-router-dom'
import { cn } from '@/lib/cn'
import { formatDateTime } from '@/lib/format'
import { useMarkNotificationRead } from '../api/notificationsApi'
import { NOTIFICATION_ICON } from '../config'
import type { AppNotification } from '../types'

/** One notification. Opening it marks it read and goes to its page. */
export function NotificationItem({ notification, onOpen }: { notification: AppNotification; onOpen?: () => void }) {
  const navigate = useNavigate()
  const markRead = useMarkNotificationRead()
  const Icon = NOTIFICATION_ICON[notification.kind]

  const open = () => {
    if (!notification.isRead) markRead.mutate(notification.id)
    onOpen?.()
    if (notification.link) navigate(notification.link)
  }

  return (
    <button
      type="button"
      onClick={open}
      className={cn('flex w-full gap-3 rounded-lg px-3 py-2.5 text-left transition-colors hover:bg-surface-subtle', !notification.isRead && 'bg-primary-soft')}
    >
      <span className="grid size-8 shrink-0 place-items-center rounded-full bg-surface text-primary shadow-card"><Icon className="size-4" /></span>
      <span className="min-w-0 flex-1">
        <span className={cn('block text-sm text-heading', !notification.isRead && 'font-semibold')}>{notification.title}</span>
        <span className="block text-xs text-body">{notification.body}</span>
        <span className="mt-0.5 block text-[11px] text-muted">{formatDateTime(notification.createdAt)}</span>
      </span>
      {!notification.isRead && <span aria-label="Unread" className="mt-1.5 size-2 shrink-0 rounded-full bg-primary" />}
    </button>
  )
}
