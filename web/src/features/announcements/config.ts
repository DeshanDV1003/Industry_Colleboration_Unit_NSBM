import type { AnnouncementAudience } from './types'

export const AUDIENCE_META: Record<AnnouncementAudience, { label: string; description: string }> = {
  Everyone: { label: 'Everyone', description: 'Every student, lecturer, partner, the registrar and Unit staff.' },
  Students: { label: 'Students', description: 'Every student account.' },
  Lecturers: { label: 'Lecturers', description: 'Every lecturer.' },
  Partners: { label: 'Industry partners', description: 'Every partner company contact.' },
  Staff: { label: 'University staff', description: 'Lecturers, the Assistant Registrar and Unit admins.' },
}

/** Mirrors SendAnnouncementCommandValidator.MaxMessageLength on the API. */
export const MAX_MESSAGE_LENGTH = 500
