import { useState } from 'react'
import { Check, Inbox, Mail, Phone } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { PageHeader } from '@/components/layout/PageHeader'
import { Badge } from '@/components/ui/Badge'
import { Button } from '@/components/ui/Button'
import { Card, CardBody } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { SegmentedControl } from '@/components/ui/SegmentedControl'
import { PageSpinner } from '@/components/ui/Spinner'
import { formatDateTime } from '@/lib/format'
import { getErrorMessage } from '@/lib/http'
import { useMarkEnquiryHandled, usePartnerEnquiries } from '../api/showcaseApi'
import { PARTNER_INTEREST_LABEL } from '../config'

type Filter = 'open' | 'handled'

/** The Unit's inbox for "become a partner" enquiries from the public website. */
export default function PartnerEnquiriesPage() {
  const { data, isLoading, isError, error } = usePartnerEnquiries()
  const markHandled = useMarkEnquiryHandled()
  const [filter, setFilter] = useState<Filter>('open')

  if (isLoading) return <PageSpinner label="Loading enquiries…" />
  if (isError || !data) return <EmptyState icon={Inbox} title="Enquiries could not be loaded" description={getErrorMessage(error)} />

  const open = data.filter((e) => !e.isHandled)
  const visible = filter === 'open' ? open : data.filter((e) => e.isHandled)

  return (
    <>
      <PageHeader title="Partner enquiries" description="Companies that asked to partner with the Unit through the public website. Reply by email, then mark them handled." />
      <div className="mb-4">
        <SegmentedControl ariaLabel="Filter enquiries" value={filter} onChange={setFilter}
          options={[{ value: 'open', label: 'Open', count: open.length }, { value: 'handled', label: 'Handled', count: data.length - open.length }]} />
      </div>
      {visible.length === 0 ? <EmptyState icon={Inbox} title={filter === 'open' ? 'No open enquiries' : 'Nothing handled yet'} /> : (
        <div className="grid gap-4 lg:grid-cols-2">
          {visible.map((e) => (
            <Card key={e.id}>
              <CardBody className="space-y-3 text-sm">
                <div className="flex flex-wrap items-start justify-between gap-2">
                  <div>
                    <p className="text-base font-semibold text-heading">{e.organisationName}</p>
                    <p className="text-xs text-muted">{e.contactName} · {formatDateTime(e.submittedAt)}</p>
                  </div>
                  <Badge tone="primary">{PARTNER_INTEREST_LABEL[e.interest]}</Badge>
                </div>
                <p className="whitespace-pre-line text-body">{e.message}</p>
                <div className="flex flex-wrap items-center gap-3 text-xs">
                  <a href={`mailto:${e.email}`} className="flex items-center gap-1"><Mail className="size-3.5" /> {e.email}</a>
                  {e.phone && <span className="flex items-center gap-1 text-muted"><Phone className="size-3.5" /> {e.phone}</span>}
                  <span className="ml-auto">
                    {e.isHandled
                      ? <span className="text-muted">Handled by {e.handledBy}</span>
                      : <Button size="sm" variant="secondary" loading={markHandled.isPending && markHandled.variables === e.id}
                          onClick={() => markHandled.mutate(e.id, { onError: (err) => toast.error(getErrorMessage(err)) })}><Check className="size-4" /> Mark handled</Button>}
                  </span>
                </div>
              </CardBody>
            </Card>
          ))}
        </div>
      )}
    </>
  )
}
