import type { BadgeTone } from '@/components/ui/Badge'

/** Mirrors NSBM.ICU.Domain.Enums.SupportSignal. Signals are prompts to reach out — never judgements. */
export type SupportSignal = 'NoCareerPath' | 'NoPracticeYet' | 'Inactive' | 'LowBaseline' | 'StrugglingWithTask'

export const SUPPORT_SIGNAL_META: Record<SupportSignal, { label: string; tone: BadgeTone; description: string }> = {
  NoCareerPath: { label: 'No career path', tone: 'warning', description: 'Has not chosen a career role yet.' },
  NoPracticeYet: { label: 'No practice yet', tone: 'warning', description: 'No baseline, task, knowledge check or interview practice so far.' },
  Inactive: { label: 'Inactive 14+ days', tone: 'danger', description: 'No activity in the last 14 days.' },
  LowBaseline: { label: 'Low baseline', tone: 'danger', description: 'Latest baseline below 40% — may need a starting point.' },
  StrugglingWithTask: { label: 'Stuck on a task', tone: 'danger', description: 'Two or more attempts with fewer than half the checks passed.' },
}
