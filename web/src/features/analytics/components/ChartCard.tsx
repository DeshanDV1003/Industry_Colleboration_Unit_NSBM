import { useState, type ReactNode } from 'react'
import { BarChart3, Table2 } from 'lucide-react'
import { Button } from '@/components/ui/Button'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'

interface ChartCardProps {
  title: string
  description: string
  /** The same numbers as a table: for screen readers, exact values and colour-independent reading. */
  table: ReactNode
  empty?: string | null
  children: ReactNode
}

/** A chart with a one-click table view of the same data. */
export function ChartCard({ title, description, table, empty, children }: ChartCardProps) {
  const [asTable, setAsTable] = useState(false)

  return (
    // A named region, so assistive technology (and tests) can jump straight to each chart.
    <Card role="region" aria-label={title}>
      <CardHeader title={title} description={description}
        action={!empty && (
          <Button size="sm" variant="ghost" onClick={() => setAsTable((v) => !v)} aria-pressed={asTable}>
            {asTable ? <><BarChart3 className="size-4" /> Chart</> : <><Table2 className="size-4" /> Table</>}
          </Button>
        )} />
      <CardBody className="pt-4">
        {empty ? <p className="py-8 text-center text-sm text-muted">{empty}</p> : asTable ? table : children}
      </CardBody>
    </Card>
  )
}

/** A compact, accessible data table used by every chart's table view. */
export function DataTable({ head, rows }: { head: string[]; rows: (string | number)[][] }) {
  return (
    <div className="overflow-x-auto">
      <table className="w-full text-sm">
        <thead className="border-b border-line text-left text-xs text-muted">
          <tr>{head.map((h, i) => <th key={h} className={i === 0 ? 'py-2 pr-3' : 'px-3 py-2 text-right'}>{h}</th>)}</tr>
        </thead>
        <tbody>
          {rows.map((row, r) => (
            <tr key={r} className="border-b border-line last:border-0">
              {row.map((cell, c) => <td key={c} className={c === 0 ? 'py-2 pr-3 text-heading' : 'px-3 py-2 text-right tabular-nums'}>{cell}</td>)}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
