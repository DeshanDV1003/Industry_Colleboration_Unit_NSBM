import { useParams } from 'react-router-dom'
import { Award, ClipboardList, Info } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { PageHeader } from '@/components/layout/PageHeader'
import { Badge } from '@/components/ui/Badge'
import { Button } from '@/components/ui/Button'
import { Card } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { PageSpinner } from '@/components/ui/Spinner'
import { formatRange } from '@/features/calendar'
import { getErrorMessage } from '@/lib/http'
import { useEventAttendance, useIssueCertificates, useSetAttendance } from '../api/industryEventsApi'

/** The host marks who attended, then issues verifiable certificates once the event has ended. */
export default function EventAttendancePage() {
  const eventId = Number(useParams().id)
  const { data, isLoading, isError, error } = useEventAttendance(eventId)
  const setAttendance = useSetAttendance()
  const issue = useIssueCertificates()

  if (isLoading) return <PageSpinner label="Loading attendance…" />
  if (isError || !data) return <EmptyState icon={ClipboardList} title="Attendance could not be loaded" description={getErrorMessage(error)} />

  const waiting = data.attendees.filter((a) => a.attended && !a.certificateCode).length
  const issueCertificates = () => issue.mutate(eventId, {
    onSuccess: (r) => toast.success(`${r.issued} certificate${r.issued === 1 ? '' : 's'} issued. Students have been notified.`),
    onError: (e) => toast.error(getErrorMessage(e)),
  })

  return (
    <>
      <PageHeader
        title={`Attendance: ${data.title}`}
        description={`${formatRange(data.startDate, data.endDate)} · ${data.attendees.length} registered · ${data.attendees.filter((a) => a.attended).length} attended`}
        actions={data.hasEnded && (
          <Button loading={issue.isPending} disabled={waiting === 0} onClick={issueCertificates}>
            <Award className="size-4" /> Issue {waiting > 0 ? waiting : ''} certificate{waiting === 1 ? '' : 's'}
          </Button>
        )}
      />
      {!data.hasStarted && (
        <p className="mb-4 flex gap-2 rounded-xl bg-primary-soft p-3 text-sm text-primary">
          <Info className="mt-0.5 size-4 shrink-0" /> You can mark attendance once the event starts. Certificates can be issued after it ends.
        </p>
      )}
      {data.attendees.length === 0 ? (
        <EmptyState icon={ClipboardList} title="No registrations yet" description="Students who register will appear here." />
      ) : (
        <Card className="overflow-hidden">
          <table className="w-full text-sm">
            <thead className="border-b border-line bg-surface-subtle text-left text-xs font-semibold uppercase tracking-wider text-muted">
              <tr>
                <th className="px-5 py-3">Student</th>
                <th className="hidden px-5 py-3 md:table-cell">Team</th>
                <th className="px-5 py-3">Attended</th>
                <th className="hidden px-5 py-3 lg:table-cell">Certificate</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-line">
              {data.attendees.map((a) => (
                <tr key={a.registrationId}>
                  <td className="px-5 py-3">
                    <p className="font-semibold text-heading">{a.studentName}</p>
                    <p className="text-xs text-muted">{a.studentNo} · {a.programme}</p>
                  </td>
                  <td className="hidden px-5 py-3 md:table-cell">{a.teamName ?? <span className="text-muted">—</span>}</td>
                  <td className="px-5 py-3">
                    <input
                      type="checkbox"
                      aria-label={`${a.studentName} attended`}
                      className="size-4 accent-[var(--color-primary)]"
                      checked={a.attended}
                      disabled={!data.hasStarted || !!a.certificateCode || setAttendance.isPending}
                      onChange={() => setAttendance.mutate({ eventId, registrationId: a.registrationId, attended: !a.attended }, { onError: (e) => toast.error(getErrorMessage(e)) })}
                    />
                  </td>
                  <td className="hidden px-5 py-3 lg:table-cell">
                    {a.certificateCode ? <Badge tone="success"><Award className="size-3" /> {a.certificateCode}</Badge> : <span className="text-muted">—</span>}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </Card>
      )}
    </>
  )
}
