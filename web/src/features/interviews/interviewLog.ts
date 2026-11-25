import { formatDateTime } from '@/lib/format'
import type { InterviewReport } from './types'

const STAGE_TITLE = { SoftSkills: 'Stage 1 — Soft skills', Technical: 'Stage 2 — Technical' } as const

/** Builds a plain-text practice log the student can keep or share with a lecturer. */
export function buildInterviewLog(report: InterviewReport): string {
  const lines = [
    'NSBM Green University · Industry Collaboration Unit',
    'Interview practice log',
    `${report.studentName} (${report.studentNo}) — generated ${formatDateTime(report.generatedAt)}`,
    'Feedback uses simple topic checks. It is practice evidence, not a grade.',
    '',
  ]

  let lastStage = ''
  for (const item of report.items) {
    const stage = `${STAGE_TITLE[item.stage]}${item.careerPathName ? ` (${item.careerPathName})` : ''}`
    if (stage !== lastStage) {
      lines.push('='.repeat(60), stage, '='.repeat(60), '')
      lastStage = stage
    }
    lines.push(
      `${item.isFollowUp ? 'Follow-up' : 'Question'} [${item.topic}]: ${item.prompt}`,
      `Answer (${item.answer.inputMode.toLowerCase()}, ${formatDateTime(item.answer.answeredAt)}):`,
      item.answer.text,
      `Topic coverage: ${item.answer.coveragePercent}% · Mentioned: ${item.answer.matchedTopics.join(', ') || '—'}`,
      `Could add: ${item.answer.topicsToConsider.join(', ') || '—'}`,
      ...item.answer.notes.map((n) => `Note: ${n}`),
      '',
    )
  }
  return lines.join('\n')
}
