import { forwardRef, type InputHTMLAttributes, type ReactNode } from 'react'
import { cn } from '@/lib/cn'

interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  label?: string
  error?: string
  leadingIcon?: ReactNode
  trailing?: ReactNode
}

export const Input = forwardRef<HTMLInputElement, InputProps>(function Input(
  { label, error, leadingIcon, trailing, id, className, ...props },
  ref,
) {
  const inputId = id ?? props.name

  return (
    <div className={className}>
      {label && (
        <label htmlFor={inputId} className="mb-1.5 block text-sm font-medium text-heading">
          {label}
        </label>
      )}
      <div className="relative">
        {leadingIcon && (
          <span className="pointer-events-none absolute inset-y-0 left-3.5 flex items-center text-muted">{leadingIcon}</span>
        )}
        <input
          ref={ref}
          id={inputId}
          aria-invalid={!!error}
          aria-describedby={error ? `${inputId}-error` : undefined}
          className={cn(
            'block h-11 w-full rounded-control border bg-[var(--color-bg-input)] px-3.5 text-sm text-heading outline-none transition-all',
            'placeholder:text-muted focus:border-primary focus:ring-4 focus:ring-primary-soft',
            error ? 'border-danger' : 'border-line-strong',
            leadingIcon && 'pl-10',
            trailing && 'pr-11',
          )}
          {...props}
        />
        {trailing && <span className="absolute inset-y-0 right-1.5 flex items-center">{trailing}</span>}
      </div>
      {error && (
        <p id={`${inputId}-error`} className="mt-1.5 text-xs font-medium text-danger">
          {error}
        </p>
      )}
    </div>
  )
})
