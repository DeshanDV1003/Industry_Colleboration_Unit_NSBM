import { useState } from 'react'
import { Download, FileBarChart, Play, Printer } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { PageHeader } from '@/components/layout/PageHeader'
import { Button } from '@/components/ui/Button'
import { Card, CardBody } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { PageSpinner, Spinner } from '@/components/ui/Spinner'
import { cn } from '@/lib/cn'
import { getErrorMessage } from '@/lib/http'
import { useSession } from '@/stores/authStore'
import { useDownloadExcel, useReportOptions, useReportTable } from '../api/reportsApi'
import { ReportFilters } from '../components/ReportFilters'
import { ReportPreview } from '../components/ReportPreview'
import { reportsFor, type ReportDefinition } from '../config'
import type { ReportParams } from '../types'

/** Pick a report, set its filters, preview it, then download it as Excel or print it (save as PDF from the print dialog). */
export default function ReportsPage() {
  const role = useSession()?.role
  const available = role ? reportsFor(role) : []
  const options = useReportOptions()
  const [report, setReport] = useState<ReportDefinition | undefined>(available[0])
  const [filters, setFilters] = useState<ReportParams>(() => available[0]?.defaults() ?? {})
  const [shown, setShown] = useState<ReportParams | null>(null)
  const table = useReportTable(report?.endpoint ?? '', shown)
  const excel = useDownloadExcel()

  if (!report) return <EmptyState icon={FileBarChart} title="No reports for this portal" />
  if (options.isLoading) return <PageSpinner label="Loading reports…" />
  if (!options.data) return <EmptyState icon={FileBarChart} title="Reports could not be loaded" description={getErrorMessage(options.error)} />

  const choose = (next: ReportDefinition) => {
    setReport(next)
    setFilters(next.defaults())
    setShown(null)
  }

  const downloadExcel = () => excel.mutate({ endpoint: report.endpoint, params: filters }, {
    onError: (e) => toast.error(getErrorMessage(e)),
  })

  const previewIsCurrent = shown !== null && JSON.stringify(shown) === JSON.stringify(filters) && table.data

  return (
    <>
      <div className="print:hidden">
        <PageHeader title="Reports" description="Preview a report, then download it as Excel or print it. To make a PDF, print and choose Save as PDF." />

        <div className="mb-6 grid gap-3 sm:grid-cols-2 xl:grid-cols-4">
          {available.map((r) => (
            <button key={r.key} type="button" onClick={() => choose(r)} aria-pressed={r.key === report.key}
              className={cn('flex items-start gap-3 rounded-card border p-4 text-left transition-colors',
                r.key === report.key ? 'border-primary bg-primary-soft' : 'border-line bg-surface hover:border-line-strong')}>
              <span className="grid size-9 shrink-0 place-items-center rounded-lg bg-primary-soft text-primary"><r.icon className="size-5" /></span>
              <span>
                <span className="block font-semibold text-heading">{r.title}</span>
                <span className="mt-0.5 block text-xs text-muted">{r.description}</span>
              </span>
            </button>
          ))}
        </div>

        <Card className="mb-6">
          <CardBody className="space-y-4 pt-5">
            <ReportFilters report={report} options={options.data} value={filters} onChange={setFilters} />
            <div className="flex flex-wrap justify-end gap-2">
              <Button variant="secondary" onClick={() => setShown(filters)}><Play className="size-4" /> Show report</Button>
              <Button variant="secondary" loading={excel.isPending} onClick={downloadExcel}><Download className="size-4" /> Download Excel</Button>
              <Button disabled={!previewIsCurrent} title={previewIsCurrent ? undefined : 'Show the report first'} onClick={() => window.print()}>
                <Printer className="size-4" /> Print
              </Button>
            </div>
          </CardBody>
        </Card>
      </div>

      {shown !== null && (table.isFetching && !table.data ? <Spinner />
        : table.isError ? <EmptyState icon={FileBarChart} title="The report could not be built" description={getErrorMessage(table.error)} />
        : table.data && <ReportPreview table={table.data} />)}
    </>
  )
}
