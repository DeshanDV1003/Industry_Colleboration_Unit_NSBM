import { ArrowRight, Info, Mic } from 'lucide-react'
import { PageHeader } from '@/components/layout/PageHeader'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { LinkButton } from '@/components/ui/LinkButton'
import { PageSpinner } from '@/components/ui/Spinner'
import { getErrorMessage } from '@/lib/http'
import { useInterviewSession } from '../api/interviewsApi'
import { CameraPreview } from '../components/CameraPreview'
import { QuestionPractice } from '../components/QuestionPractice'

/** Stage 1: soft-skills interview practice. */
export default function InterviewPrepPage() {
  const { data: session, isLoading, isError, error } = useInterviewSession('SoftSkills')

  if (isLoading) return <PageSpinner label="Loading questions…" />
  if (isError || !session) return <EmptyState icon={Mic} title="Questions could not be loaded" description={getErrorMessage(error)} />

  const answered = session.questions.filter((q) => q.latestAnswer).length

  return (
    <>
      <PageHeader
        title="Stage 1 · Soft-skills interview"
        description="Practise communication, teamwork and workplace situations. Answer as often as you like — a low score never blocks you."
        actions={<LinkButton to="/interview" variant="secondary">Stage 2: technical <ArrowRight className="size-4" /></LinkButton>}
      />

      <div className="grid gap-6 xl:grid-cols-[minmax(0,1fr)_340px]">
        <div className="space-y-4">
          {session.questions.map((q, i) => <QuestionPractice key={q.id} question={q} index={i} />)}
        </div>
        <aside className="space-y-6">
          <Card>
            <CardHeader title="Your progress" />
            <CardBody>
              <p className="font-display text-3xl font-bold text-heading">{answered}/{session.questions.length}</p>
              <p className="text-sm text-muted">questions answered</p>
              <LinkButton to="/interview-report" variant="secondary" className="mt-4 w-full">View interview report</LinkButton>
            </CardBody>
          </Card>
          <CameraPreview />
          <Card>
            <CardBody className="space-y-2 text-xs text-muted">
              <p className="flex gap-2"><Info className="mt-0.5 size-3.5 shrink-0" /> Feedback uses simple topic and keyword checks — there is no AI grading.</p>
              <p className="flex gap-2"><Mic className="mt-0.5 size-3.5 shrink-0" /> Voice input uses your browser's speech service, which may send audio to the browser maker. Only the text you save is stored.</p>
            </CardBody>
          </Card>
        </aside>
      </div>
    </>
  )
}
