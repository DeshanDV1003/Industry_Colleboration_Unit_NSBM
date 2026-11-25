import { useState } from 'react'
import { BadgeCheck, CheckCircle2, XCircle } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { PageHeader } from '@/components/layout/PageHeader'
import { Badge } from '@/components/ui/Badge'
import { Button } from '@/components/ui/Button'
import { Card, CardBody } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { SegmentedControl } from '@/components/ui/SegmentedControl'
import { PageSpinner } from '@/components/ui/Spinner'
import { controlClass } from '@/components/ui/controlClass'
import { MODERATION_STATUSES, MODERATION_STATUS_META } from '@/config/tasks'
import { cn } from '@/lib/cn'
import { formatDate } from '@/lib/format'
import { getErrorMessage } from '@/lib/http'
import type { ModerationStatus } from '@/types/tasks'
import { useBadgeReviewQueue, useReviewBadge } from '../api/achievementsApi'
import type { BadgeReviewItem } from '../types'

function ReviewRow({ item }: { item: BadgeReviewItem }) {
  const review = useReviewBadge()
  const [note, setNote] = useState('')
  const decide = (decision: 'Approve' | 'Reject') => {
    if (decision === 'Reject' && !note.trim()) { toast.error('Add a short note so the student knows what to practise.'); return }
    review.mutate({ id: item.id, decision, note: note.trim() || null }, {
      onSuccess: () => toast.success(decision === 'Approve' ? 'Badge approved. The student has been told.' : 'Sent back with your advice.'),
      onError: (e) => toast.error(getErrorMessage(e)),
    })
  }

  return (
    <Card>
      <CardBody className="space-y-3 text-sm">
        <div className="flex flex-wrap items-start justify-between gap-2">
          <div>
            <p className="flex items-center gap-1.5 text-base font-semibold text-heading"><BadgeCheck className="size-4 text-primary" /> {item.skillName}</p>
            <p className="text-xs text-muted">{item.studentName} · {item.studentNo} · {item.programme}{item.careerPathName && ` · ${item.careerPathName}`}</p>
          </div>
          <span className="text-xs text-muted">Requested {formatDate(item.requestedAt)}</span>
        </div>
        <div>
          <p className="mb-1 text-xs font-semibold uppercase tracking-wide text-muted">Practice evidence</p>
          <ul className="list-disc space-y-0.5 pl-5">{item.evidence.map((e) => <li key={e}>{e}</li>)}</ul>
        </div>
        {item.studentNote && <p className="rounded-lg bg-surface-subtle p-2.5"><span className="font-medium text-heading">Student:</span> {item.studentNote}</p>}
        {item.status === 'Pending' ? (
          <>
            <textarea aria-label={`Note for ${item.studentName}`} rows={2} maxLength={1000} value={note} onChange={(e) => setNote(e.target.value)}
              placeholder="Optional when approving; required when declining." className={cn(controlClass(false), 'py-2')} />
            <div className="flex justify-end gap-2">
              <Button size="sm" variant="secondary" loading={review.isPending && review.variables?.decision === 'Reject'} onClick={() => decide('Reject')}><XCircle className="size-4" /> Not yet</Button>
              <Button size="sm" loading={review.isPending && review.variables?.decision === 'Approve'} onClick={() => decide('Approve')}><CheckCircle2 className="size-4" /> Approve badge</Button>
            </div>
          </>
        ) : (
          <p className="text-xs text-muted">
            <Badge tone={item.status === 'Approved' ? 'success' : 'danger'}>{item.status === 'Approved' ? 'Approved' : 'Not yet'}</Badge>{' '}
            by {item.reviewerName} on {formatDate(item.reviewedAt!)}{item.reviewNote && ` · “${item.reviewNote}”`}
          </p>
        )}
      </CardBody>
    </Card>
  )
}

/** Lecturers decide skill badges from the student's practice evidence. */
export default function BadgeReviewsPage() {
  const { data, isLoading, isError, error } = useBadgeReviewQueue()
  const [status, setStatus] = useState<ModerationStatus>('Pending')

  if (isLoading) return <PageSpinner label="Loading badge requests…" />
  if (isError || !data) return <EmptyState icon={BadgeCheck} title="Badge requests could not be loaded" description={getErrorMessage(error)} />

  const visible = data.filter((b) => b.status === status)

  return (
    <>
      <PageHeader title="Badge reviews" description="Students ask for a skill badge once the platform records practice evidence. Approve it if the evidence supports the skill." />
      <div className="mb-4">
        <SegmentedControl ariaLabel="Filter by status" value={status} onChange={setStatus}
          options={MODERATION_STATUSES.map((s) => ({ value: s, label: MODERATION_STATUS_META[s].label, count: data.filter((b) => b.status === s).length }))} />
      </div>
      {visible.length === 0
        ? <EmptyState icon={BadgeCheck} title={status === 'Pending' ? 'No badge requests waiting' : 'Nothing here'} description={status === 'Pending' ? 'New requests will appear here.' : undefined} />
        : <div className="grid gap-4 lg:grid-cols-2">{visible.map((b) => <ReviewRow key={b.id} item={b} />)}</div>}
    </>
  )
}
