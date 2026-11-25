import { useState } from 'react'
import { useParams } from 'react-router-dom'
import { ArrowLeft, CheckCircle2, ClipboardCheck, Info } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { PageHeader } from '@/components/layout/PageHeader'
import { Button } from '@/components/ui/Button'
import { Card } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { controlClass } from '@/components/ui/controlClass'
import { LinkButton } from '@/components/ui/LinkButton'
import { PageSpinner } from '@/components/ui/Spinner'
import { cn } from '@/lib/cn'
import { formatDateTime } from '@/lib/format'
import { getErrorMessage } from '@/lib/http'
import { useKnowledgeCheck, useSubmitKnowledgeCheck } from '../api/practiceApi'
import type { KnowledgeCheck } from '../types'

/** Mirrors SubmitKnowledgeCheckCommandValidator.MinimumAnswerLength. */
const MIN_ANSWER_LENGTH = 20

function KnowledgeCheckForm({ check }: { check: KnowledgeCheck }) {
  const [answers, setAnswers] = useState<Record<number, string>>(
    () => Object.fromEntries(check.questions.map((q) => [q.id, q.answer ?? ''])),
  )
  const submit = useSubmitKnowledgeCheck()
  const complete = check.questions.every((q) => answers[q.id].trim().length >= MIN_ANSWER_LENGTH)

  const handleSubmit = () =>
    submit.mutate(
      { taskId: check.taskId, answers: check.questions.map((q) => ({ questionId: q.id, text: answers[q.id] })) },
      {
        onSuccess: () => toast.success('Answers saved as practice evidence.'),
        onError: (e) => toast.error(getErrorMessage(e)),
      },
    )

  return (
    <div className="space-y-4">
      {check.questions.map((question, index) => {
        const length = answers[question.id].trim().length
        return (
          <Card key={question.id} className="p-6">
            <label htmlFor={`answer-${question.id}`} className="block font-medium text-heading">
              <span className="mr-2 text-xs font-bold text-muted">Q{index + 1}</span>{question.prompt}
            </label>
            <textarea
              id={`answer-${question.id}`}
              rows={4}
              value={answers[question.id]}
              onChange={(e) => setAnswers((prev) => ({ ...prev, [question.id]: e.target.value }))}
              className={cn(controlClass(false), 'mt-3 py-2.5')}
              placeholder="Explain in your own words…"
            />
            <p className={cn('mt-1.5 text-right text-xs', length >= MIN_ANSWER_LENGTH ? 'text-success' : 'text-muted')}>
              {length < MIN_ANSWER_LENGTH ? `${MIN_ANSWER_LENGTH - length} more characters needed` : 'Looks good'}
            </p>
          </Card>
        )
      })}

      <div className="flex flex-wrap items-center justify-between gap-3">
        <p className="flex items-center gap-2 text-xs text-muted">
          <Info className="size-3.5" /> Answers are saved as beginner-level practice evidence for your lecturer — they are not graded automatically.
        </p>
        <Button disabled={!complete} loading={submit.isPending} onClick={handleSubmit}>
          {check.submittedAt ? 'Update answers' : 'Submit answers'}
        </Button>
      </div>
    </div>
  )
}

export default function KnowledgeCheckPage() {
  const taskId = Number(useParams().id)
  const { data: check, isLoading, isError, error } = useKnowledgeCheck(taskId)

  if (isLoading) return <PageSpinner label="Loading questions…" />
  if (isError || !check) return <EmptyState icon={ClipboardCheck} title="Knowledge check not available" description={getErrorMessage(error)} />

  const back = <LinkButton to={`/tasks/${taskId}`} variant="secondary" className="h-9"><ArrowLeft className="size-4" /> Back to workbench</LinkButton>

  return (
    <>
      <div className="mb-4">{back}</div>
      <PageHeader
        title="Knowledge check"
        description={<>Follow-up questions for <strong className="text-heading">{check.taskTitle}</strong>. Show what you really understand.</>}
        actions={check.submittedAt && (
          <span className="flex items-center gap-1.5 text-sm text-success">
            <CheckCircle2 className="size-4" /> Saved {formatDateTime(check.submittedAt)}
          </span>
        )}
      />

      {check.hasAttempt ? (
        <KnowledgeCheckForm key={check.submittedAt ?? 'new'} check={check} />
      ) : (
        <EmptyState
          icon={ClipboardCheck}
          title="Submit an attempt first"
          description="The knowledge check asks about your solution, so try the task in the workbench before answering."
          action={<LinkButton to={`/tasks/${taskId}`}>Open the workbench</LinkButton>}
        />
      )}
    </>
  )
}
