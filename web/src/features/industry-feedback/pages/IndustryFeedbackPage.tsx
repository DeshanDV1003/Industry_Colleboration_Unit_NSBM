import { useState } from 'react'
import { MessagesSquare } from 'lucide-react'
import { PageHeader } from '@/components/layout/PageHeader'
import { EmptyState } from '@/components/ui/EmptyState'
import { SegmentedControl } from '@/components/ui/SegmentedControl'
import { PageSpinner } from '@/components/ui/Spinner'
import { MentorQuestionCard, useMentorQueue } from '@/features/mentoring'
import { useAttemptsForReview } from '../api/taskReviewsApi'
import { AttemptReviewCard } from '../components/AttemptReviewCard'
import { MentorReplyForm } from '../components/MentorReplyForm'

type Tab = 'mentoring' | 'reviews'

/** Mentor replies to student questions and review comments on attempts at the company's tasks. */
export default function IndustryFeedbackPage() {
  const [tab, setTab] = useState<Tab>('mentoring')
  const { data: questions, isLoading: qLoading } = useMentorQueue()
  const { data: attempts, isLoading: aLoading } = useAttemptsForReview()

  if (qLoading || aLoading) return <PageSpinner label="Loading…" />

  const waiting = questions?.filter((q) => !q.reply).length ?? 0
  const unreviewed = attempts?.filter((a) => a.reviews.length === 0).length ?? 0

  return (
    <>
      <PageHeader title="Mentoring and reviews" description="Answer student questions and give feedback on attempts at your company's tasks. Advice is practice support, not assessment." />
      <div className="mb-4">
        <SegmentedControl ariaLabel="Feedback type" value={tab} onChange={setTab}
          options={[{ value: 'mentoring', label: 'Mentor questions', count: waiting }, { value: 'reviews', label: 'Task attempts', count: unreviewed }]} />
      </div>

      <div className="max-w-4xl space-y-4">
        {tab === 'mentoring' ? (
          questions?.length === 0 ? <EmptyState icon={MessagesSquare} title="No questions yet" /> :
            questions?.map((q) => <MentorQuestionCard key={q.id} question={q} showStudent footer={!q.reply && <MentorReplyForm questionId={q.id} />} />)
        ) : (
          attempts?.length === 0 ? <EmptyState icon={MessagesSquare} title="No attempts yet" description="Student attempts on your company's tasks will appear here." /> :
            attempts?.map((a) => <AttemptReviewCard key={a.attemptId} attempt={a} />)
        )}
      </div>
    </>
  )
}
