import { Link, type LinkProps } from 'react-router-dom'
import { cn } from '@/lib/cn'

type Variant = 'primary' | 'secondary'

const VARIANTS: Record<Variant, string> = {
  primary: 'bg-primary text-white hover:bg-primary-hover hover:text-white',
  secondary: 'border border-line-strong bg-surface text-heading hover:bg-surface-subtle hover:text-heading',
}

/** A router link styled like a Button, for navigation actions. */
export function LinkButton({ variant = 'primary', className, ...props }: LinkProps & { variant?: Variant }) {
  return (
    <Link
      className={cn('inline-flex h-10 items-center justify-center gap-2 whitespace-nowrap rounded-control px-4 text-sm font-semibold transition-all', VARIANTS[variant], className)}
      {...props}
    />
  )
}
