import { useState } from 'react'
import { AlertCircle } from 'lucide-react'
import { Badge } from '@/components/ui/Badge'
import { Button } from '@/components/ui/Button'
import { Card } from '@/components/ui/Card'
import { ProgressBar } from '@/components/ui/ProgressBar'
import { cn } from '@/lib/cn'
import { getErrorMessage } from '@/lib/http'
import { useSubmitBaseline } from '../api/baselineApi'
import type { BaselineQuestion } from '../types'

interface BaselineQuizProps {
  questions: BaselineQuestion[]
  onSubmitted: () => void
  onCancel?: () => void
}

export function BaselineQuiz({ questions, onSubmitted, onCancel }: BaselineQuizProps) {
  const [answers, setAnswers] = useState<Record<number, number>>({})
  const submit = useSubmitBaseline()

  const answeredCount = Object.keys(answers).length
  const complete = answeredCount === questions.length

  const handleSubmit = () =>
    submit.mutate(
      Object.entries(answers).map(([questionId, optionId]) => ({ questionId: Number(questionId), optionId })),
      { onSuccess: onSubmitted },
    )

  return (
    <div className="space-y-4">
      <Card className="sticky top-[88px] z-10 flex flex-wrap items-center gap-4 px-6 py-4">
        <div className="min-w-[200px] flex-1">
          <p className="text-sm font-medium text-heading">{answeredCount} of {questions.length} answered</p>
          <ProgressBar className="mt-2" label="Questions answered" value={(answeredCount / questions.length) * 100} />
        </div>
        {onCancel && <Button variant="ghost" onClick={onCancel}>Cancel</Button>}
        <Button disabled={!complete} loading={submit.isPending} onClick={handleSubmit}>
          Submit answers
        </Button>
      </Card>

      {submit.isError && (
        <div role="alert" className="flex items-center gap-2 rounded-control bg-danger-soft px-4 py-3 text-sm font-medium text-danger">
          <AlertCircle className="size-4" /> {getErrorMessage(submit.error)}
        </div>
      )}

      {questions.map((question, index) => (
        <Card key={question.id} className="p-6">
          <fieldset>
            <legend className="flex w-full flex-wrap items-center justify-between gap-2">
              <span className="text-xs font-bold text-muted">QUESTION {index + 1}</span>
              <Badge>{question.skillName}</Badge>
            </legend>
            <p className="mt-2 font-medium text-heading">{question.prompt}</p>
            <div className="mt-4 grid gap-2 sm:grid-cols-2">
              {question.options.map((option) => {
                const checked = answers[question.id] === option.id
                return (
                  <label
                    key={option.id}
                    className={cn(
                      'flex cursor-pointer items-center gap-3 rounded-xl border px-4 py-3 text-sm transition-colors',
                      checked ? 'border-primary bg-primary-soft text-heading' : 'border-line hover:border-line-strong hover:bg-surface-subtle',
                    )}
                  >
                    <input
                      type="radio"
                      name={`q-${question.id}`}
                      className="size-4 accent-[var(--color-primary)]"
                      checked={checked}
                      onChange={() => setAnswers((prev) => ({ ...prev, [question.id]: option.id }))}
                    />
                    {option.text}
                  </label>
                )
              })}
            </div>
          </fieldset>
        </Card>
      ))}
    </div>
  )
}
