import { useNavigate, useParams } from 'react-router-dom'
import type { UseFormSetError } from 'react-hook-form'
import { CalendarHeart, Lock } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { PageHeader } from '@/components/layout/PageHeader'
import { EmptyState } from '@/components/ui/EmptyState'
import { LinkButton } from '@/components/ui/LinkButton'
import { PageSpinner } from '@/components/ui/Spinner'
import { applyServerErrors } from '@/lib/formErrors'
import { getErrorMessage } from '@/lib/http'
import { useCreateIndustryEvent, useIndustryEvent, useUpdateIndustryEvent } from '../api/industryEventsApi'
import { EventEditorForm } from '../components/EventEditorForm'
import { emptyEventForm, toEventFormValues, toEventInput, type EventFormValues } from '../eventForm'

const LIST_PATH = '/industry-events'

/** Proposes a new event (/industry-events/new) or edits a pending or rejected one (/industry-events/:id/edit). */
export default function EventEditorPage() {
  const { id } = useParams()
  const eventId = id ? Number(id) : null
  const navigate = useNavigate()
  const { data: event, isLoading, isError, error } = useIndustryEvent(eventId)
  const create = useCreateIndustryEvent()
  const update = useUpdateIndustryEvent()

  if (eventId !== null && isLoading) return <PageSpinner label="Loading event…" />
  if (eventId !== null && (isError || !event)) return <EmptyState icon={CalendarHeart} title="Event could not be loaded" description={getErrorMessage(error)} />
  if (event && !event.canBeEdited) {
    return (
      <EmptyState icon={Lock} title="This event is live" description="Approved events are open to students and can no longer be edited."
        action={<LinkButton to={LIST_PATH} variant="secondary">Back to events</LinkButton>} />
    )
  }

  const handleSubmit = (values: EventFormValues, setError: UseFormSetError<EventFormValues>) => {
    const input = toEventInput(values)
    const onError = (e: unknown) => { if (!applyServerErrors(e, setError, 'Event.')) toast.error(getErrorMessage(e)) }
    const onSuccess = () => { toast.success('Submitted for review. Students can register once the Unit approves it.'); navigate(LIST_PATH) }
    if (eventId === null) create.mutate(input, { onSuccess, onError })
    else update.mutate({ id: eventId, input }, { onSuccess, onError })
  }

  return (
    <>
      <PageHeader
        title={event ? 'Edit event' : 'Host an event'}
        description="Events stay Pending until the Industry Collaboration Unit reviews them."
      />
      <EventEditorForm
        key={event?.id ?? 'new'}
        defaultValues={event ? toEventFormValues(event) : emptyEventForm}
        submitLabel={event ? 'Save and resubmit' : 'Submit for review'}
        submitting={create.isPending || update.isPending}
        onSubmit={handleSubmit}
        onCancel={() => navigate(LIST_PATH)}
      />
    </>
  )
}
