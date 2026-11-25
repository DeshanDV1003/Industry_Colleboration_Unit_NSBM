import { useState, type ReactNode } from 'react'
import { ShieldCheck } from 'lucide-react'
import { ModerationStatusBadge } from '@/components/ModerationStatusBadge'
import { Button } from '@/components/ui/Button'
import { Card } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { SegmentedControl } from '@/components/ui/SegmentedControl'
import { MODERATION_STATUSES, MODERATION_STATUS_META } from '@/config/tasks'
import { formatDate } from '@/lib/format'
import type { ModerationStatus } from '@/types/tasks'

/** The fields the board needs from any moderated list item. */
export interface ModeratedItem {
  id: number
  title: string
  companyName: string
  status: ModerationStatus
  createdAt: string
  reviewedAt: string | null
  reviewNote: string | null
}

interface ModerationBoardProps<T extends ModeratedItem> {
  items: T[]
  emptyHint: string
  renderTags: (item: T) => ReactNode
  onOpen: (item: T) => void
}

/** Status tabs and a card grid shared by every approval queue. Pending items are shown oldest first. */
export function ModerationBoard<T extends ModeratedItem>({ items, emptyHint, renderTags, onOpen }: ModerationBoardProps<T>) {
  const [status, setStatus] = useState<ModerationStatus>('Pending')

  const visible = items
    .filter((i) => i.status === status)
    .sort((a, b) => status === 'Pending'
      ? a.createdAt.localeCompare(b.createdAt)
      : (b.reviewedAt ?? b.createdAt).localeCompare(a.reviewedAt ?? a.createdAt))

  return (
    <>
      <div className="mb-4">
        <SegmentedControl
          ariaLabel="Filter by status"
          value={status}
          onChange={setStatus}
          options={MODERATION_STATUSES.map((s) => ({ value: s, label: MODERATION_STATUS_META[s].label, count: items.filter((i) => i.status === s).length }))}
        />
      </div>

      {visible.length === 0 ? (
        <EmptyState
          icon={ShieldCheck}
          title={status === 'Pending' ? 'Nothing waiting for review' : `No ${MODERATION_STATUS_META[status].label.toLowerCase()} items`}
          description={status === 'Pending' ? emptyHint : undefined}
        />
      ) : (
        <div className="grid gap-4 lg:grid-cols-2">
          {visible.map((item) => (
            <Card key={item.id} className="flex flex-col p-5">
              <div className="flex items-start justify-between gap-3">
                <div className="min-w-0">
                  <p className="text-xs font-semibold text-muted">{item.companyName}</p>
                  <h3 className="mt-0.5 font-semibold">{item.title}</h3>
                </div>
                <ModerationStatusBadge status={item.status} />
              </div>
              <div className="mt-3 flex flex-wrap gap-1.5">{renderTags(item)}</div>
              {item.reviewNote && <p className="mt-3 text-sm text-muted">Note: {item.reviewNote}</p>}
              <div className="mt-auto flex items-center justify-between pt-4">
                <span className="text-xs text-muted">
                  {item.status === 'Pending' ? `Submitted ${formatDate(item.createdAt)}` : `Reviewed ${formatDate(item.reviewedAt ?? item.createdAt)}`}
                </span>
                <Button size="sm" variant={item.status === 'Pending' ? 'primary' : 'secondary'} onClick={() => onOpen(item)}>
                  {item.status === 'Pending' ? 'Review' : 'View'}
                </Button>
              </div>
            </Card>
          ))}
        </div>
      )}
    </>
  )
}
