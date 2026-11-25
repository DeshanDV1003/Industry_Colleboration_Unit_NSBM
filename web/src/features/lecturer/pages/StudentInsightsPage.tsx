import { useState } from 'react'
import { Link, useSearchParams } from 'react-router-dom'
import { ArrowLeft, CheckCircle2, UserSearch } from 'lucide-react'
import { PageHeader } from '@/components/layout/PageHeader'
import { Badge } from '@/components/ui/Badge'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { LinkButton } from '@/components/ui/LinkButton'
import { SegmentedControl } from '@/components/ui/SegmentedControl'
import { PageSpinner } from '@/components/ui/Spinner'
import { controlClass } from '@/components/ui/controlClass'
import { cn } from '@/lib/cn'
import { formatDate, formatDateTime } from '@/lib/format'
import { getErrorMessage } from '@/lib/http'
import { useStudentDirectory, useStudentInsights, type StudentInsights } from '../api/lecturerApi'
import { InterviewReviewSection } from '../components/InterviewReviewSection'
import { LecturerActions } from '../components/LecturerActions'
import { SupportSignalBadges } from '../components/SupportSignalBadges'

type Tab = 'practice' | 'interview' | 'notes'

function Fact({ label, value }: { label: string; value: string }) {
  return <div><p className="text-xs text-muted">{label}</p><p className="font-semibold text-heading">{value}</p></div>
}

function InsightsView({ s }: { s: StudentInsights }) {
  const [tab, setTab] = useState<Tab>('practice')

  return (
    <div className="grid gap-6 xl:grid-cols-[minmax(0,1fr)_360px]">
      <div className="space-y-6">
        <Card>
          <CardBody className="space-y-4">
            <div className="flex flex-wrap items-start justify-between gap-3">
              <div>
                <h2 className="text-xl font-semibold">{s.name}</h2>
                <p className="text-sm text-muted">{s.studentNo} · {s.programme} · Batch {s.batch}</p>
              </div>
              <SupportSignalBadges signals={s.signals} />
            </div>
            <div className="grid grid-cols-2 gap-4 sm:grid-cols-4">
              <Fact label="Career role" value={s.careerPathName ?? 'Not chosen'} />
              <Fact label="Baseline" value={s.baselinePercent === null ? '—' : `${s.baselinePercent}%`} />
              <Fact label="Roadmap" value={`${s.roadmapCompleted}/${s.roadmapTotal} steps`} />
              <Fact label="Tasks tried" value={String(s.attempts.length)} />
            </div>
            <div className="flex flex-wrap gap-1.5">
              {s.practisedSkills.map((k) => <Badge key={k} tone="success">{k}</Badge>)}
              {s.listedOnlySkills.map((k) => <Badge key={k} tone="warning">{k} (listed)</Badge>)}
            </div>
          </CardBody>
        </Card>

        <SegmentedControl ariaLabel="Section" value={tab} onChange={setTab}
          options={[{ value: 'practice', label: 'Practice record' }, { value: 'interview', label: 'Interview practice' }, { value: 'notes', label: 'Notes & assignments', count: s.notes.length + s.assignments.length }]} />

        {tab === 'practice' && (
          <>
            <Card>
              <CardHeader title="Task attempts" />
              <CardBody>
                {s.attempts.length === 0 ? <p className="text-sm text-muted">No task attempts yet.</p> : (
                  <ul className="divide-y divide-line">
                    {s.attempts.map((a) => (
                      <li key={a.taskId} className="flex flex-wrap items-center justify-between gap-2 py-2.5 text-sm">
                        <span className="font-medium text-heading">{a.taskTitle}</span>
                        <span className="flex items-center gap-2 text-xs text-muted">
                          {a.attempts} attempt{a.attempts === 1 ? '' : 's'} · last {formatDate(a.lastSubmittedAt)}
                          <Badge tone={a.bestPassedChecks === a.totalChecks ? 'success' : 'warning'}>{a.bestPassedChecks}/{a.totalChecks}</Badge>
                          {a.knowledgeCheckDone && <CheckCircle2 className="size-4 text-accent" aria-label="Knowledge check done" />}
                        </span>
                      </li>
                    ))}
                  </ul>
                )}
              </CardBody>
            </Card>
            {s.knowledgeChecks.map((k) => (
              <Card key={k.taskTitle}>
                <CardHeader title={`Knowledge check · ${k.taskTitle}`} description={formatDateTime(k.submittedAt)} />
                <CardBody className="space-y-3 text-sm">
                  {k.answers.map((a) => <div key={a.prompt}><p className="font-medium text-heading">{a.prompt}</p><p className="whitespace-pre-line">{a.answer}</p></div>)}
                </CardBody>
              </Card>
            ))}
            <Card>
              <CardHeader title="Recent activity" />
              <CardBody>
                {s.recentActivity.length === 0 ? <p className="text-sm text-muted">No activity yet.</p> : (
                  <ul className="space-y-1.5 text-sm">{s.recentActivity.map((a) => <li key={a.at + a.description}><span className="text-xs text-muted">{formatDateTime(a.at)}</span> · {a.description}</li>)}</ul>
                )}
              </CardBody>
            </Card>
          </>
        )}

        {tab === 'interview' && <Card><CardBody><InterviewReviewSection studentId={s.id} /></CardBody></Card>}

        {tab === 'notes' && (
          <>
            <Card>
              <CardHeader title="Support notes" />
              <CardBody>
                {s.notes.length === 0 ? <p className="text-sm text-muted">No notes yet.</p> : (
                  <ul className="space-y-3">{s.notes.map((n) => <li key={n.id} className="rounded-xl border border-line p-3 text-sm"><p className="text-xs text-muted">{n.lecturerName} · {formatDateTime(n.createdAt)}</p><p className="mt-1">{n.text}</p></li>)}</ul>
                )}
              </CardBody>
            </Card>
            <Card>
              <CardHeader title="Assigned tasks" />
              <CardBody>
                {s.assignments.length === 0 ? <p className="text-sm text-muted">No tasks assigned.</p> : (
                  <ul className="space-y-2 text-sm">{s.assignments.map((a) => <li key={a.id}><span className="font-medium text-heading">{a.taskTitle}</span> <span className="text-xs text-muted">· {a.lecturerName}, {formatDate(a.assignedAt)}</span>{a.note && <p className="text-muted">“{a.note}”</p>}</li>)}</ul>
                )}
              </CardBody>
            </Card>
          </>
        )}
      </div>

      <aside><LecturerActions student={s} /></aside>
    </div>
  )
}

