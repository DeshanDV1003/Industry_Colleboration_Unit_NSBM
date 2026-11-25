import { cn } from '@/lib/cn'

export function Spinner({ className, label }: { className?: string; label?: string }) {
  return (
    <div role="status" className={cn('flex flex-col items-center gap-3', className)}>
      <span className="size-9 animate-spin rounded-full border-[3px] border-line border-t-primary" aria-hidden />
      {label ? <span className="text-sm text-muted">{label}</span> : <span className="sr-only">Loading</span>}
    </div>
  )
}

export function PageSpinner({ label }: { label?: string }) {
  return (
    <div className="grid min-h-[50vh] place-items-center">
      <Spinner label={label} />
    </div>
  )
}
