import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { Bot, BookOpenCheck, MessagesSquare, ShieldCheck, TriangleAlert } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { PageHeader } from '@/components/layout/PageHeader'
import { Badge } from '@/components/ui/Badge'
import { Button } from '@/components/ui/Button'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { LinkButton } from '@/components/ui/LinkButton'
import { SegmentedControl } from '@/components/ui/SegmentedControl'
import { PageSpinner } from '@/components/ui/Spinner'
import { formatDateTime } from '@/lib/format'
import { getErrorMessage } from '@/lib/http'
import { useAiInterviewAvailability, useMyAiInterviews, useStartAiInterview } from '../api/aiInterviewApi'
import type { InterviewStage } from '../types'

const STAGE_LABEL: Record<InterviewStage, string> = { SoftSkills: 'Soft skills', Technical: 'Technical' }

function StartCard({ questions }: { questions: number }) {
  const navigate = useNavigate()
  const start = useStartAiInterview()
  const [stage, setStage] = useState<InterviewStage>('SoftSkills')
  const [consent, setConsent] = useState(false)

  return (
    <Card>
      <CardHeader title="Start a practice interview" description={`${questions} questions with follow-ups, then rubric feedback.`} />
      <CardBody className="space-y-4 text-sm">
        <SegmentedControl
          ariaLabel="Interview type"
          value={stage}
          onChange={setStage}
          options={[{ value: 'SoftSkills', label: 'Soft skills' }, { value: 'Technical', label: 'Technical' }]}
        />
        <p className="text-muted">
          {stage === 'SoftSkills'
            ? 'Teamwork, communication, pressure and motivation. Use real examples from projects, work or societies.'
            : 'Practical questions for your chosen career path, from fundamentals to how you would approach a problem.'}
        </p>
        <label className="flex gap-2 rounded-xl border border-line p-3">
          <input type="checkbox" className="mt-0.5 size-4 shrink-0 accent-[var(--color-primary)]" checked={consent} onChange={(e) => setConsent(e.target.checked)} />
          <span>
            I agree that my typed answers are sent to Anthropic's Claude AI service to run this practice interview.
            No audio or video is sent. Don't include personal details you would not share with an interviewer.
          </span>
        </label>
        <Button
          disabled={!consent}
          loading={start.isPending}
          onClick={() => start.mutate(stage, {
            onSuccess: (session) => navigate(`/ai-interview/${session.id}`),
            onError: (e) => toast.error(getErrorMessage(e)),
          })}
        >
          <Bot className="size-4" /> Start interview
        </Button>
      </CardBody>
    </Card>
  )
}

export default function AiInterviewHomePage() {
  const availability = useAiInterviewAvailability()
  const history = useMyAiInterviews()

  if (availability.isLoading || history.isLoading) return <PageSpinner label="Loading…" />
  if (availability.isError || !availability.data) return <EmptyState icon={Bot} title="AI interviewer could not be loaded" description={getErrorMessage(availability.error)} />

  return (
    <>
      <PageHeader
        title="AI interviewer"
        description="Practise a realistic conversation with an AI interviewer that asks follow-up questions, then get feedback on your answers."
      />
      <div className="grid gap-6 xl:grid-cols-[420px_minmax(0,1fr)]">
        <div className="space-y-6">
          {availability.data.isAvailable ? <StartCard questions={availability.data.questionsPerSession} /> : (
            <Card>
              <CardBody className="space-y-3 text-sm">
                <p className="flex items-center gap-2 font-semibold text-heading"><TriangleAlert className="size-4 text-warning" /> The AI interviewer is not set up yet</p>
                <p className="text-muted">The Industry Collaboration Unit needs to add an AI service key. Meanwhile, the standard practice questions work without it.</p>
                <div className="flex flex-wrap gap-2">
                  <LinkButton to="/interview-prep" variant="secondary"><MessagesSquare className="size-4" /> Soft-skills practice</LinkButton>
                  <LinkButton to="/interview" variant="secondary"><BookOpenCheck className="size-4" /> Technical practice</LinkButton>
                </div>
              </CardBody>
            </Card>
          )}
          <p className="flex gap-2 rounded-xl bg-surface-subtle p-3 text-xs text-muted">
            <ShieldCheck className="mt-0.5 size-4 shrink-0 text-success" />
            Your AI interviews are private. Lecturers and companies cannot see them, and the feedback is practice, not a hiring score.
          </p>
        </div>

        <Card>
          <CardHeader title="Your practice interviews" />
          <CardBody>
            {!history.data?.length ? <p className="text-sm text-muted">No AI interviews yet.</p> : (
              <ul className="divide-y divide-line">
                {history.data.map((s) => (
                  <li key={s.id} className="flex flex-wrap items-center gap-3 py-3 text-sm">
                    <Link to={`/ai-interview/${s.id}`} className="font-semibold">{STAGE_LABEL[s.stage]} · {s.targetRole}</Link>
                    <span className="text-xs text-muted">{formatDateTime(s.startedAt)} · {s.answerCount} answer{s.answerCount === 1 ? '' : 's'}</span>
                    <span className="ml-auto">
                      {s.status === 'Completed'
                        ? <Badge tone="success">Feedback {s.averageScore ?? '—'}/5</Badge>
                        : <Badge tone="warning">In progress</Badge>}
                    </span>
                  </li>
                ))}
              </ul>
            )}
          </CardBody>
        </Card>
      </div>
    </>
  )
}
