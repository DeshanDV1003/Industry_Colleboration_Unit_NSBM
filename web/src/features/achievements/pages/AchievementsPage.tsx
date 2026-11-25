import { Award, Mountain, Trophy } from 'lucide-react'
import { PageHeader } from '@/components/layout/PageHeader'
import { Card, CardBody } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { LinkButton } from '@/components/ui/LinkButton'
import { ProgressBar } from '@/components/ui/ProgressBar'
import { PageSpinner } from '@/components/ui/Spinner'
import { cn } from '@/lib/cn'
import { getErrorMessage } from '@/lib/http'
import { useMyAchievements } from '../api/achievementsApi'
import { BadgesCard } from '../components/BadgesCard'
import { ProfileCardSharing } from '../components/ProfileCardSharing'
import { StreakCard } from '../components/StreakCard'
import { LEVEL_COUNT, LEVEL_META } from '../config'

export default function AchievementsPage() {
  const { data, isLoading, isError, error } = useMyAchievements()

  if (isLoading) return <PageSpinner label="Loading achievements…" />
  if (isError || !data) return <EmptyState icon={Trophy} title="Achievements could not be loaded" description={getErrorMessage(error)} />

  const { progress } = data
  const level = LEVEL_META[progress.level]
  const roadmapPercent = progress.roadmapTotal === 0 ? 0 : Math.round((progress.roadmapCompleted / progress.roadmapTotal) * 100)

  return (
    <>
      <PageHeader title="Achievements" description="Your practice streak, roadmap level and skill badges. Practice evidence, not a formal qualification." />
      <div className="grid gap-6 xl:grid-cols-[minmax(0,1fr)_380px]">
        <div className="space-y-6">
          <StreakCard progress={progress} />
          <BadgesCard data={data} />
        </div>
        <div className="space-y-6">
          <Card>
            <CardBody className="space-y-3 text-sm">
              <p className="flex items-center gap-2 text-xs font-semibold uppercase tracking-wide text-muted"><Mountain className="size-4" /> Roadmap level</p>
              <p className="font-display text-xl font-bold text-heading">{level.label}</p>
              <div className="flex gap-1" aria-hidden>
                {Array.from({ length: LEVEL_COUNT }, (_, i) => <span key={i} className={cn('h-2 flex-1 rounded-full', i < level.rank ? 'bg-primary' : 'bg-surface-subtle')} />)}
              </div>
              <p className="text-muted">{level.description}. {progress.roadmapCompleted}/{progress.roadmapTotal} steps.</p>
              <ProgressBar value={roadmapPercent} label="Roadmap progress" />
              {progress.roadmapTotal === 0 && <LinkButton to="/pathways" variant="secondary">Choose a career path</LinkButton>}
            </CardBody>
          </Card>
          <Card>
            <CardBody className="flex items-center justify-between gap-3 text-sm">
              <span className="flex items-center gap-2"><Award className="size-5 text-accent" /> {data.certificateCount} event certificate{data.certificateCount === 1 ? '' : 's'}</span>
              <LinkButton to="/certificates" variant="secondary">View</LinkButton>
            </CardBody>
          </Card>
          <ProfileCardSharing settings={data.profileCard} />
        </div>
      </div>
    </>
  )
}
