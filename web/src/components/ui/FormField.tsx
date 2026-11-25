import type { ReactNode } from 'react'

interface FormFieldProps {
  label: string
  htmlFor?: string
  hint?: ReactNode
  error?: string
  className?: string
  children: ReactNode
}

/** Label, control, hint and error message laid out consistently for every form control. */
export function FormField({ label, htmlFor, hint, error, className, children }: FormFieldProps) {
  return (
    <div className={className}>
      <label htmlFor={htmlFor} className="mb-1.5 block text-sm font-medium text-heading">{label}</label>
      {children}
      {error
        ? <p className="mt-1.5 text-xs font-medium text-danger">{error}</p>
        : hint && <p className="mt-1.5 text-xs text-muted">{hint}</p>}
    </div>
  )
}

