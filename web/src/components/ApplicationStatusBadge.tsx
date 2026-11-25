import { Badge } from '@/components/ui/Badge'
import { APPLICATION_STATUS_META } from '@/config/opportunities'
import type { ApplicationStatus } from '@/types/opportunities'

export function ApplicationStatusBadge({ status }: { status: ApplicationStatus }) {
  const { label, tone } = APPLICATION_STATUS_META[status]
  return <Badge tone={tone}>{label}</Badge>
}
