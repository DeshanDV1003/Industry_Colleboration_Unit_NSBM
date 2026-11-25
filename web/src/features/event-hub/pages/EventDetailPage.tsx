import { useState } from 'react'
import { useParams } from 'react-router-dom'
import { Award, Building2, CalendarDays, CalendarHeart, MapPin, Users } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { PageHeader } from '@/components/layout/PageHeader'
import { Badge } from '@/components/ui/Badge'
import { Button } from '@/components/ui/Button'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { LinkButton } from '@/components/ui/LinkButton'
import { PageSpinner } from '@/components/ui/Spinner'
import { EVENT_FORMAT_META, teamSizeLabel } from '@/config/events'
import { formatRange } from '@/features/calendar'
import { formatDate } from '@/lib/format'
import { getErrorMessage } from '@/lib/http'
import { useCancelRegistration, useEventDetail, useRegisterForEvent } from '../api/eventHubApi'
import { TeamPanel } from '../components/TeamPanel'
import type { EventDetail } from '../types'

const onError = (e: unknown) => toast.error(getErrorMessage(e))

function RegistrationCard({ detail }: { detail: EventDetail }) {
  const { event, myCertificateCode } = detail
  const register = useRegisterForEvent()
  const cancel = useCancelRegistration()
  const [confirming, setConfirming] = useState(false)

  return (
    <Card>
      <CardHeader title="Your place" description={`${event.registeredCount}/${event.capacity} places taken · registration closes ${formatDate(event.registrationClosesOn)}`} />
      <CardBody className="space-y-3 text-sm">
        {myCertificateCode ? (
          <>
            <p className="flex items-center gap-2 text-success"><Award className="size-4" /> You took part and have a certificate.</p>
            <LinkButton to="/certificates">View certificate</LinkButton>
          </>
        ) : event.isRegistered ? (
          <>
            <Badge tone="success">You are registered</Badge>
            {event.registrationOpen && (confirming
              ? <div className="flex gap-2">
                  <Button variant="danger" size="sm" loading={cancel.isPending} onClick={() => cancel.mutate(event.id, { onSuccess: () => setConfirming(false), onError })}>Yes, give up my place</Button>
                  <Button variant="ghost" size="sm" onClick={() => setConfirming(false)}>Keep it</Button>
                </div>
              : <Button variant="secondary" size="sm" onClick={() => setConfirming(true)}>Cancel registration</Button>)}
          </>
        ) : event.isOpen ? (
          <Button loading={register.isPending} onClick={() => register.mutate(event.id, { onSuccess: () => toast.success('You are registered. It is on your calendar.'), onError })}>
            Register
          </Button>
        ) : (
          <p className="text-muted">{event.hasEnded ? 'This event has ended.' : event.registrationOpen ? 'This event is full.' : 'Registration has closed.'}</p>
        )}
      </CardBody>
    </Card>
  )
}

export default function EventDetailPage() {
  const id = Number(useParams().id)
  const { data: detail, isLoading, isError, error } = useEventDetail(id)

  if (isLoading) return <PageSpinner label="Loading event…" />
  if (isError || !detail) return <EmptyState icon={CalendarHeart} title="Event could not be loaded" description={getErrorMessage(error)} action={<LinkButton to="/events" variant="secondary">All events</LinkButton>} />

  const { event } = detail
  const meta = EVENT_FORMAT_META[event.format]
  const isTeamEvent = event.maxTeamSize !== null

  return (
    <>
      <PageHeader
        title={event.title}
        description={<span className="flex flex-wrap items-center gap-1.5">
          <Badge tone={meta.tone}><meta.icon className="size-3" /> {meta.label}</Badge>
          <Badge>{teamSizeLabel(event.minTeamSize, event.maxTeamSize)}</Badge>
        </span>}
        actions={<LinkButton to="/events" variant="secondary">All events</LinkButton>}
      />
      <div className="grid gap-6 xl:grid-cols-[minmax(0,1fr)_380px]">
        <Card>
          <CardBody className="space-y-4 text-sm">
            <dl className="grid gap-2 sm:grid-cols-2">
              <div className="flex items-center gap-2"><Building2 className="size-4 text-muted" /> {event.companyName}</div>
              <div className="flex items-center gap-2"><CalendarDays className="size-4 text-muted" /> {formatRange(event.startDate, event.endDate)}</div>
              <div className="flex items-center gap-2"><MapPin className="size-4 text-muted" /> {event.location}</div>
              <div className="flex items-center gap-2"><Users className="size-4 text-muted" /> {event.capacity} places</div>
            </dl>
            <p className="whitespace-pre-line text-body">{detail.description}</p>
          </CardBody>
        </Card>
        <div className="space-y-6">
          <RegistrationCard detail={detail} />
          {isTeamEvent && event.isRegistered && <TeamPanel detail={detail} />}
        </div>
      </div>
    </>
  )
}
