import { useState } from 'react'
import { formatDay } from '../config'
import { useElementWidth } from './useElementWidth'

export interface LineSeries { name: string; values: number[]; color: string }

interface LineChartProps {
  /** ISO dates of each point (week starts). */
  x: string[]
  series: LineSeries[]
  height?: number
  /** Unit words for a single-series tooltip: [one, many], e.g. ["student", "students"]. */
  unit?: [string, string]
}

const PAD = { top: 12, right: 16, bottom: 26, left: 34 }

/** Rounds up to 1, 2 or 5 × 10ⁿ so ticks are clean numbers. */
function niceMax(value: number) {
  if (value <= 4) return 4
  const power = 10 ** Math.floor(Math.log10(value))
  return ([1, 2, 5, 10].map((s) => s * power).find((s) => s >= value) ?? 10 * power)
}

/**
 * Change over time on one axis. 2px lines, recessive grid, a crosshair with every series' value on hover,
 * a legend and end labels when there is more than one series.
 */
export function LineChart({ x, series, height = 220, unit = ['', ''] }: LineChartProps) {
  const { ref, width } = useElementWidth<HTMLDivElement>()
  const [hover, setHover] = useState<number | null>(null)

  const max = niceMax(Math.max(1, ...series.flatMap((s) => s.values)))
  const ticks = [0, max / 4, max / 2, (3 * max) / 4, max]
  const plotW = Math.max(0, width - PAD.left - PAD.right)
  const plotH = height - PAD.top - PAD.bottom
  const xAt = (i: number) => PAD.left + (x.length <= 1 ? plotW / 2 : (i * plotW) / (x.length - 1))
  const yAt = (v: number) => PAD.top + plotH - (v / max) * plotH
  const labelEvery = Math.max(1, Math.ceil(x.length / Math.max(1, Math.floor(plotW / 64))))
  const multi = series.length > 1

  const onMove = (e: React.MouseEvent<SVGRectElement>) => {
    const box = e.currentTarget.getBoundingClientRect()
    const i = Math.round(((e.clientX - box.left) / box.width) * (x.length - 1))
    setHover(Math.min(x.length - 1, Math.max(0, i)))
  }

  return (
    <div ref={ref} className="relative">
      {multi && (
        <ul className="mb-2 flex flex-wrap gap-4 text-xs text-body" aria-label="Legend">
          {series.map((s) => (
            <li key={s.name} className="flex items-center gap-1.5"><span className="h-0.5 w-4 rounded" style={{ background: s.color }} />{s.name}</li>
          ))}
        </ul>
      )}
      {width > 0 && (
        <svg width={width} height={height} role="img" aria-label={`${series.map((s) => s.name).join(' and ')} per week`}>
          {ticks.map((t) => (
            <g key={t}>
              <line x1={PAD.left} x2={width - PAD.right} y1={yAt(t)} y2={yAt(t)} stroke="var(--viz-grid)" strokeWidth={1} />
              <text x={PAD.left - 6} y={yAt(t)} dy="0.32em" textAnchor="end" className="fill-[var(--color-text-muted)] text-[11px]">{t}</text>
            </g>
          ))}
          {x.map((d, i) => i % labelEvery === 0 && (
            <text key={d} x={xAt(i)} y={height - 6} textAnchor="middle" className="fill-[var(--color-text-muted)] text-[11px]">{formatDay(d)}</text>
          ))}
          {series.map((s) => (
            <polyline key={s.name} fill="none" stroke={s.color} strokeWidth={2} strokeLinejoin="round" strokeLinecap="round"
              points={s.values.map((v, i) => `${xAt(i)},${yAt(v)}`).join(' ')} />
          ))}
          {hover !== null && (
            <g>
              <line x1={xAt(hover)} x2={xAt(hover)} y1={PAD.top} y2={PAD.top + plotH} stroke="var(--color-text-muted)" strokeDasharray="3 3" strokeWidth={1} />
              {series.map((s) => (
                <circle key={s.name} cx={xAt(hover)} cy={yAt(s.values[hover])} r={4.5} fill={s.color} stroke="var(--color-bg-card)" strokeWidth={2} />
              ))}
            </g>
          )}
          <rect x={PAD.left} y={PAD.top} width={plotW} height={plotH} fill="transparent" onMouseMove={onMove} onMouseLeave={() => setHover(null)} />
        </svg>
      )}
      {hover !== null && (
        <div role="tooltip" className="pointer-events-none absolute top-8 z-10 rounded-lg border border-line bg-surface px-3 py-2 text-xs shadow-pop"
          style={{ left: Math.min(Math.max(xAt(hover) - 70, 0), Math.max(0, width - 150)) }}>
          <p className="font-semibold text-heading">Week of {formatDay(x[hover])}</p>
          {series.map((s) => (
            <p key={s.name} className="mt-0.5 flex items-center gap-1.5 text-body">
              <span className="size-2 rounded-full" style={{ background: s.color }} />
              {multi ? `${s.name}: ${s.values[hover]}` : `${s.values[hover]} ${s.values[hover] === 1 ? unit[0] : unit[1]}`}
            </p>
          ))}
        </div>
      )}
    </div>
  )
}
