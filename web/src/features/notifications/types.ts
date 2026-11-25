import type { CalendarEntry } from '@/features/calendar'

/** Mirrors Domain.Enums.NotificationKind. */
export type NotificationKind =
  | 'ApplicationUpdate' | 'MentorReply' | 'TaskReview' | 'LecturerNote'
  | 'TaskAssigned' | 'ContentReviewed' | 'ContentSubmitted' | 'CalendarEvent'
  | 'IndustryEvent' | 'CertificateIssued' | 'BadgeRequested' | 'BadgeReviewed' | 'PartnerEnquiry'
  | 'HallAssigned' | 'HallBookingCancelled' | 'Announcement' | 'HallNeeded'

/** Mirrors Features/Notifications/Dtos/NotificationDto. */
export interface AppNotification {
  id: number
  kind: NotificationKind
  title: string
  body: string
  link: string | null
  createdAt: string
  isRead: boolean
}

export interface NotificationList {
  unreadCount: number
  items: AppNotification[]
}

/** Mirrors Features/Notifications/Dtos/NextStepKind. */
export type NextStepKind =
  | 'ChooseCareerPath' | 'TakeBaseline' | 'ContinueRoadmap'
  | 'OverduePlannerItems' | 'TryAssignedTask' | 'ReviewPendingContent' | 'AllocateEventHalls'

export interface NextStep {
  kind: NextStepKind
  subject: string | null
  count: number | null
  targetId: number | null
}

/** Mirrors Features/Notifications/Dtos/WeeklyDigestDto. */
export interface WeeklyDigest {
  from: string
  to: string
  upcoming: CalendarEntry[]
  unreadCount: number
  recentUnread: AppNotification[]
  nextSteps: NextStep[]
}
