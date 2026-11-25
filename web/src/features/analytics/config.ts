import type { CompanyPipeline } from './types'

/** Categorical slots in fixed order (never cycled). Each maps to a validated CSS token with light and dark steps. */
export const SERIES = ['var(--viz-1)', 'var(--viz-2)', 'var(--viz-3)', 'var(--viz-4)', 'var(--viz-5)'] as const
export const SINGLE = 'var(--viz-single)'

/** Application statuses in pipeline order; each keeps its slot whatever is filtered. */
export const PIPELINE_STAGES: { key: keyof Omit<CompanyPipeline, 'company' | 'total'>; label: string }[] = [
  { key: 'submitted', label: 'Submitted' },
  { key: 'underReview', label: 'Under review' },
  { key: 'shortlisted', label: 'Shortlisted' },
  { key: 'interviewInvited', label: 'Interview invited' },
  { key: 'notSelected', label: 'Not selected' },
]

export const PERIODS = [4, 12, 26, 52] as const

const dayFormat = new Intl.DateTimeFormat('en-GB', { day: 'numeric', month: 'short' })
const parse = (iso: string) => { const [y, m, d] = iso.split('-').map(Number); return new Date(y, m - 1, d) }
export const formatDay = (iso: string) => dayFormat.format(parse(iso))
export const percent = (part: number, whole: number) => (whole === 0 ? 0 : Math.round((100 * part) / whole))
