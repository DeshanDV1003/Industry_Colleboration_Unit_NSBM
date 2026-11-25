import type { ReactNode } from 'react'
import { Link } from 'react-router-dom'
import { ArrowRight, Compass, Gauge, Map } from 'lucide-react'
import { Badge } from '@/components/ui/Badge'
import { Card } from '@/components/ui/Card'
import { LinkButton } from '@/components/ui/LinkButton'
import { ProgressBar } from '@/components/ui/ProgressBar'
import { Spinner } from '@/components/ui/Spinner'
import { CareerIcon } from '@/features/careers'
import { formatDate } from '@/lib/format'
import { useStudentOverview } from '../api/dashboardApi'

interface OverviewCardProps {
  title: string
  icon: typeof Compass
  link?: { to: string; label: string }
  children: ReactNode
}

function OverviewCard({ title, icon: Icon, link, children }: OverviewCardProps) {
  return (
    <Card className="flex flex-col p-6">
      <div className="flex items-center justify-between">
        <p className="flex items-center gap-2 text-xs font-bold uppercase tracking-wider text-muted">
          <Icon className="size-4" /> {title}
        </p>
        {link && (
          <Link to={link.to} className="flex items-center gap-1 text-xs font-semibold">
            {link.label} <ArrowRight className="size-3.5" />
          </Link>
        )}
      </div>
      <div className="mt-4 flex flex-1 flex-col">{children}</div>
    </Card>
  )
}

export function StudentOverviewCards() {
  const { data: overview, isLoading } = useStudentOverview()

  if (isLoading || !overview) return <Card className="grid h-44 place-items-center"><Spinner /></Card>

  const { careerPath, roadmap, baseline } = overview

  if (!careerPath) {
    return (
      <Card className="flex flex-wrap items-center justify-between gap-4 p-6">
        <div>
          <h3 className="text-base font-semibold">Start by choosing a career path</h3>
          <p className="mt-1 text-muted">Your baseline, roadmap and practice tasks all follow the role you pick.</p>
        </div>
        <LinkButton to="/pathways"><Compass className="size-4" /> Explore career roles</LinkButton>
      </Card>
    )
  }

  return (
    <div className="grid gap-5 lg:grid-cols-3">
      <OverviewCard title="Career goal" icon={Compass} link={{ to: '/pathways', label: 'Change' }}>
        <div className="flex items-center gap-3">
          <span className="grid size-11 place-items-center rounded-xl bg-brand text-white"><CareerIcon slug={careerPath.slug} className="size-5" /></span>
          <div>
            <p className="text-lg font-semibold text-heading">{careerPath.name}</p>
            {careerPath.selectedAt && <p className="text-xs text-muted">Chosen {formatDate(careerPath.selectedAt)}</p>}
          </div>
        </div>
        <p className="mt-3 text-sm text-muted">{careerPath.tagline}</p>
      </OverviewCard>

      <OverviewCard title="Roadmap" icon={Map} link={{ to: '/roadmap', label: 'Open' }}>
        <div className="flex items-end gap-2">
          <span className="font-display text-3xl font-bold text-heading">{roadmap.percent}%</span>
          <span className="pb-1 text-sm text-muted">{roadmap.completed} of {roadmap.total} steps</span>
        </div>
        <ProgressBar className="mt-3" label="Roadmap progress" value={roadmap.percent} />
        <p className="mt-auto pt-3 text-sm">
          {roadmap.nextStepTitle
            ? <><span className="text-muted">Next: </span><span className="font-medium text-heading">{roadmap.nextStepTitle}</span></>
            : <span className="font-medium text-success">All steps done — great work!</span>}
        </p>
      </OverviewCard>

      <OverviewCard title="Skills baseline" icon={Gauge} link={baseline ? { to: '/baseline', label: 'Details' } : undefined}>
        {baseline ? (
          <>
            <div className="flex items-end gap-2">
              <span className="font-display text-3xl font-bold text-heading">{baseline.scorePercent}%</span>
              <span className="pb-1 text-sm text-muted">on {formatDate(baseline.completedAt)}</span>
            </div>
            <div className="mt-3">
              <p className="text-xs font-semibold text-muted">Focus next on</p>
              <div className="mt-1.5 flex flex-wrap gap-1.5">
                {baseline.focusSkills.length > 0
                  ? baseline.focusSkills.map((s) => <Badge key={s} tone="danger">{s}</Badge>)
                  : <Badge tone="success">No weak topics — keep practising</Badge>}
              </div>
            </div>
          </>
        ) : (
          <>
            <p className="text-sm text-muted">Answer a few starter questions to see your strong and weak topics.</p>
            <LinkButton to="/baseline" className="mt-auto self-start">Take the baseline</LinkButton>
          </>
        )}
      </OverviewCard>
    </div>
  )
}