/** Pick a student (?student=id) and see their practice record, interview answers and notes. */
export default function StudentInsightsPage() {
  const [params, setParams] = useSearchParams()
  const studentId = Number(params.get('student')) || null
  const { data: students } = useStudentDirectory('', false)
  const insights = useStudentInsights(studentId)

  return (
    <>
      <Link to="/lecturer" className="mb-4 inline-flex items-center gap-1.5 text-sm font-semibold"><ArrowLeft className="size-4" /> Student directory</Link>
      <PageHeader
        title="Student details"
        description="Task attempts, checks, skill signals and recent activity — for guidance, not a final judgement."
        actions={
          <select aria-label="Choose a student" value={studentId ?? ''} onChange={(e) => setParams({ student: e.target.value })}
            className={cn(controlClass(false), 'h-10 w-72')}>
            <option value="">Choose a student…</option>
            {students?.map((s) => <option key={s.id} value={s.id}>{s.name} · {s.studentNo}{s.careerPathName ? ` · ${s.careerPathName}` : ''}</option>)}
          </select>
        }
      />
      {studentId === null ? (
        <EmptyState icon={UserSearch} title="Choose a student" description="Pick a student by name, ID or career role." action={<LinkButton to="/lecturer" variant="secondary">Open the directory</LinkButton>} />
      ) : insights.isLoading ? <PageSpinner label="Loading student details…" /> : insights.isError || !insights.data ? (
        <EmptyState icon={UserSearch} title="Student not found" description={getErrorMessage(insights.error)} />
      ) : (
        <InsightsView key={insights.data.id} s={insights.data} />
      )}
    </>
  )
}
