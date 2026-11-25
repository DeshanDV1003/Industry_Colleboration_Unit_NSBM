import { useParams } from 'react-router-dom'
import { Bot, Flag, RotateCw } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { PageHeader } from '@/components/layout/PageHeader'
import { Badge } from '@/components/ui/Badge'
import { Button } from '@/components/ui/Button'
import { Card, CardBody } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { LinkButton } from '@/components/ui/LinkButton'
import { PageSpinner } from '@/components/ui/Spinner'
import { getErrorMessage } from '@/lib/http'
import { useAiInterview, useAnswerAiInterview, useContinueAiInterview, useFinishAiInterview } from '../api/aiInterviewApi'
import { AnswerComposer } from '../components/AnswerComposer'
import { CoachingPanel } from '../components/CoachingPanel'
import { FeedbackReport } from '../components/FeedbackReport'
import { InterviewTranscript } from '../components/InterviewTranscript'

const onError = (e: unknown) => toast.error(getErrorMessage(e))

export default function AiInterviewSessionPage() {
  const id = Number(useParams().id)
  const { data: session, isLoading, isError, error } = useAiInterview(id)
  const answer = useAnswerAiInterview()
  const resume = useContinueAiInterview()
  const finish = useFinishAiInterview()

  if (isLoading) return <PageSpinner label="Loading interview…" />
  if (isError || !session) return <EmptyState icon={Bot} title="Interview could not be loaded" description={getErrorMessage(error)} action={<LinkButton to="/ai-interview" variant="secondary">Back</LinkButton>} />

  const busy = answer.isPending || resume.isPending || finish.isPending
  const answered = session.turns.filter((t) => t.speaker === 'Student').length
  const inProgress = session.status === 'InProgress'

  return (
    <>
      <PageHeader
        title={session.stage === 'Technical' ? 'Technical practice interview' : 'Soft-skills practice interview'}
        description={<span className="flex flex-wrap items-center gap-2">
          {session.targetRole}
          <Badge>{inProgress ? `Question ${session.questionCount} of ${session.questionsPerSession}` : 'Finished'}</Badge>
        </span>}
        actions={<LinkButton to="/ai-interview" variant="secondary">All interviews</LinkButton>}
      />
      <div className="grid gap-6 xl:grid-cols-[minmax(0,1fr)_360px]">
        <div className="space-y-6">
          <Card>
            <CardBody className="space-y-5">
              <InterviewTranscript turns={session.turns} thinking={busy} />
              {inProgress && session.awaitingAnswer && !busy && (
                <AnswerComposer sending={answer.isPending} onSend={(text) => answer.mutate({ id, answer: text }, { onError })} />
              )}
              {inProgress && session.needsNextStep && !busy && (
                <div className="flex flex-wrap items-center gap-3 rounded-xl bg-warning-soft p-3 text-sm">
                  <span className="text-heading">Your answer is saved, but the interviewer has not replied yet.</span>
                  <Button size="sm" onClick={() => resume.mutate(id, { onError })}><RotateCw className="size-4" /> Try again</Button>
                </div>
              )}
              {inProgress && answered > 0 && !busy && (
                <div className="flex justify-end border-t border-line pt-4">
                  <Button variant="ghost" size="sm" onClick={() => finish.mutate(id, { onError })}>
                    <Flag className="size-4" /> Finish now and get feedback
                  </Button>
                </div>
              )}
            </CardBody>
          </Card>
          {session.feedback && <FeedbackReport feedback={session.feedback} />}
        </div>
        <div className="xl:sticky xl:top-24 xl:self-start">
          {inProgress ? <CoachingPanel /> : (
            <Card><CardBody className="space-y-3 text-sm">
              <p className="font-semibold text-heading">Practise again?</p>
              <p className="text-muted">Use the "try next time" points in your next interview and compare the scores.</p>
              <LinkButton to="/ai-interview">Start another</LinkButton>
            </CardBody></Card>
          )}
        </div>
      </div>
    </>
  )
}
