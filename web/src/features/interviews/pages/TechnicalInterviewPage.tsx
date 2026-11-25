import { useState } from 'react'
import { ArrowLeft, BookOpenCheck, Compass } from 'lucide-react'
import { PageHeader } from '@/components/layout/PageHeader'
import { EmptyState } from '@/components/ui/EmptyState'
import { LinkButton } from '@/components/ui/LinkButton'
import { PageSpinner } from '@/components/ui/Spinner'
import { controlClass } from '@/components/ui/controlClass'
import { useCareerPaths } from '@/features/careers'
import { cn } from '@/lib/cn'
import { getErrorMessage } from '@/lib/http'
import { useInterviewSession } from '../api/interviewsApi'
import { QuestionPractice } from '../components/QuestionPractice'

/** Stage 2: technical interview practice for a chosen career role (defaults to the student's path). */
export default function TechnicalInterviewPage() {
  const { data: paths } = useCareerPaths()
  const [pathId, setPathId] = useState<number | null>(null)
  const { data: session, isLoading, isError, error } = useInterviewSession('Technical', pathId)

  if (isLoading) return <PageSpinner label="Loading questions…" />
  if (isError || !session) return <EmptyState icon={BookOpenCheck} title="Questions could not be loaded" description={getErrorMessage(error)} />

  const roleSelect = (
    <select aria-label="Career role" value={session.careerPathId ?? ''} onChange={(e) => setPathId(Number(e.target.value) || null)}
      className={cn(controlClass(false), 'h-10 w-56')}>
      {session.careerPathId === null && <option value="">Choose a role…</option>}
      {paths?.map((p) => <option key={p.id} value={p.id}>{p.name}</option>)}
    </select>
  )

  return (
    <>
      <PageHeader
        title="Stage 2 · Technical interview"
        description="Answer technical and practical questions for a career role. You get basic topic feedback and can try again any time."
        actions={<>
          <LinkButton to="/interview-prep" variant="secondary"><ArrowLeft className="size-4" /> Stage 1</LinkButton>
          {roleSelect}
        </>}
      />

      {session.careerPathId === null ? (
        <EmptyState icon={Compass} title="Choose a career role" description="Pick a role above, or choose your career path first." action={<LinkButton to="/pathways">Explore career roles</LinkButton>} />
      ) : (
        <div className="max-w-4xl space-y-4">
          {session.questions.map((q, i) => <QuestionPractice key={q.id} question={q} index={i} />)}
          <LinkButton to="/interview-report" variant="secondary">View interview report</LinkButton>
        </div>
      )}
    </>
  )
}
