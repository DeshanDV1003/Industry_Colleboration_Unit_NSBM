import type { ReactNode } from 'react'
import { Link } from 'react-router-dom'
import { Award, CheckCircle2, Info } from 'lucide-react'
import { ReviewComments } from '@/components/ReviewComments'
import { PageHeader } from '@/components/layout/PageHeader'
import { Badge } from '@/components/ui/Badge'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { PageSpinner } from '@/components/ui/Spinner'
import { formatDate } from '@/lib/format'
import { getErrorMessage } from '@/lib/http'
import { usePortfolio } from '../api/portfolioApi'

function Stat({ label, value, to }: { label: string; value: ReactNode; to: string }) {
  return (
    <Link to={to} className="rounded-card border border-line bg-surface p-4 shadow-card transition-colors hover:border-primary">
      <p className="font-display text-2xl font-bold text-heading">{value}</p>
      <p className="text-xs text-muted">{label}</p>
    </Link>
  )
}

/** Task activity, practice evidence and feedback in one place. */
export default function PortfolioPage() {
  const { data: p, isLoading, isError, error } = usePortfolio()

  if (isLoading) return <PageSpinner label="Gathering your portfolio…" />
  if (isError || !p) return <EmptyState icon={Award} title="Portfolio could not be loaded" description={getErrorMessage(error)} />

  return (
    <>
      <PageHeader
        title="Career portfolio"
        description={<><strong className="text-heading">{p.fullName}</strong> · {p.studentNo} · {p.programme}{p.careerPathName && <> · preparing for {p.careerPathName}</>}{p.headline && <> — “{p.headline}”</>}</>}
      />

      <div className="mb-6 grid gap-4 sm:grid-cols-3 xl:grid-cols-6">
        <Stat label="Roadmap steps done" value={`${p.roadmapCompleted}/${p.roadmapTotal}`} to="/roadmap" />
        <Stat label="Baseline score" value={p.baselineScorePercent === null ? '—' : `${p.baselineScorePercent}%`} to="/baseline" />
        <Stat label="Industry tasks tried" value={p.tasks.length} to="/tasks" />
        <Stat label="Interview answers" value={p.interview.softSkillsAnswered + p.interview.technicalAnswered} to="/interview-report" />
        <Stat label="Sprint cards done" value={p.sprintItemsDone} to="/team-simulation" />
        <Stat label="Applications sent" value={p.applicationsSent} to="/applications" />
      </div>

      <div className="grid gap-6 xl:grid-cols-[minmax(0,1fr)_380px]">
        <Card>
          <CardHeader title="Industry task activity" description="Best result per task, your knowledge check and company feedback." />
          <CardBody className="space-y-5">
            {p.tasks.length === 0 ? <p className="text-sm text-muted">No task attempts yet.</p> : p.tasks.map((t) => (
              <div key={t.taskId} className="border-b border-line pb-5 last:border-0 last:pb-0">
                <div className="flex flex-wrap items-start justify-between gap-2">
                  <div>
                    <Link to={`/tasks/${t.taskId}`} className="font-semibold">{t.title}</Link>
                    <p className="text-xs text-muted">{t.companyName} · {t.attempts} attempt{t.attempts === 1 ? '' : 's'}</p>
                  </div>
                  <div className="flex flex-wrap gap-1.5">
                    <Badge tone={t.bestPassedChecks === t.totalChecks ? 'success' : 'warning'}>Best {t.bestPassedChecks}/{t.totalChecks} checks</Badge>
                    {t.knowledgeCheckDone && <Badge tone="accent"><CheckCircle2 className="size-3" /> Explained</Badge>}
                  </div>
                </div>
                {t.reviews.length > 0 && <div className="mt-3"><ReviewComments reviews={t.reviews} /></div>}
              </div>
            ))}
          </CardBody>
        </Card>

        <div className="space-y-6">
          {p.lecturerNotes.length > 0 && (
            <Card>
              <CardHeader title="Lecturer feedback" />
              <CardBody className="space-y-3">
                {p.lecturerNotes.map((n) => (
                  <div key={n.createdAt} className="rounded-xl bg-primary-soft p-3 text-sm">
                    <p className="text-xs font-semibold text-primary">{n.lecturerName} · {formatDate(n.createdAt)}</p>
                    <p className="mt-1 text-heading">{n.text}</p>
                  </div>
                ))}
              </CardBody>
            </Card>
          )}
          <Card>
            <CardHeader title="Skills with practice signals" description="From baseline answers and industry tasks." />
            <CardBody>
              {p.practisedSkills.length === 0
                ? <p className="text-sm text-muted">None yet.</p>
                : <div className="flex flex-wrap gap-1.5">{p.practisedSkills.map((s) => <Badge key={s} tone="success">{s}</Badge>)}</div>}
              {p.listedSkills.length > 0 && (
                <>
                  <p className="mb-1.5 mt-4 text-xs font-semibold text-muted">Listed, not yet practised</p>
                  <div className="flex flex-wrap gap-1.5">{p.listedSkills.map((s) => <Badge key={s} tone="warning">{s}</Badge>)}</div>
                </>
              )}
            </CardBody>
          </Card>
          <Card>
            <CardHeader title="Interview practice" />
            <CardBody className="text-sm">
              <p>{p.interview.softSkillsAnswered} soft-skills and {p.interview.technicalAnswered} technical answers.</p>
              <p className="text-muted">Average topic coverage {p.interview.averageCoveragePercent}%.</p>
            </CardBody>
          </Card>
          <p className="flex gap-2 text-xs text-muted">
            <Info className="mt-0.5 size-3.5 shrink-0" /> Practice evidence shows your growth. It is not a certified measure of ability.
          </p>
        </div>
      </div>
    </>
  )
}
