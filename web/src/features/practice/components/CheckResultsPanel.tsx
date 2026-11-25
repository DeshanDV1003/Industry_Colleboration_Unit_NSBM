import { CheckCircle2, Info, XCircle } from 'lucide-react'
import { ReviewComments } from '@/components/ReviewComments'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { ProgressBar } from '@/components/ui/ProgressBar'
import { formatDateTime } from '@/lib/format'
import type { TaskAttempt } from '../types'

export function CheckResultsPanel({ attempt }: { attempt: TaskAttempt }) {
  const percent = attempt.totalChecks === 0 ? 0 : (attempt.passedChecks / attempt.totalChecks) * 100
  const allPassed = attempt.passedChecks === attempt.totalChecks

  return (
    <Card>
      <CardHeader
        title={`Attempt ${attempt.attemptNumber} · ${attempt.passedChecks} of ${attempt.totalChecks} checks passed`}
        description={formatDateTime(attempt.submittedAt)}
      />
      <CardBody>
        <ProgressBar label="Checks passed" value={percent} />
        <ul className="mt-4 space-y-2">
          {attempt.results.map((result) => (
            <li key={result.checkId} className="flex gap-2.5 text-sm">
              {result.passed
                ? <CheckCircle2 className="mt-0.5 size-4 shrink-0 text-success" aria-label="Passed" />
                : <XCircle className="mt-0.5 size-4 shrink-0 text-danger" aria-label="Not yet" />}
              <div>
                <p className={result.passed ? 'text-heading' : 'font-medium text-heading'}>{result.description}</p>
                {result.hint && <p className="mt-0.5 text-xs text-muted">Hint: {result.hint}</p>}
              </div>
            </li>
          ))}
        </ul>
        <p className="mt-4 flex gap-2 text-xs text-muted">
          <Info className="mt-0.5 size-3.5 shrink-0" />
          {allPassed
            ? 'All pattern checks pass. Checks only read your code as text — explain your reasoning in the knowledge check.'
            : 'Pattern checks only read your code as text; nothing is executed. Use the hints, adjust and try again.'}
        </p>
        {attempt.reviews.length > 0 && (
          <div className="mt-4">
            <p className="mb-2 text-xs font-bold uppercase tracking-wider text-muted">Industry review</p>
            <ReviewComments reviews={attempt.reviews} />
          </div>
        )}
      </CardBody>
    </Card>
  )
}
