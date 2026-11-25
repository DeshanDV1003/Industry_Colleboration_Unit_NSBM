import type { BadgeTone } from '@/components/ui/Badge'
import type { ApplicationStatus, SkillEvidence, WorkMode } from '@/types/opportunities'

export const WORK_MODES: WorkMode[] = ['OnSite', 'Hybrid', 'Remote']

export const WORK_MODE_LABEL: Record<WorkMode, string> = {
  OnSite: 'On-site',
  Hybrid: 'Hybrid',
  Remote: 'Remote',
}

/** Statuses a company can move an application to, in pipeline order. */
export const COMPANY_APPLICATION_STATUSES: ApplicationStatus[] = ['UnderReview', 'Shortlisted', 'InterviewInvited', 'NotSelected']

export const APPLICATION_STATUS_META: Record<ApplicationStatus, { label: string; tone: BadgeTone }> = {
  Submitted: { label: 'Submitted', tone: 'neutral' },
  UnderReview: { label: 'Under review', tone: 'primary' },
  Shortlisted: { label: 'Shortlisted', tone: 'accent' },
  InterviewInvited: { label: 'Interview invited', tone: 'success' },
  NotSelected: { label: 'Not selected', tone: 'danger' },
}

export const SKILL_EVIDENCE_META: Record<SkillEvidence, { label: string; tone: BadgeTone; description: string }> = {
  PracticeSignal: { label: 'Practice signal', tone: 'success', description: 'A correct baseline answer or task activity suggests this skill is developing.' },
  ListedOnly: { label: 'Listed only', tone: 'warning', description: 'In the profile, but there is no practice signal yet.' },
  Missing: { label: 'Missing', tone: 'danger', description: 'Not listed and not seen in practice yet.' },
}

export const SKILL_EVIDENCE_ORDER: SkillEvidence[] = ['PracticeSignal', 'ListedOnly', 'Missing']

/** Shown wherever a match percentage appears (feature guide: responsible-use rules). */
export const MATCH_DISCLAIMER = 'Match counts overlapping required skills only. It does not measure CV quality, promise an interview or predict hiring.'
