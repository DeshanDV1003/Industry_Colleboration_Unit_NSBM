import { Compass, Map } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { PageHeader } from '@/components/layout/PageHeader'
import { Badge } from '@/components/ui/Badge'
import { Card, CardBody } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { LinkButton } from '@/components/ui/LinkButton'
import { ProgressBar } from '@/components/ui/ProgressBar'
import { PageSpinner } from '@/components/ui/Spinner'
import { LEARNING_STAGES, LEARNING_STAGE_META } from '@/config/learning'
import { getErrorMessage } from '@/lib/http'
import { useRoadmap, useSetStepCompletion } from '../api/roadmapApi'
import { RoadmapStepItem } from '../components/RoadmapStepItem'

export default function RoadmapPage() {
  const { data: roadmap, isLoading, isError, error } = useRoadmap()
  const setCompletion = useSetStepCompletion()

  if (isLoading) return <PageSpinner label="Loading your roadmap…" />
  if (isError || !roadmap) return <EmptyState icon={Map} title="Roadmap could not be loaded" description={getErrorMessage(error)} />

  if (roadmap.careerPathId === null) {
    return (
      <EmptyState
        icon={Compass}
        title="Choose a career path first"
        description="Your roadmap is a step-by-step plan for the role you choose."
        action={<LinkButton to="/pathways">Explore career roles</LinkButton>}
      />
    )
  }

  const toggle = (stepId: number, completed: boolean) =>
    setCompletion.mutate({ stepId, completed }, { onError: (e) => toast.error(getErrorMessage(e)) })

  return (
    <>
      <PageHeader
        title="Learning roadmap"
        description={`Your step-by-step plan for ${roadmap.careerPathName}, from beginner to advanced. Tick a step when you feel confident with it.`}
      />

      <Card className="mb-6">
        <CardBody className="flex flex-wrap items-center gap-6">
          <div>
            <p className="font-display text-4xl font-bold text-heading">{roadmap.progressPercent}%</p>
            <p className="text-sm text-muted">{roadmap.completedCount} of {roadmap.totalCount} steps done</p>
          </div>
          <ProgressBar className="min-w-[220px] flex-1" label="Roadmap progress" value={roadmap.progressPercent} />
        </CardBody>
      </Card>

      <div className="grid gap-6 lg:grid-cols-3">
        {LEARNING_STAGES.map((stage) => {
          const steps = roadmap.steps.filter((s) => s.stage === stage)
          if (steps.length === 0) return null
          const meta = LEARNING_STAGE_META[stage]

          return (
            <section key={stage}>
              <div className="mb-3 flex items-center gap-2">
                <Badge tone={meta.tone}>{meta.label}</Badge>
                <span className="text-xs text-muted">
                  {steps.filter((s) => s.isCompleted).length}/{steps.length} done
                </span>
              </div>
              <ol>
                {steps.map((step, index) => (
                  <RoadmapStepItem
                    key={step.id}
                    step={step}
                    isLast={index === steps.length - 1}
                    onToggle={(completed) => toggle(step.id, completed)}
                  />
                ))}
              </ol>
            </section>
          )
        })}
      </div>
    </>
  )
}
