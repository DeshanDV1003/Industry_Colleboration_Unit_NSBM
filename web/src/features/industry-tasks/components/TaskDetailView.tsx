import { CircleCheck, CircleSlash, Clock, MessageSquareText } from 'lucide-react'
import { Badge } from '@/components/ui/Badge'
import { CodeEditor } from '@/components/ui/CodeEditor'
import { LEARNING_STAGE_META } from '@/config/learning'
import { WORK_SITUATION_LABEL } from '@/config/tasks'
import type { IndustryTask } from '../types'

function Section({ title, children }: { title: string; children: React.ReactNode }) {
  return (
    <section className="mt-6 first:mt-0">
      <h3 className="mb-2 text-xs font-bold uppercase tracking-wider text-muted">{title}</h3>
      {children}
    </section>
  )
}

/** Read-only view of a complete task, including its pattern checks. Used by partners and reviewers. */
export function TaskDetailView({ task }: { task: IndustryTask }) {
  return (
    <div>
      <div className="flex flex-wrap items-center gap-2">
        <Badge tone="primary">{task.careerPathName}</Badge>
        <Badge tone={LEARNING_STAGE_META[task.level].tone}>{LEARNING_STAGE_META[task.level].label}</Badge>
        <Badge>{WORK_SITUATION_LABEL[task.situation]}</Badge>
        <span className="flex items-center gap-1 text-xs text-muted"><Clock className="size-3.5" /> ~{task.estimatedMinutes} min</span>
      </div>

      {task.reviewNote && (
        <div className="mt-4 flex gap-2.5 rounded-xl bg-warning-soft px-4 py-3 text-sm text-heading">
          <MessageSquareText className="mt-0.5 size-4 shrink-0 text-warning" />
          <div>
            <p className="font-semibold">Reviewer note{task.reviewedBy && ` · ${task.reviewedBy}`}</p>
            <p className="mt-0.5">{task.reviewNote}</p>
          </div>
        </div>
      )}

      <div className="mt-6">
        <Section title="Summary"><p>{task.summary}</p></Section>
        <Section title="Instructions"><p className="whitespace-pre-line">{task.instructions}</p></Section>
        <Section title="Skills and tools">
          <div className="flex flex-wrap gap-1.5">{task.skills.map((s) => <Badge key={s.id}>{s.name}</Badge>)}</div>
        </Section>
        {task.starterCode && (
          <Section title="Starter code">
            <CodeEditor value={task.starterCode} onChange={() => undefined} readOnly language={task.starterLanguage} minRows={4} ariaLabel="Starter code" />
          </Section>
        )}
        <Section title="Pattern checks">
          <ul className="space-y-2">
            {task.checks.map((check) => (
              <li key={check.id} className="rounded-xl border border-line p-3">
                <div className="flex items-start gap-2">
                  {check.shouldMatch
                    ? <CircleCheck className="mt-0.5 size-4 shrink-0 text-success" aria-label="Must match" />
                    : <CircleSlash className="mt-0.5 size-4 shrink-0 text-danger" aria-label="Must not match" />}
                  <div className="min-w-0">
                    <p className="font-medium text-heading">{check.description}</p>
                    <code className="mt-1 block break-all rounded bg-surface-subtle px-2 py-1 font-mono text-xs">{check.pattern}</code>
                    {check.hint && <p className="mt-1 text-xs text-muted">Hint: {check.hint}</p>}
                  </div>
                </div>
              </li>
            ))}
          </ul>
        </Section>
        <Section title="Knowledge check questions">
          <ol className="list-decimal space-y-1 pl-5">{task.knowledgeQuestions.map((q) => <li key={q.id}>{q.prompt}</li>)}</ol>
        </Section>
      </div>
    </div>
  )
}
