import { Bug, Cloud, Code2, Compass, Infinity as InfinityIcon, type LucideIcon } from 'lucide-react'

const ICONS: Record<string, LucideIcon> = {
  'software-engineering': Code2,
  'quality-assurance': Bug,
  devops: InfinityIcon,
  'cloud-engineering': Cloud,
}

/** The icon that represents a career path, by slug. Falls back to a compass for unknown paths. */
export function CareerIcon({ slug, className }: { slug: string; className?: string }) {
  const Icon = ICONS[slug] ?? Compass
  return <Icon className={className} aria-hidden />
}
