/** Mirrors Domain.Enums.AnnouncementAudience. */
export const ANNOUNCEMENT_AUDIENCES = ['Everyone', 'Students', 'Lecturers', 'Partners', 'Staff'] as const
export type AnnouncementAudience = (typeof ANNOUNCEMENT_AUDIENCES)[number]

/** Mirrors Features/Announcements/Dtos/AnnouncementDto. */
export interface Announcement {
  id: number
  audience: AnnouncementAudience
  title: string
  message: string
  sentByName: string
  sentAt: string
  recipientCount: number
}

/** Mirrors Features/Announcements/Commands/SendAnnouncementCommand. */
export interface AnnouncementInput {
  audience: AnnouncementAudience
  title: string
  message: string
}
