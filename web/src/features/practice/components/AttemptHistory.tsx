import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { cn } from '@/lib/cn'
import { formatDateTime } from '@/lib/format'
import type { TaskAttempt } from '../types'

interface AttemptHistoryProps {
  attempts: TaskAttempt[]
  selectedId: number | null
  onSelect: (attempt: TaskAttempt) => void
}

export function AttemptHistory({ attempts, selectedId, onSelect }: AttemptHistoryProps) {
  if (attempts.length === 0) return null

  return (
    <Card>
      <CardHeader title="Your attempts" description="Select an attempt to see its results and load its code." />
      <CardBody>
        <ul className="space-y-1.5">
          {attempts.map((attempt) => {
            const allPassed = attempt.passedChecks === attempt.totalChecks
            return (
              <li key={attempt.id}>
                <button
                  onClick={() => onSelect(attempt)}
                  className={cn(
                    'flex w-full items-center justify-between rounded-xl border px-3 py-2 text-left text-sm transition-colors',
                    attempt.id === selectedId ? 'border-primary bg-primary-soft' : 'border-line hover:bg-surface-subtle',
                  )}
                >
                  <span className="font-medium text-heading">Attempt {attempt.attemptNumber}</span>
                  <span className="flex items-center gap-3 text-xs">
                    <span className="text-muted">{formatDateTime(attempt.submittedAt)}</span>
                    <span className={allPassed ? 'font-semibold text-success' : 'font-semibold text-warning'}>
                      {attempt.passedChecks}/{attempt.totalChecks}
                    </span>
                  </span>
                </button>
              </li>
            )
          })}
        </ul>
      </CardBody>
    </Card>
  )
}
