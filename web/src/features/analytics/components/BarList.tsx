import { useState } from 'react'
import { SINGLE } from '../config'

export interface BarRow {
  label: string
  /** 0–100, or null when withheld for privacy. */
  percent: number | null
  /** Shown on hover and beside withheld rows, e.g. "3 of 4 attended". */
  detail: string
}

/** Horizontal bars for a percentage per item: one hue, value at the tip, details on hover. */
export function BarList({ rows, withheld }: { rows: BarRow[]; withheld: string }) {
  const [hover, setHover] = useState<string | null>(null)

  return (
    <ul className="space-y-3">
      {rows.map((row) => (
        <li key={row.label} onMouseEnter={() => setHover(row.label)} onMouseLeave={() => setHover(null)} className="relative">
          <div className="mb-1 flex justify-between gap-3 text-sm">
            <span className="truncate text-heading">{row.label}</span>
            <span className="shrink-0 tabular-nums text-body">{row.percent === null ? '—' : `${row.percent}%`}</span>
          </div>
          {row.percent === null ? (
            <p className="text-xs text-muted">{withheld} ({row.detail})</p>
          ) : (
            <div className="h-2.5 rounded-full bg-[var(--viz-grid)]">
              <div className="h-full rounded-full" style={{ width: `${Math.max(row.percent, row.percent > 0 ? 2 : 0)}%`, background: SINGLE }} />
            </div>
          )}
          {hover === row.label && row.percent !== null && (
            <div role="tooltip" className="pointer-events-none absolute right-0 top-full z-10 mt-1 rounded-lg border border-line bg-surface px-3 py-1.5 text-xs text-body shadow-pop">
              {row.detail}
            </div>
          )}
        </li>
      ))}
    </ul>
  )
}
