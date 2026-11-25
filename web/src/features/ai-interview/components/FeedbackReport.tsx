import { CheckCircle2, Info, TrendingUp } from 'lucide-react'
import { Badge } from '@/components/ui/Badge'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { ProgressBar } from '@/components/ui/ProgressBar'
import type { AiInterviewFeedback } from '../types'

const MAX_SCORE = 5

/** AI-generated practice feedback, clearly labelled as such. */
export function FeedbackReport({ feedback }: { feedback: AiInterviewFeedback }) {
  return (
    <Card>
      <CardHeader
        title="Your practice feedback"
        description="AI-generated, for practice only. It reviews your answers, not you."
        action={<Badge tone="primary">Average {feedback.averageScore}/{MAX_SCORE}</Badge>}
      />
      <CardBody className="space-y-5 text-sm">
        <p className="text-heading">{feedback.summary}</p>

        <div className="space-y-3">
          {feedback.scores.map((s) => (
            <div key={s.criterion}>
              <div className="mb-1 flex justify-between gap-2"><span className="font-medium text-heading">{s.criterion}</span><span className="font-semibold">{s.score}/{MAX_SCORE}</span></div>
              <ProgressBar value={(s.score / MAX_SCORE) * 100} label={s.criterion} />
              <p className="mt-1 text-xs text-muted">{s.comment}</p>
            </div>
          ))}
        </div>

        <div className="grid gap-4 md:grid-cols-2">
          <section>
            <h4 className="mb-2 flex items-center gap-1.5 font-semibold text-success"><CheckCircle2 className="size-4" /> What went well</h4>
            <ul className="list-disc space-y-1 pl-5">{feedback.strengths.map((s) => <li key={s}>{s}</li>)}</ul>
          </section>
          <section>
            <h4 className="mb-2 flex items-center gap-1.5 font-semibold text-primary"><TrendingUp className="size-4" /> Try next time</h4>
            <ul className="list-disc space-y-1 pl-5">{feedback.improvements.map((s) => <li key={s}>{s}</li>)}</ul>
          </section>
        </div>

        <p className="flex gap-2 text-xs text-muted">
          <Info className="mt-0.5 size-3.5 shrink-0" /> AI feedback can be wrong. It is private to you and is not a hiring score; lecturers and companies cannot see it.
        </p>
      </CardBody>
    </Card>
  )
}
