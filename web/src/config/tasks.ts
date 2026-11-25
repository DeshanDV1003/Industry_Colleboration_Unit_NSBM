import type { BadgeTone } from '@/components/ui/Badge'
import type { ModerationStatus, WorkSituation } from '@/types/tasks'

export const WORK_SITUATIONS: WorkSituation[] = ['BugFix', 'Feature', 'Testing', 'CodeReview', 'Deployment', 'Troubleshooting']

export const WORK_SITUATION_LABEL: Record<WorkSituation, string> = {
  BugFix: 'Bug fix',
  Feature: 'Feature',
  Testing: 'Testing',
  CodeReview: 'Code review',
  Deployment: 'Deployment',
  Troubleshooting: 'Troubleshooting',
}

export const MODERATION_STATUSES: ModerationStatus[] = ['Pending', 'Approved', 'Rejected']

export const MODERATION_STATUS_META: Record<ModerationStatus, { label: string; tone: BadgeTone }> = {
  Pending: { label: 'Pending review', tone: 'warning' },
  Approved: { label: 'Approved', tone: 'success' },
  Rejected: { label: 'Rejected', tone: 'danger' },
}

/** Starter-code languages a partner can choose; the value is shown as a label above the editor. */
export const STARTER_LANGUAGES = ['javascript', 'typescript', 'csharp', 'java', 'python', 'sql', 'yaml', 'json', 'dockerfile', 'text'] as const
