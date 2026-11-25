import type { BadgeTone } from '@/components/ui/Badge'
import type { AuditCategory } from './types'

export const CATEGORY_META: Record<AuditCategory, { label: string; tone: BadgeTone }> = {
  Accounts: { label: 'Accounts', tone: 'primary' },
  Moderation: { label: 'Moderation', tone: 'accent' },
  Halls: { label: 'Halls', tone: 'success' },
  Communication: { label: 'Communication', tone: 'warning' },
  Applications: { label: 'Applications', tone: 'neutral' },
  Platform: { label: 'Platform', tone: 'danger' },
}
