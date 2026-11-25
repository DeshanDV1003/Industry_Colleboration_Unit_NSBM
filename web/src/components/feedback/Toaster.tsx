import { AlertCircle, CheckCircle2, Info, X } from 'lucide-react'
import { cn } from '@/lib/cn'
import { useToastStore, type ToastTone } from './toast'

const ICONS = { success: CheckCircle2, error: AlertCircle, info: Info } satisfies Record<ToastTone, unknown>
const ACCENTS: Record<ToastTone, string> = {
  success: 'border-l-success text-success',
  error: 'border-l-danger text-danger',
  info: 'border-l-primary text-primary',
}

export function Toaster() {
  const { items, dismiss } = useToastStore()

  return (
    <div aria-live="polite" className="fixed bottom-5 right-5 z-[100] flex w-[340px] flex-col gap-2">
      {items.map(({ id, message, tone }) => {
        const Icon = ICONS[tone]
        return (
          <div
            key={id}
            role={tone === 'error' ? 'alert' : 'status'}
            className={cn('animate-enter flex items-start gap-3 rounded-xl border border-l-4 border-line bg-surface px-4 py-3 shadow-pop', ACCENTS[tone])}
          >
            <Icon className="mt-0.5 size-[18px] shrink-0" aria-hidden />
            <p className="flex-1 text-sm font-medium text-heading">{message}</p>
            <button onClick={() => dismiss(id)} className="text-muted hover:text-heading" aria-label="Dismiss notification">
              <X className="size-4" />
            </button>
          </div>
        )
      })}
    </div>
  )
}
