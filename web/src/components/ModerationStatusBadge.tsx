import { Badge } from '@/components/ui/Badge'
import { MODERATION_STATUS_META } from '@/config/tasks'
import type { ModerationStatus } from '@/types/tasks'

export function ModerationStatusBadge({ status }: { status: ModerationStatus }) {
  const { label, tone } = MODERATION_STATUS_META[status]
  return <Badge tone={tone}>{label}</Badge>
}
