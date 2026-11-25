import { FormField } from '@/components/ui/FormField'
import { controlClass } from '@/components/ui/controlClass'
import { cn } from '@/lib/cn'
import { APPLICATION_STATUS_LABEL, type ReportDefinition } from '../config'
import type { ReportOptions, ReportParams } from '../types'

interface ReportFiltersProps {
  report: ReportDefinition
  options: ReportOptions
  value: ReportParams
  onChange: (value: ReportParams) => void
}

/** The filters a report declares, rendered from its definition. A cohort sets both programme and academic year. */
export function ReportFilters({ report, options, value, onChange }: ReportFiltersProps) {
  const set = (patch: ReportParams) => onChange({ ...value, ...patch })
  const select = cn(controlClass(false), 'h-10')
  const cohort = value.programme ? `${value.programme}|${value.academicYear}` : ''

  return (
    <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
      {report.filters.includes('dateRange') && (<>
        <FormField label="From" htmlFor="rf-from">
          <input id="rf-from" type="date" className={select} value={value.from} onChange={(e) => set({ from: e.target.value })} />
        </FormField>
        <FormField label="To" htmlFor="rf-to">
          <input id="rf-to" type="date" className={select} value={value.to} onChange={(e) => set({ to: e.target.value })} />
        </FormField>
      </>)}
      {report.filters.includes('venue') && (
        <FormField label="Venue" htmlFor="rf-venue">
          <select id="rf-venue" className={select} value={value.venueId} onChange={(e) => set({ venueId: e.target.value })}>
            <option value="">All venues</option>
            {options.venues.map((v) => <option key={v.id} value={v.id}>{v.name}</option>)}
          </select>
        </FormField>
      )}
      {report.filters.includes('cohort') && (
        <FormField label="Cohort" htmlFor="rf-cohort">
          <select id="rf-cohort" className={select} value={cohort} onChange={(e) => {
            const [programme = '', academicYear = ''] = e.target.value ? e.target.value.split('|') : []
            set({ programme, academicYear })
          }}>
            <option value="">All cohorts</option>
            {options.cohorts.map((c) => (
              <option key={`${c.programme}|${c.academicYear}`} value={`${c.programme}|${c.academicYear}`}>{c.programme} · Year {c.academicYear}</option>
            ))}
          </select>
        </FormField>
      )}
      {report.filters.includes('event') && (
        <FormField label="Event" htmlFor="rf-event">
          <select id="rf-event" className={select} value={value.eventId} onChange={(e) => set({ eventId: e.target.value })}>
            <option value="">All approved events</option>
            {options.events.map((ev) => <option key={ev.id} value={ev.id}>{ev.name}</option>)}
          </select>
        </FormField>
      )}
      {report.filters.includes('applicationStatus') && (
        <FormField label="Status" htmlFor="rf-status">
          <select id="rf-status" className={select} value={value.status} onChange={(e) => set({ status: e.target.value })}>
            <option value="">Every status</option>
            {Object.entries(APPLICATION_STATUS_LABEL).map(([key, label]) => <option key={key} value={key}>{label}</option>)}
          </select>
        </FormField>
      )}
    </div>
  )
}
