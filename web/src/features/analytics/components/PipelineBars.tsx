import { useState } from 'react'
import { PIPELINE_STAGES, SERIES } from '../config'
import type { CompanyPipeline } from '../types'

/**
 * One bar per company, split by application status. Bars share one scale (the busiest company is full width),
 * segments are separated by a 2px surface gap, each status keeps its colour, and a legend names them.
 */
export function PipelineBars({ rows }: { rows: CompanyPipeline[] }) {
  const [hover, setHover] = useState<string | null>(null)
  const max = Math.max(1, ...rows.map((r) => r.total))

  return (
    <div>
      <ul className="mb-4 flex flex-wrap gap-x-4 gap-y-1 text-xs text-body" aria-label="Legend">
        {PIPELINE_STAGES.map((s, i) => (
          <li key={s.key} className="flex items-center gap-1.5"><span className="size-2.5 rounded-sm" style={{ background: SERIES[i] }} />{s.label}</li>
        ))}
      </ul>
      <ul className="space-y-3">
        {rows.map((row) => (
          <li key={row.company}>
            <div className="mb-1 flex justify-between gap-3 text-sm">
              <span className="truncate text-heading">{row.company}</span>
              <span className="shrink-0 tabular-nums text-body">{row.total}</span>
            </div>
            <div className="relative flex h-3 gap-[2px]" style={{ width: `${(100 * row.total) / max}%` }}>
              {PIPELINE_STAGES.map((stage, i) => row[stage.key] > 0 && (
                <div key={stage.key} className="h-full first:rounded-l-[4px] last:rounded-r-[4px]"
                  style={{ flexGrow: row[stage.key], background: SERIES[i] }}
                  onMouseEnter={() => setHover(`${row.company}|${stage.key}`)} onMouseLeave={() => setHover(null)}>
                  {hover === `${row.company}|${stage.key}` && (
                    <div role="tooltip" className="pointer-events-none absolute left-0 top-full z-10 mt-1 whitespace-nowrap rounded-lg border border-line bg-surface px-3 py-1.5 text-xs text-body shadow-pop">
                      {stage.label}: {row[stage.key]} of {row.total}
                    </div>
                  )}
                </div>
              ))}
            </div>
          </li>
        ))}
      </ul>
    </div>
  )
}
