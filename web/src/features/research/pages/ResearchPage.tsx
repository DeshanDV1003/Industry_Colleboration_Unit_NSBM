import { BarChart3, Info } from 'lucide-react'
import { ApplicationStatusBadge } from '@/components/ApplicationStatusBadge'
import { PageHeader } from '@/components/layout/PageHeader'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { ProgressBar } from '@/components/ui/ProgressBar'
import { PageSpinner } from '@/components/ui/Spinner'
import { getErrorMessage } from '@/lib/http'
import { useResearchOverview } from '../api/researchApi'

function Counter({ label, value, hint }: { label: string; value: number | string; hint?: string }) {
  return (
    <Card className="p-4">
      <p className="font-display text-2xl font-bold text-heading">{value}</p>
      <p className="text-xs font-medium text-heading">{label}</p>
      {hint && <p className="text-xs text-muted">{hint}</p>}
    </Card>
  )
}

/** Sample counters for research planning (RQ1–RQ4). Shared by lecturers and Unit admins. */
export default function ResearchPage() {
  const { data: o, isLoading, isError, error } = useResearchOverview()

  if (isLoading) return <PageSpinner label="Counting…" />
  if (isError || !o) return <EmptyState icon={BarChart3} title="Analytics could not be loaded" description={getErrorMessage(error)} />

  return (
    <>
      <PageHeader title="Research analytics" description="Fictional group activity and example pilot measures to help plan research. These are demo values, not findings." />

      <div className="mb-6 grid gap-4 sm:grid-cols-2 lg:grid-cols-4 2xl:grid-cols-8">
        <Counter label="Students" value={o.students} hint={`${o.studentsWithCareerPath} chose a path`} />
        <Counter label="Baseline attempts" value={o.baselineAttempts} hint={o.baselineAttempts === 0 ? 'no data yet' : `average ${o.averageBaselinePercent}%`} />
        <Counter label="Approved tasks" value={o.approvedTasks} />
        <Counter label="Task attempts" value={o.taskAttempts} hint={`${o.taskAttemptsAllChecksPassed} passed every check`} />
        <Counter label="Knowledge checks" value={o.knowledgeChecks} />
        <Counter label="Interview answers" value={o.interviewAnswers} />
        <Counter label="Mentor questions" value={o.mentorQuestions} hint={`${o.mentorQuestionsAnswered} answered`} />
        <Counter label="Sprint cards done" value={o.sprintItemsDone} />
      </div>

      <div className="grid gap-6 lg:grid-cols-3">
        <Card>
          <CardHeader title="Students by career path" />
          <CardBody className="space-y-3">
            {o.studentsByCareerPath.map((p) => (
              <div key={p.label}>
                <div className="flex justify-between text-sm"><span className="text-heading">{p.label}</span><span className="text-muted">{p.count}</span></div>
                <ProgressBar label={`${p.label} students`} value={o.students ? (p.count / o.students) * 100 : 0} />
              </div>
            ))}
          </CardBody>
        </Card>

        <Card>
          <CardHeader title="RQ1 · Most common skill gaps" description="Core skills students on that path have neither listed nor practised." />
          <CardBody>
            {o.topSkillGaps.length === 0 ? <p className="text-sm text-muted">No gaps found.</p> : (
              <ul className="space-y-3">
                {o.topSkillGaps.map((g) => (
                  <li key={g.skillName + g.careerPathName} className="text-sm">
                    <div className="flex justify-between gap-2"><span className="font-medium text-heading">{g.skillName}</span><span className="text-muted">{g.studentsMissing}/{g.studentsOnPath}</span></div>
                    <p className="text-xs text-muted">{g.careerPathName}</p>
                  </li>
                ))}
              </ul>
            )}
          </CardBody>
        </Card>

        <Card>
          <CardHeader title="RQ4 · Applications by status" description={`${o.approvedVacancies} open vacancies`} />
          <CardBody>
            <ul className="space-y-2">
              {o.applicationsByStatus.map((a) => (
                <li key={a.status} className="flex items-center justify-between"><ApplicationStatusBadge status={a.status} /><span className="font-semibold text-heading">{a.count}</span></li>
              ))}
            </ul>
          </CardBody>
        </Card>
      </div>

      <p className="mt-4 flex gap-2 text-xs text-muted">
        <Info className="mt-0.5 size-3.5 shrink-0" /> A real pilot needs consent, suitable tests and expert-checked rules. Scores and examples here are not research results.
      </p>
    </>
  )
}
