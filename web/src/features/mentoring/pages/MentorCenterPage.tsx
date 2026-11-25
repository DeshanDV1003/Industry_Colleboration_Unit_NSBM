import { useState } from 'react'
import { MessagesSquare, Send } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { PageHeader } from '@/components/layout/PageHeader'
import { Button } from '@/components/ui/Button'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { FormField } from '@/components/ui/FormField'
import { PageSpinner } from '@/components/ui/Spinner'
import { controlClass } from '@/components/ui/controlClass'
import { cn } from '@/lib/cn'
import { getErrorMessage } from '@/lib/http'
import { useAskMentor, useMyMentorQuestions } from '../api/mentoringApi'
import { MentorQuestionCard } from '../components/MentorQuestionCard'

function AskForm() {
  const ask = useAskMentor()
  const [title, setTitle] = useState('')
  const [body, setBody] = useState('')

  const submit = () =>
    ask.mutate({ title, body }, {
      onSuccess: () => { setTitle(''); setBody(''); toast.success('Question sent to industry mentors.') },
      onError: (e) => toast.error(getErrorMessage(e)),
    })

  return (
    <Card>
      <CardHeader title="Ask a mentor" description="About a task, a technology or getting started at work." />
      <CardBody className="space-y-3">
        <FormField label="Question" htmlFor="mentor-title">
          <input id="mentor-title" className={cn(controlClass(false), 'h-10')} value={title} onChange={(e) => setTitle(e.target.value)} placeholder="Which test tools should I learn first?" />
        </FormField>
        <FormField label="Details" htmlFor="mentor-body" hint={body.trim().length < 20 ? `${20 - body.trim().length} more characters needed` : undefined}>
          <textarea id="mentor-body" rows={4} className={cn(controlClass(false), 'py-2.5')} value={body} onChange={(e) => setBody(e.target.value)} />
        </FormField>
        <Button loading={ask.isPending} disabled={!title.trim() || body.trim().length < 20} onClick={submit}><Send className="size-4" /> Send question</Button>
        <p className="text-xs text-muted">Replies come from fictional demo mentors at partner companies.</p>
      </CardBody>
    </Card>
  )
}

export default function MentorCenterPage() {
  const { data: questions, isLoading, isError, error } = useMyMentorQuestions()

  if (isLoading) return <PageSpinner label="Loading your questions…" />
  if (isError || !questions) return <EmptyState icon={MessagesSquare} title="Questions could not be loaded" description={getErrorMessage(error)} />

  return (
    <>
      <PageHeader title="Mentor support" description="Ask industry mentors for guidance. Questions go to a mentor queue where a partner can reply." />
      <div className="grid gap-6 xl:grid-cols-[minmax(0,1fr)_380px]">
        <div className="space-y-4">
          {questions.length === 0
            ? <EmptyState icon={MessagesSquare} title="No questions yet" description="Your questions and mentor replies will appear here." />
            : questions.map((q) => <MentorQuestionCard key={q.id} question={q} />)}
        </div>
        <aside><AskForm /></aside>
      </div>
    </>
  )
}
