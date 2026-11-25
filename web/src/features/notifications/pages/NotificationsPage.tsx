import { Bell, CheckCheck } from 'lucide-react'
import { PageHeader } from '@/components/layout/PageHeader'
import { Button } from '@/components/ui/Button'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { PageSpinner } from '@/components/ui/Spinner'
import { getErrorMessage } from '@/lib/http'
import { useMarkAllNotificationsRead, useNotifications } from '../api/notificationsApi'
import { NotificationItem } from '../components/NotificationItem'
import { WeeklyDigestCard } from '../components/WeeklyDigestCard'

const PAGE_SIZE = 100

export default function NotificationsPage() {
  const { data, isLoading, isError, error } = useNotifications(PAGE_SIZE)
  const markAll = useMarkAllNotificationsRead()

  if (isLoading) return <PageSpinner label="Loading notifications…" />
  if (isError || !data) return <EmptyState icon={Bell} title="Notifications could not be loaded" description={getErrorMessage(error)} />

  return (
    <>
      <PageHeader
        title="Notifications"
        description="Updates on your applications, feedback, tasks and Unit events, plus your week at a glance."
        actions={data.unreadCount > 0 && (
          <Button variant="secondary" size="sm" loading={markAll.isPending} onClick={() => markAll.mutate()}>
            <CheckCheck className="size-4" /> Mark all read
          </Button>
        )}
      />
      <div className="grid gap-6 xl:grid-cols-[minmax(0,1fr)_400px]">
        <Card>
          <CardHeader title="All notifications" description={`${data.unreadCount} unread`} />
          <CardBody className="space-y-0.5 px-3">
            {data.items.length === 0
              ? <p className="px-3 py-8 text-center text-sm text-muted">Nothing yet. You'll be told here when something needs your attention.</p>
              : data.items.map((n) => <NotificationItem key={n.id} notification={n} />)}
          </CardBody>
        </Card>
        <div className="xl:sticky xl:top-24 xl:self-start"><WeeklyDigestCard /></div>
      </div>
    </>
  )
}
