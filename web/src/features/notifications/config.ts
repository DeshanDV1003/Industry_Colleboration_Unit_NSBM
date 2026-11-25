import {
  Award, BadgeCheck, Hand, ShieldAlert, CalendarClock, CalendarX, DoorOpen, Megaphone, Briefcase, CalendarHeart, Inbox, CalendarDays, ClipboardCheck, Compass, Gauge, Map, MessageSquareText, MessagesSquare,
  NotebookPen, ShieldCheck, Wrench, type LucideIcon,
} from 'lucide-react'
import type { NextStep, NotificationKind } from './types'

export const NOTIFICATION_ICON: Record<NotificationKind, LucideIcon> = {
  ApplicationUpdate: Briefcase,
  MentorReply: MessagesSquare,
  TaskReview: MessageSquareText,
  LecturerNote: NotebookPen,
  TaskAssigned: Wrench,
  ContentReviewed: ShieldCheck,
  ContentSubmitted: ClipboardCheck,
  CalendarEvent: CalendarDays,
  IndustryEvent: CalendarHeart,
  CertificateIssued: Award,
  BadgeRequested: BadgeCheck,
  BadgeReviewed: BadgeCheck,
  PartnerEnquiry: Inbox,
  HallAssigned: DoorOpen,
  HallBookingCancelled: CalendarX,
  Announcement: Megaphone,
  HallNeeded: CalendarClock,
  Welcome: Hand,
  AccountSecurity: ShieldAlert,
}

interface StepView { icon: LucideIcon; text: string; to: string }

/** Turns a next-step kind from the API into wording and a link. */
export function describeStep(step: NextStep): StepView {
  switch (step.kind) {
    case 'ChooseCareerPath':
      return { icon: Compass, text: 'Choose a career path to unlock your roadmap.', to: '/pathways' }
    case 'TakeBaseline':
      return { icon: Gauge, text: 'Take the skills baseline for your career path.', to: '/baseline' }
    case 'ContinueRoadmap':
      return { icon: Map, text: `Next roadmap step: ${step.subject}`, to: '/roadmap' }
    case 'OverduePlannerItems':
      return { icon: CalendarDays, text: `${step.count} planner item${step.count === 1 ? ' is' : 's are'} overdue.`, to: '/planner' }
    case 'TryAssignedTask':
      return { icon: Wrench, text: `Try your assigned task: ${step.subject}`, to: `/tasks/${step.targetId}` }
    case 'ReviewPendingContent':
      return { icon: ShieldCheck, text: `${step.count} partner item${step.count === 1 ? '' : 's'} waiting for approval.`, to: '/admin' }
    case 'AllocateEventHalls':
      return { icon: CalendarClock, text: `${step.count} approved event${step.count === 1 ? ' needs' : 's need'} a hall.`, to: '/hall-bookings' }
  }
}
