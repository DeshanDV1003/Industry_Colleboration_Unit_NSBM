import { CheckCircle2, Info, XCircle } from 'lucide-react'
import { Badge } from '@/components/ui/Badge'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { ProgressBar } from '@/components/ui/ProgressBar'
import { SKILL_SIGNAL_META } from '@/config/learning'
import { formatDateTime } from '@/lib/format'
import type { BaselineResult } from '../types'

export function BaselineResultView({ result }: { result: BaselineResult }) {
  return (
    <div className="grid gap-6 xl:grid-cols-[minmax(0,1fr)_minmax(0,1.4fr)]">
      <div className="space-y-6">
        <Card>
          <CardHeader title="First snapshot" description={`Completed ${formatDateTime(result.completedAt)}`} />
          <CardBody>
            <div className="flex items-end gap-2">
              <span className="font-display text-5xl font-bold text-heading">{result.scorePercent}%</span>
              <span className="pb-2 text-muted">{result.correctCount} of {result.questionCount} correct</span>
            </div>
            <ProgressBar className="mt-4" label="Baseline score" value={result.scorePercent} />
            <p className="mt-4 flex gap-2 text-xs text-muted">
              <Info className="mt-0.5 size-3.5 shrink-0" />
              This is a practice signal to guide your learning, not a certified measure of ability.
            </p>
          </CardBody>
        </Card>

        <Card>
          <CardHeader title="Strong and weak topics" description="Based on your answers for each skill." />
          <CardBody>
            <ul className="divide-y divide-line">
              {result.skills.map((skill) => {
                const meta = SKILL_SIGNAL_META[skill.signal]
                return (
                  <li key={skill.skillId} className="flex items-center justify-between gap-3 py-3 first:pt-0 last:pb-0">
                    <div>
                      <p className="font-medium text-heading">{skill.skillName}</p>
                      <p className="text-xs text-muted">{skill.correct} of {skill.total} correct</p>
                    </div>
                    <Badge tone={meta.tone}>{meta.label}</Badge>
                  </li>
                )
              })}
            </ul>
          </CardBody>
        </Card>
      </div>

      <Card>
        <CardHeader title="Review your answers" description="See what each question was checking and why." />
        <CardBody>
          <ol className="space-y-4">
            {result.review.map((item, index) => (
              <li key={item.questionId} className="rounded-xl border border-line p-4">
                <div className="flex items-start gap-3">
                  {item.isCorrect
                    ? <CheckCircle2 className="mt-0.5 size-5 shrink-0 text-success" aria-label="Correct" />
                    : <XCircle className="mt-0.5 size-5 shrink-0 text-danger" aria-label="Incorrect" />}
                  <div className="min-w-0 flex-1">
                    <p className="font-medium text-heading">{index + 1}. {item.prompt}</p>
                    <p className="mt-2 text-sm">
                      <span className="text-muted">Your answer: </span>
                      <span className={item.isCorrect ? 'text-success' : 'text-danger'}>{item.selectedOption}</span>
                    </p>
                    {!item.isCorrect && (
                      <p className="text-sm"><span className="text-muted">Correct answer: </span>{item.correctOption}</p>
                    )}
                    {item.explanation && <p className="mt-2 text-sm text-muted">{item.explanation}</p>}
                  </div>
                  <Badge className="hidden sm:inline-flex">{item.skillName}</Badge>
                </div>
              </li>
            ))}
          </ol>
        </CardBody>
      </Card>
    </div>
  )
}
