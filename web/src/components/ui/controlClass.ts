import { cn } from '@/lib/cn'

/** Shared visual style for native inputs, selects and textareas. */
export const controlClass = (invalid?: boolean) =>
  cn(
    'block w-full rounded-control border bg-[var(--color-bg-input)] px-3.5 text-sm text-heading outline-none transition-all',
    'placeholder:text-muted focus:border-primary focus:ring-4 focus:ring-primary-soft',
    invalid ? 'border-danger' : 'border-line-strong',
  )
