import { FileSpreadsheet } from 'lucide-react'
import { EmptyState } from '@/components/ui/EmptyState'
import { cn } from '@/lib/cn'
import type { ReportColumnKind, ReportTable } from '../types'

const dateFormat = new Intl.DateTimeFormat('en-GB', { day: 'numeric', month: 'short', year: 'numeric' })
const parseIsoDay = (iso: string) => { const [y, m, d] = iso.split('-').map(Number); return new Date(y, m - 1, d) }

/** Shows a value the way the Excel file formats it. */
function format(value: string | number | boolean | null, kind: ReportColumnKind): string {
  if (value === null || value === '') return '—'
  if (typeof value === 'boolean') return value ? 'Yes' : 'No'
  if (kind === 'Date' && typeof value === 'string') return dateFormat.format(parseIsoDay(value))
  if (kind === 'Time' && typeof value === 'string') return value.slice(0, 5)
  if (kind === 'Percent') return `${value}%`
  return String(value)
}

const numeric = (kind: ReportColumnKind) => kind === 'Number' || kind === 'Percent'

/** The report as a table on screen and on paper (the app's sidebar and header are hidden when printing). */
export function ReportPreview({ table }: { table: ReportTable }) {
  return (
    <section aria-label={table.title} className="rounded-card border border-line bg-surface print:border-0">
      <div className="border-b border-line px-5 py-4 print:px-0">
        <h2 className="text-lg font-semibold">{table.title}</h2>
        <p className="text-sm text-muted">{table.subtitle}</p>
        <p className="hidden text-xs text-muted print:block">NSBM Green University · Industry Collaboration Unit · printed {dateFormat.format(new Date())}</p>
      </div>
      {table.rows.length === 0 ? (
        <EmptyState icon={FileSpreadsheet} title="Nothing to report for these filters" className="border-0" />
      ) : (
        <div className="overflow-x-auto print:overflow-visible">
          <table className="w-full text-sm print:text-[10px]">
            <thead className="bg-primary text-left text-xs text-white print:bg-transparent print:text-black">
              <tr>
                {table.columns.map((c) => (
                  <th key={c.header} className={cn('whitespace-nowrap px-3 py-2 font-semibold', numeric(c.kind) && 'text-right')}>{c.header}</th>
                ))}
              </tr>
            </thead>
            <tbody>
              {table.rows.map((row, r) => (
                <tr key={r} className="border-b border-line last:border-0 print:break-inside-avoid">
                  {row.map((value, c) => (
                    <td key={c} className={cn('px-3 py-2 align-top', table.columns[c].kind !== 'Text' && 'whitespace-nowrap',
                      numeric(table.columns[c].kind) && 'text-right tabular-nums')}>
                      {format(value, table.columns[c].kind)}
                    </td>
                  ))}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
      <p className="border-t border-line px-5 py-3 text-xs text-muted print:hidden">{table.rows.length} row{table.rows.length === 1 ? '' : 's'}</p>
    </section>
  )
}
