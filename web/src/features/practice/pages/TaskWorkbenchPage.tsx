import { useState } from 'react'
import { useParams } from 'react-router-dom'
import { ArrowLeft, ArrowRight, Building2, CheckCircle2, Clock, RotateCcw, Send, Wrench } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { Badge } from '@/components/ui/Badge'
import { Button } from '@/components/ui/Button'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { CodeEditor } from '@/components/ui/CodeEditor'
import { EmptyState } from '@/components/ui/EmptyState'
import { LinkButton } from '@/components/ui/LinkButton'
import { PageSpinner } from '@/components/ui/Spinner'
import { LEARNING_STAGE_META } from '@/config/learning'
import { WORK_SITUATION_LABEL } from '@/config/tasks'
import { getErrorMessage } from '@/lib/http'
import { useSubmitAttempt, useWorkbench } from '../api/practiceApi'
import { AttemptHistory } from '../components/AttemptHistory'
import { CheckResultsPanel } from '../components/CheckResultsPanel'
import type { TaskAttempt, Workbench } from '../types'

function WorkbenchView({ workbench }: { workbench: Workbench }) {
  const { task, attempts } = workbench
  const latest = attempts[0] ?? null
  const [code, setCode] = useState(latest?.code ?? task.starterCode ?? '')
  const [selected, setSelected] = useState<TaskAttempt | null>(latest)
  const submit = useSubmitAttempt()

  const handleSubmit = () =>
    submit.mutate(
      { taskId: task.id, code },
      {
        onSuccess: (attempt) => {
          setSelected(attempt)
          toast.info(`Attempt ${attempt.attemptNumber}: ${attempt.passedChecks} of ${attempt.totalChecks} checks passed.`)
        },
        onError: (e) => toast.error(getErrorMessage(e)),
      },
    )

  const selectAttempt = (attempt: TaskAttempt) => {
    setSelected(attempt)
    setCode(attempt.code)
  }

  return (
    <>
      <LinkButton to="/tasks" variant="secondary" className="mb-4 h-9"><ArrowLeft className="size-4" /> All tasks</LinkButton>

      <div className="mb-6">
        <div className="flex flex-wrap gap-1.5">
          <Badge tone="primary">{task.careerPathName}</Badge>
          <Badge tone={LEARNING_STAGE_META[task.level].tone}>{LEARNING_STAGE_META[task.level].label}</Badge>
          <Badge>{WORK_SITUATION_LABEL[task.situation]}</Badge>
        </div>
        <h1 className="mt-3 text-2xl font-bold">{task.title}</h1>
        <p className="mt-1 flex flex-wrap items-center gap-4 text-sm text-muted">
          <span className="flex items-center gap-1"><Building2 className="size-4" /> {task.companyName}</span>
          <span className="flex items-center gap-1"><Clock className="size-4" /> ~{task.estimatedMinutes} min</span>
        </p>
      </div>

      <div className="grid gap-6 xl:grid-cols-[minmax(0,2fr)_minmax(0,3fr)]">
        <div className="space-y-6">
          <Card>
            <CardHeader title="The ticket" />
            <CardBody>
              <p className="whitespace-pre-line leading-relaxed">{task.instructions}</p>
              <div className="mt-4 flex flex-wrap gap-1.5">{task.skills.map((s) => <Badge key={s}>{s}</Badge>)}</div>
            </CardBody>
          </Card>
          <Card>
            <CardHeader title="What will be checked" description="Your submission is compared with these simple patterns." />
            <CardBody>
              <ul className="list-disc space-y-1 pl-5 text-sm">{task.checks.map((c) => <li key={c.id}>{c.description}</li>)}</ul>
            </CardBody>
          </Card>
          <AttemptHistory attempts={attempts} selectedId={selected?.id ?? null} onSelect={selectAttempt} />
        </div>

        <div className="space-y-6">
          <Card>
            <CardHeader
              title="Your solution"
              action={task.starterCode && (
                <Button variant="ghost" size="sm" onClick={() => setCode(task.starterCode ?? '')}>
                  <RotateCcw className="size-4" /> Reset to starter
                </Button>
              )}
            />
            <CardBody className="space-y-4">
              <CodeEditor value={code} onChange={setCode} language={task.starterLanguage} minRows={18} ariaLabel="Your solution" />
              <div className="flex flex-wrap items-center justify-between gap-3">
                <p className="text-xs text-muted">Code is checked as text only — it is not run.</p>
                <Button loading={submit.isPending} disabled={!code.trim()} onClick={handleSubmit}>
                  <Send className="size-4" /> Submit attempt
                </Button>
              </div>
            </CardBody>
          </Card>

          {selected && <CheckResultsPanel attempt={selected} />}

          {attempts.length > 0 && (
            <Card className="flex flex-wrap items-center justify-between gap-4 p-5">
              <div>
                <p className="flex items-center gap-2 font-semibold text-heading">
                  {workbench.knowledgeCheckSubmittedAt && <CheckCircle2 className="size-4 text-success" />}
                  Knowledge check
                </p>
                <p className="text-sm text-muted">
                  {workbench.knowledgeCheckSubmittedAt
                    ? 'Your answers are saved as practice evidence. You can update them at any time.'
                    : `Explain your work in ${task.knowledgeQuestionCount} short answer${task.knowledgeQuestionCount === 1 ? '' : 's'}.`}
                </p>
              </div>
              <LinkButton to={`/assessment/${task.id}`} variant={workbench.knowledgeCheckSubmittedAt ? 'secondary' : 'primary'}>
                {workbench.knowledgeCheckSubmittedAt ? 'Review answers' : 'Answer questions'} <ArrowRight className="size-4" />
              </LinkButton>
            </Card>
          )}
        </div>
      </div>
    </>
  )
}

export default function TaskWorkbenchPage() {
  const taskId = Number(useParams().id)
  const { data: workbench, isLoading, isError, error } = useWorkbench(taskId)

  if (isLoading) return <PageSpinner label="Opening the workbench…" />
  if (isError || !workbench) {
    return (
      <EmptyState
        icon={Wrench}
        title="Task not available"
        description={getErrorMessage(error)}
        action={<LinkButton to="/tasks" variant="secondary">Back to tasks</LinkButton>}
      />
    )
  }

  return <WorkbenchView key={workbench.task.id} workbench={workbench} />
}
