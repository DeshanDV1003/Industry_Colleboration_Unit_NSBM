import { Megaphone } from 'lucide-react'
import { PageHeader } from '@/components/layout/PageHeader'
import { Badge } from '@/components/ui/Badge'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { Spinner } from '@/components/ui/Spinner'
import { formatDateTime } from '@/lib/format'
import { getErrorMessage } from '@/lib/http'
import { useAnnouncements } from '../api/announcementsApi'
import { AnnouncementComposer } from '../components/AnnouncementComposer'
import { AUDIENCE_META } from '../config'

/** The Unit admin notifies everyone, or one group, and sees what was already sent. */
export default function AnnouncementsPage() {
  const { data: sent, isLoading, isError, error } = useAnnouncements()

  return (
    <>
      <PageHeader title="Announcements" description="Notify the whole platform, or one group, in a single step." />
      <div className="grid items-start gap-6 lg:grid-cols-2">
        <AnnouncementComposer />
        <Card>
          <CardHeader title="Sent" description="Newest first." />
          <CardBody className="space-y-3">
            {isLoading ? <Spinner />
              : isError ? <p className="text-sm text-danger">{getErrorMessage(error)}</p>
              : !sent?.length ? <EmptyState icon={Megaphone} title="Nothing sent yet" />
              : sent.map((a) => (
                <div key={a.id} className="rounded-xl border border-line p-3 text-sm">
                  <div className="flex flex-wrap items-start justify-between gap-2">
                    <p className="font-semibold text-heading">{a.title}</p>
                    <Badge tone="primary">{AUDIENCE_META[a.audience].label}</Badge>
                  </div>
                  <p className="mt-1 whitespace-pre-line text-body">{a.message}</p>
                  <p className="mt-2 text-xs text-muted">{a.sentByName} · {formatDateTime(a.sentAt)} · {a.recipientCount} recipient{a.recipientCount === 1 ? '' : 's'}</p>
                </div>
              ))}
          </CardBody>
        </Card>
      </div>
    </>
  )
}
