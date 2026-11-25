import { useState } from 'react'
import { Link } from 'react-router-dom'
import { Info, Search, Users } from 'lucide-react'
import { PageHeader } from '@/components/layout/PageHeader'
import { Card } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { ProgressBar } from '@/components/ui/ProgressBar'
import { PageSpinner } from '@/components/ui/Spinner'
import { controlClass } from '@/components/ui/controlClass'
import { cn } from '@/lib/cn'
import { formatDate } from '@/lib/format'
import { getErrorMessage } from '@/lib/http'
import { useStudentDirectory } from '../api/lecturerApi'
import { SupportSignalBadges } from '../components/SupportSignalBadges'

export default function StudentDirectoryPage() {
  const [search, setSearch] = useState('')
  const [followUp, setFollowUp] = useState(false)
  const { data: students, isLoading, isError, error, isFetching } = useStudentDirectory(search, followUp)

  return (
    <>
      <PageHeader title="Student directory" description="Find students by name, ID or career role and see where they may need support. Students are listed by name — never ranked." />

      <div className="mb-4 flex flex-wrap items-center gap-3">
        <label className="relative min-w-[260px] flex-1">
          <span className="sr-only">Search students</span>
          <Search className="pointer-events-none absolute left-3.5 top-1/2 size-4 -translate-y-1/2 text-muted" />
          <input type="search" value={search} onChange={(e) => setSearch(e.target.value)} placeholder="Name, student ID (e.g. DEMO-001) or career role"
            className={cn(controlClass(false), 'h-10 pl-10')} />
        </label>
        <label className="flex items-center gap-2 text-sm font-medium text-heading">
          <input type="checkbox" checked={followUp} onChange={(e) => setFollowUp(e.target.checked)} className="size-4 accent-[var(--color-primary)]" />
          Only students who may need follow-up
        </label>
      </div>

      {isLoading ? <PageSpinner label="Loading students…" /> : isError ? (
        <EmptyState icon={Users} title="Students could not be loaded" description={getErrorMessage(error)} />
      ) : students?.length === 0 ? (
        <EmptyState icon={Users} title="No students match" description="Try a different search or clear the follow-up filter." />
      ) : (
        <Card className={cn('overflow-hidden', isFetching && 'opacity-70')}>
          <table className="w-full text-sm">
            <thead className="border-b border-line bg-surface-subtle text-left text-xs font-semibold uppercase tracking-wider text-muted">
              <tr>
                <th className="px-5 py-3">Student</th>
                <th className="hidden px-5 py-3 md:table-cell">Career role</th>
                <th className="hidden px-5 py-3 lg:table-cell">Tasks</th>
                <th className="hidden px-5 py-3 lg:table-cell">Roadmap</th>
                <th className="hidden px-5 py-3 xl:table-cell">Last activity</th>
                <th className="px-5 py-3">Support signals</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-line">
              {students?.map((s) => (
                <tr key={s.id} className="align-top">
                  <td className="px-5 py-3.5">
                    <Link to={`/lecturer-insights?student=${s.id}`} className="font-semibold">{s.name}</Link>
                    <p className="text-xs text-muted">{s.studentNo} · {s.programme}</p>
                  </td>
                  <td className="hidden px-5 py-3.5 md:table-cell">{s.careerPathName ?? <span className="text-muted">Not chosen</span>}</td>
                  <td className="hidden px-5 py-3.5 lg:table-cell">{s.tasksAttempted}</td>
                  <td className="hidden px-5 py-3.5 lg:table-cell">
                    <div className="w-28"><ProgressBar label={`${s.name} roadmap progress`} value={s.roadmapPercent} /><span className="text-xs text-muted">{s.roadmapPercent}%</span></div>
                  </td>
                  <td className="hidden px-5 py-3.5 text-muted xl:table-cell">{s.lastActivityAt ? formatDate(s.lastActivityAt) : '—'}</td>
                  <td className="px-5 py-3.5"><SupportSignalBadges signals={s.signals} /></td>
                </tr>
              ))}
            </tbody>
          </table>
        </Card>
      )}
      <p className="mt-3 flex gap-2 text-xs text-muted">
        <Info className="mt-0.5 size-3.5 shrink-0" /> Support signals are simple example rules to help you decide whom to contact. They are not judgements of a student.
      </p>
    </>
  )
}
