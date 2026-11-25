import { useState } from 'react'
import { toast } from '@/components/feedback/toast'
import { ReviewComments } from '@/components/ReviewComments'
import { Badge } from '@/components/ui/Badge'
import { Button } from '@/components/ui/Button'
import { Card } from '@/components/ui/Card'
import { CodeEditor } from '@/components/ui/CodeEditor'
import { controlClass } from '@/components/ui/controlClass'
import { cn } from '@/lib/cn'
import { formatDateTime } from '@/lib/format'
import { getErrorMessage } from '@/lib/http'
import { useReviewAttempt, type AttemptForReview } from '../api/taskReviewsApi'

export function AttemptReviewCard({ attempt }: { attempt: AttemptForReview }) {
  const [showCode, setShowCode] = useState(false)
  const [comment, setComment] = useState('')
  const [improvements, setImprovements] = useState('')
  const review = useReviewAttempt()

  const submit = () =>
    review.mutate({ attemptId: attempt.attemptId, comment, improvements: improvements.trim() || null }, {
      onSuccess: () => { setComment(''); setImprovements(''); toast.success('Review shared with the student.') },
      onError: (e) => toast.error(getErrorMessage(e)),
    })

  return (
    <Card className="p-5">
      <div className="flex flex-wrap items-start justify-between gap-2">
        <div>
          <h3 className="font-semibold">{attempt.taskTitle}</h3>
          <p className="text-xs text-muted">{attempt.studentName} · {attempt.studentNo} · attempt {attempt.attemptNumber} · {formatDateTime(attempt.submittedAt)}</p>
        </div>
        <div className="flex gap-1.5">
          <Badge tone={attempt.passedChecks === attempt.totalChecks ? 'success' : 'warning'}>{attempt.passedChecks}/{attempt.totalChecks} checks</Badge>
          <Badge tone={attempt.reviews.length ? 'accent' : 'neutral'}>{attempt.reviews.length ? 'Reviewed' : 'Not reviewed'}</Badge>
        </div>
      </div>

      <button className="mt-3 text-sm font-semibold" onClick={() => setShowCode((v) => !v)} aria-expanded={showCode}>
        {showCode ? 'Hide submission' : 'View submission'}
      </button>
      {showCode && <CodeEditor className="mt-2" value={attempt.code} onChange={() => undefined} readOnly minRows={6} ariaLabel="Student submission" />}

      {attempt.reviews.length > 0 && <div className="mt-3"><ReviewComments reviews={attempt.reviews} /></div>}

      <div className="mt-4 grid gap-2 md:grid-cols-2">
        <textarea aria-label="Review comment" rows={3} value={comment} onChange={(e) => setComment(e.target.value)} placeholder="What did they do well?" className={cn(controlClass(false), 'py-2.5')} />
        <textarea aria-label="Improvement ideas" rows={3} value={improvements} onChange={(e) => setImprovements(e.target.value)} placeholder="One or two concrete improvements (optional)" className={cn(controlClass(false), 'py-2.5')} />
      </div>
      <Button size="sm" className="mt-2" loading={review.isPending} disabled={comment.trim().length < 10} onClick={submit}>Share review</Button>
    </Card>
  )
}
