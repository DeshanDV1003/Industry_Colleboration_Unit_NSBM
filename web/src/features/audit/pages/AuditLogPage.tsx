import { useDeferredValue, useState } from 'react'
import { ChevronLeft, ChevronRight, Download, ScrollText, Search } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { PageHeader } from '@/components/layout/PageHeader'
import { Badge } from '@/components/ui/Badge'
import { Button } from '@/components/ui/Button'
import { Card } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { FormField } from '@/components/ui/FormField'
import { PageSpinner } from '@/components/ui/Spinner'
import { controlClass } from '@/components/ui/controlClass'
import { ROLE_META } from '@/config/roles'
import { cn } from '@/lib/cn'
import { formatDateTime } from '@/lib/format'
import { getErrorMessage } from '@/lib/http'
import { useAuditLog, useExportAudit } from '../api/auditApi'
import { CATEGORY_META } from '../config'
import { AUDIT_CATEGORIES, type AuditFilter } from '../types'

const NO_FILTER: AuditFilter = { category: '', search: '', from: '', to: '' }

/** Who did what and when, for the Unit head. Read-only; export the filtered entries to Excel. */
export default function AuditLogPage() {
  const [filter, setFilter] = useState<AuditFilter>(NO_FILTER)
  const [page, setPage] = useState(1)
  // Typing in search should not send a request per keystroke.
  const query = useDeferredValue(filter)
  const { data, isLoading, isError, error, isFetching } = useAuditLog(query, page)
  const exportAudit = useExportAudit()

  const set = (patch: Partial<AuditFilter>) => { setFilter((f) => ({ ...f, ...patch })); setPage(1) }
  const control = cn(controlClass(false), 'h-10')
  const pages = data ? Math.max(1, Math.ceil(data.total / data.pageSize)) : 1

  return (
    <>
      <PageHeader
        title="Audit log"
        description="A permanent record of approvals, account changes, hall allocations, announcements and other actions. Entries cannot be edited or deleted."
        actions={<Button variant="secondary" loading={exportAudit.isPending}
          onClick={() => exportAudit.mutate(filter, { onError: (e) => toast.error(getErrorMessage(e)) })}>
          <Download className="size-4" /> Export to Excel
        </Button>}
      />

      <Card className="mb-4 grid gap-4 p-5 sm:grid-cols-2 xl:grid-cols-4">
        <FormField label="Area" htmlFor="au-category">
          <select id="au-category" className={control} value={filter.category} onChange={(e) => set({ category: e.target.value as AuditFilter['category'] })}>
            <option value="">All areas</option>
            {AUDIT_CATEGORIES.map((c) => <option key={c} value={c}>{CATEGORY_META[c].label}</option>)}
          </select>
        </FormField>
        <FormField label="Search" htmlFor="au-search">
          <div className="relative">
            <Search className="pointer-events-none absolute left-3 top-1/2 size-4 -translate-y-1/2 text-muted" />
            <input id="au-search" placeholder="Name, e-mail or words" className={cn(control, 'pl-9')} value={filter.search} onChange={(e) => set({ search: e.target.value })} />
          </div>
        </FormField>
        <FormField label="From" htmlFor="au-from">
          <input id="au-from" type="date" className={control} value={filter.from} onChange={(e) => set({ from: e.target.value })} />
        </FormField>
        <FormField label="To" htmlFor="au-to">
          <input id="au-to" type="date" className={control} value={filter.to} onChange={(e) => set({ to: e.target.value })} />
        </FormField>
      </Card>

      {isLoading ? <PageSpinner label="Loading the audit log…" />
        : isError || !data ? <EmptyState icon={ScrollText} title="The audit log could not be loaded" description={getErrorMessage(error)} />
        : data.items.length === 0 ? <EmptyState icon={ScrollText} title="No entries match these filters" />
        : (
          <Card className={cn('overflow-x-auto transition-opacity', isFetching && 'opacity-70')}>
            <table className="w-full text-sm">
              <thead className="border-b border-line text-left text-xs uppercase tracking-wider text-muted">
                <tr>
                  <th className="px-5 py-3">When</th>
                  <th className="px-5 py-3">Who</th>
                  <th className="px-5 py-3">Area</th>
                  <th className="px-5 py-3">What happened</th>
                </tr>
              </thead>
              <tbody>
                {data.items.map((e) => (
                  <tr key={e.id} className="border-b border-line align-top last:border-0">
                    <td className="whitespace-nowrap px-5 py-3 text-muted">{formatDateTime(e.at)}</td>
                    <td className="px-5 py-3">
                      <p className="font-medium text-heading">{e.actorName}</p>
                      {e.actorRole && <p className="text-xs text-muted">{ROLE_META[e.actorRole]?.label ?? e.actorRole}</p>}
                    </td>
                    <td className="px-5 py-3"><Badge tone={CATEGORY_META[e.category].tone}>{CATEGORY_META[e.category].label}</Badge></td>
                    <td className="px-5 py-3">
                      <p className="text-body">{e.summary}</p>
                      <p className="mt-0.5 font-mono text-[11px] text-muted">{e.action}</p>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
            <div className="flex items-center justify-between gap-2 border-t border-line px-5 py-3 text-xs text-muted">
              <span>{data.total} entr{data.total === 1 ? 'y' : 'ies'} · page {data.page} of {pages}</span>
              <span className="flex gap-1">
                <Button size="sm" variant="secondary" aria-label="Newer entries" disabled={page <= 1} onClick={() => setPage((p) => p - 1)}><ChevronLeft className="size-4" /></Button>
                <Button size="sm" variant="secondary" aria-label="Older entries" disabled={page >= pages} onClick={() => setPage((p) => p + 1)}><ChevronRight className="size-4" /></Button>
              </span>
            </div>
          </Card>
        )}
    </>
  )
}
