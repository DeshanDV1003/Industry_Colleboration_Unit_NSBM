import { useState } from 'react'
import { ArrowRight, Compass, Gauge, RotateCcw } from 'lucide-react'
import { PageHeader } from '@/components/layout/PageHeader'
import { Button } from '@/components/ui/Button'
import { EmptyState } from '@/components/ui/EmptyState'
import { LinkButton } from '@/components/ui/LinkButton'
import { PageSpinner } from '@/components/ui/Spinner'
import { getErrorMessage } from '@/lib/http'
import { useBaseline } from '../api/baselineApi'
import { BaselineQuiz } from '../components/BaselineQuiz'
import { BaselineResultView } from '../components/BaselineResultView'

export default function BaselinePage() {
  const { data: baseline, isLoading, isError, error } = useBaseline()
  const [retaking, setRetaking] = useState(false)

  if (isLoading) return <PageSpinner label="Loading your baseline…" />
  if (isError || !baseline) return <EmptyState icon={Gauge} title="Baseline could not be loaded" description={getErrorMessage(error)} />

  if (baseline.careerPathId === null) {
    return (
      <EmptyState
        icon={Compass}
        title="Choose a career path first"
        description="The skills baseline asks starter questions for the role you want to prepare for."
        action={<LinkButton to="/pathways">Explore career roles</LinkButton>}
      />
    )
  }

  const result = baseline.latestResult
  const showQuiz = !result || retaking

  return (
    <>
      <PageHeader
        title="Skills baseline"
        description={`Starter questions for ${baseline.careerPathName}. Answer honestly — this snapshot only guides what to practise next.`}
        actions={!showQuiz && (
          <>
            <Button variant="secondary" onClick={() => setRetaking(true)}>
              <RotateCcw className="size-4" /> Retake
            </Button>
            <LinkButton to="/roadmap">
              View roadmap <ArrowRight className="size-4" />
            </LinkButton>
          </>
        )}
      />

      {showQuiz ? (
        <BaselineQuiz
          key={result?.attemptId ?? 'first'}
          questions={baseline.questions}
          onSubmitted={() => setRetaking(false)}
          onCancel={result ? () => setRetaking(false) : undefined}
        />
      ) : (
        <BaselineResultView result={result} />
      )}
    </>
  )
}
