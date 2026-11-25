import { useState } from 'react'
import { Building2, Pencil, Plus } from 'lucide-react'
import { PageHeader } from '@/components/layout/PageHeader'
import { Badge } from '@/components/ui/Badge'
import { Button } from '@/components/ui/Button'
import { Card } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { PageSpinner } from '@/components/ui/Spinner'
import { getErrorMessage } from '@/lib/http'
import { useSession } from '@/stores/authStore'
import { useVenues } from '../api/venuesApi'
import { VenueFormDialog } from '../components/VenueFormDialog'
import { VENUE_TYPE_LABEL } from '../config'
import type { Venue } from '../types'

/** The halls, labs and auditoriums the registrar allocates. */
export default function VenuesPage() {
  const canManage = useSession()?.role === 'Registrar'
  const { data: venues, isLoading, isError, error } = useVenues()
  const [editing, setEditing] = useState<{ venue: Venue | null } | null>(null)

  if (isLoading) return <PageSpinner label="Loading venues…" />
  if (isError || !venues) return <EmptyState icon={Building2} title="Venues could not be loaded" description={getErrorMessage(error)} />

  return (
    <>
      <PageHeader
        title="Venues"
        description="Lecture halls, labs, auditoriums and event halls that can be allocated."
        actions={canManage && <Button onClick={() => setEditing({ venue: null })}><Plus className="size-4" /> Add venue</Button>}
      />
      {venues.length === 0 ? <EmptyState icon={Building2} title="No venues yet" /> : (
        <Card className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead className="border-b border-line text-left text-xs uppercase tracking-wider text-muted">
              <tr>
                <th className="px-5 py-3">Venue</th>
                <th className="px-5 py-3">Type</th>
                <th className="px-5 py-3 text-right">Seats</th>
                <th className="px-5 py-3">Facilities</th>
                <th className="px-5 py-3 text-right">Upcoming</th>
                <th className="px-5 py-3">Status</th>
                {canManage && <th className="px-5 py-3"><span className="sr-only">Actions</span></th>}
              </tr>
            </thead>
            <tbody>
              {venues.map((v) => (
                <tr key={v.id} className="border-b border-line last:border-0">
                  <td className="px-5 py-3">
                    <p className="font-semibold text-heading">{v.code} · {v.name}</p>
                    <p className="text-xs text-muted">{v.building}</p>
                  </td>
                  <td className="px-5 py-3">{VENUE_TYPE_LABEL[v.type]}</td>
                  <td className="px-5 py-3 text-right">{v.capacity}</td>
                  <td className="px-5 py-3 text-muted">{v.facilities ?? '—'}</td>
                  <td className="px-5 py-3 text-right">{v.upcomingBookings}</td>
                  <td className="px-5 py-3">{v.isActive ? <Badge tone="success">Open</Badge> : <Badge tone="neutral">Closed</Badge>}</td>
                  {canManage && (
                    <td className="px-5 py-3 text-right">
                      <Button size="sm" variant="ghost" aria-label={`Edit ${v.code}`} onClick={() => setEditing({ venue: v })}><Pencil className="size-4" /></Button>
                    </td>
                  )}
                </tr>
              ))}
            </tbody>
          </table>
        </Card>
      )}
      {editing && <VenueFormDialog venue={editing.venue} onClose={() => setEditing(null)} />}
    </>
  )
}
