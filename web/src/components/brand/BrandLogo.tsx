import { BRAND } from '@/config/brand'
import { cn } from '@/lib/cn'
import { useThemeStore } from '@/stores/themeStore'

interface BrandLogoProps {
  /** Forces the white logo (e.g. on the brand gradient). Defaults to white in dark theme. */
  inverted?: boolean
  className?: string
}

export function BrandLogo({ inverted, className }: BrandLogoProps) {
  const theme = useThemeStore((s) => s.theme)
  const white = inverted ?? theme === 'dark'

  return (
    <img
      src={white ? BRAND.logo.white : BRAND.logo.color}
      alt={BRAND.university}
      draggable={false}
      className={cn('h-10 w-auto shrink-0 select-none', className)}
    />
  )
}
