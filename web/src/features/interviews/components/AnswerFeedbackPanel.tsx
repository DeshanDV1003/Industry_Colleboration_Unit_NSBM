import { CheckCircle2, Lightbulb, Mic } from 'lucide-react'
import { Badge } from '@/components/ui/Badge'
import { ProgressBar } from '@/components/ui/ProgressBar'
import { formatDateTime } from '@/lib/format'
import type { AnswerFeedback } from '../types'

export function AnswerFeedbackPanel({ feedback }: { feedback: AnswerFeedback }) {
  return (
    <div className="space-y-3 rounded-xl border border-line bg-surface-subtle p-4">
      <div className="flex flex-wrap items-center justify-between gap-2 text-xs text-muted">
        <span className="flex items-center gap-1.5">
          {feedback.inputMode === 'Voice' && <Mic className="size-3.5" />}
          Saved {formatDateTime(feedback.answeredAt)} · {feedback.wordCount} words
        </span>
        <span className="font-semibold text-heading">Topic coverage {feedback.coveragePercent}%</span>
      </div>
      <ProgressBar label="Topic coverage" value={feedback.coveragePercent} />

      <ul className="space-y-1.5 text-sm">
        {feedback.notes.map((note) => (
          <li key={note} className="flex gap-2"><Lightbulb className="mt-0.5 size-4 shrink-0 text-warning" /> {note}</li>
        ))}
      </ul>

      {feedback.matchedTopics.length > 0 && (
        <div>
          <p className="mb-1 flex items-center gap-1 text-xs font-semibold text-muted"><CheckCircle2 className="size-3.5 text-success" /> Topics you mentioned</p>
          <div className="flex flex-wrap gap-1">{feedback.matchedTopics.map((t) => <Badge key={t} tone="success">{t}</Badge>)}</div>
        </div>
      )}
      {feedback.topicsToConsider.length > 0 && (
        <div>
          <p className="mb-1 text-xs font-semibold text-muted">Topics you could add</p>
          <div className="flex flex-wrap gap-1">{feedback.topicsToConsider.map((t) => <Badge key={t}>{t}</Badge>)}</div>
        </div>
      )}
    </div>
  )
}
