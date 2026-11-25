import { useState } from 'react'
import { LineChart as LineChartIcon } from 'lucide-react'
import { PageHeader } from '@/components/layout/PageHeader'
import { EmptyState } from '@/components/ui/EmptyState'
import { PageSpinner } from '@/components/ui/Spinner'
import { controlClass } from '@/components/ui/controlClass'
import { cn } from '@/lib/cn'
import { getErrorMessage } from '@/lib/http'
import { useAnalytics } from '../api/analyticsApi'
import { BarList } from '../components/BarList'
import { ChartCard, DataTable } from '../components/ChartCard'
import { LineChart } from '../components/LineChart'
import { PipelineBars } from '../components/PipelineBars'
import { StatTile } from '../components/StatTile'
import { PERIODS, PIPELINE_STAGES, SERIES, SINGLE, formatDay, percent } from '../config'

/** Trends for the Unit head and lecturers. Counts and averages over groups only; nothing ranks a student. */
export default function AnalyticsPage() {
  const [weeks, setWeeks] = useState<number>(12)
  const { data, isLoading, isError, error, isFetching } = useAnalytics(weeks)

  if (isLoading) return <PageSpinner label="Loading analytics…" />
  if (isError || !data) return <EmptyState icon={LineChartIcon} title="Analytics could not be loaded" description={getErrorMessage(error)} />

  const x = data.activity.map((w) => w.weekStart)
  const withheld = `Fewer than ${data.minGroupSize} people — hidden to protect privacy`
  const booked = data.halls.reduce((sum, h) => sum + h.bookedHours, 0)
  const available = data.halls.reduce((sum, h) => sum + h.availableHours, 0)

  return (
    <>
      <PageHeader title="Analytics" description="Trends across the platform. Group totals only — no student is identified, scored or ranked." />

      <div className="mb-6 flex flex-wrap items-center gap-3">
        <label htmlFor="an-period" className="text-sm font-medium text-heading">Period</label>
        <select id="an-period" className={cn(controlClass(false), 'h-9 w-auto')} value={weeks} onChange={(e) => setWeeks(Number(e.target.value))}>
          {PERIODS.map((p) => <option key={p} value={p}>Last {p} weeks</option>)}
        </select>
        {isFetching && <span className="text-xs text-muted">Updating…</span>}
      </div>

      <div className={cn('space-y-6 transition-opacity', isFetching && 'opacity-70')}>
        <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
          <StatTile label="Students practising" value={data.activeStudentsInPeriod} context={`of ${data.students} students, in the last ${data.weeks} weeks`} />
          <StatTile label="Internship applications" value={data.applicationsInPeriod} context={`in the last ${data.weeks} weeks`} />
          <StatTile label="Event attendance" value={eventRate(data.events, data.minGroupSize)} context="of registrations at finished partner events" />
          <StatTile label="Hall use" value={`${percent(booked, available)}%`} context={`of teaching hours booked, next ${data.upcomingHallWeeks} weeks`} />
        </div>

        <div className="grid gap-6 xl:grid-cols-2">
          <ChartCard title="Students practising each week" description="Students who did any practice: a task, an interview answer, a baseline, a check or a roadmap step."
            table={<DataTable head={['Week of', 'Students']} rows={data.activity.map((w) => [formatDay(w.weekStart), w.activeStudents])} />}>
            <LineChart x={x} unit={['student', 'students']} series={[{ name: 'Students practising', values: data.activity.map((w) => w.activeStudents), color: SINGLE }]} />
          </ChartCard>

          <ChartCard title="Practice by kind" description="Industry task attempts and interview answers submitted each week."
            table={<DataTable head={['Week of', 'Task attempts', 'Interview answers']} rows={data.activity.map((w) => [formatDay(w.weekStart), w.taskAttempts, w.interviewAnswers])} />}>
            <LineChart x={x} series={[
              { name: 'Task attempts', values: data.activity.map((w) => w.taskAttempts), color: SERIES[0] },
              { name: 'Interview answers', values: data.activity.map((w) => w.interviewAnswers), color: SERIES[1] },
            ]} />
          </ChartCard>

          <ChartCard title="Internship pipeline by company" description={`Applications submitted in the last ${data.weeks} weeks and where they stand now.`}
            empty={data.pipeline.length === 0 ? 'No applications in this period.' : null}
            table={<DataTable head={['Company', ...PIPELINE_STAGES.map((s) => s.label), 'Total']}
              rows={data.pipeline.map((p) => [p.company, ...PIPELINE_STAGES.map((s) => p[s.key]), p.total])} />}>
            <PipelineBars rows={data.pipeline} />
          </ChartCard>

          <ChartCard title="Event attendance" description="Share of registered students who attended each finished partner event."
            empty={data.events.length === 0 ? 'No partner events finished in this period.' : null}
            table={<DataTable head={['Event', 'Date', 'Registered', 'Attended']}
              rows={data.events.map((e) => [e.event, formatDay(e.date), e.registered, e.attended ?? 'hidden'])} />}>
            <BarList withheld={withheld} rows={data.events.map((e) => ({
              label: e.event,
              percent: e.attended === null ? null : percent(e.attended, e.registered),
              detail: e.attended === null ? `${e.registered} registered` : `${e.attended} of ${e.registered} attended`,
            }))} />
          </ChartCard>

          <ChartCard title={`Hall use, next ${data.upcomingHallWeeks} weeks`} description="Booked hours as a share of teaching hours (07:00–21:00, weekdays)."
            empty={data.halls.length === 0 ? 'No venues yet.' : null}
            table={<DataTable head={['Venue', 'Booked hours', 'Teaching hours']} rows={data.halls.map((h) => [h.venue, h.bookedHours, h.availableHours])} />}>
            <BarList withheld={withheld} rows={data.halls.map((h) => ({
              label: h.venue, percent: percent(h.bookedHours, h.availableHours), detail: `${h.bookedHours} of ${h.availableHours} hours booked`,
            }))} />
          </ChartCard>

          <ChartCard title="Roadmap progress by career path" description="Average share of roadmap steps done by the students on each path."
            empty={data.readiness.length === 0 ? 'No student has chosen a career path yet.' : null}
            table={<DataTable head={['Career path', 'Students', 'Baseline taken', 'Average roadmap']}
              rows={data.readiness.map((r) => [r.careerPath, r.students, r.baselineTaken ?? 'hidden', r.averageRoadmapPercent === null ? 'hidden' : `${r.averageRoadmapPercent}%`])} />}>
            <BarList withheld={withheld} rows={data.readiness.map((r) => ({
              label: r.careerPath,
              percent: r.averageRoadmapPercent,
              detail: r.baselineTaken === null ? `${r.students} student${r.students === 1 ? '' : 's'}` : `${r.students} students · ${r.baselineTaken} took the baseline`,
            }))} />
          </ChartCard>
        </div>
      </div>
    </>
  )
}

function eventRate(events: { registered: number; attended: number | null }[], minGroup: number): string {
  const shown = events.filter((e) => e.attended !== null)
  const registered = shown.reduce((s, e) => s + e.registered, 0)
  if (registered < minGroup) return '—'
  return `${percent(shown.reduce((s, e) => s + (e.attended ?? 0), 0), registered)}%`
}
