import {
  Award, BadgeCheck, Building2, CalendarClock, Megaphone, BarChart3, Bot, Inbox, Trophy, CalendarDays, CalendarHeart, BookOpenCheck, Briefcase, CalendarCheck, ClipboardCheck, Compass, FileText, FolderKanban,
  Gauge, LayoutDashboard, ListChecks, Map, MessagesSquare, Mic, NotebookPen, Settings, ShieldCheck, Target,
  UserCog, UserSearch, Users, Wrench, type LucideIcon,
} from 'lucide-react'
import type { Role } from './roles'

export interface NavItem {
  label: string
  to: string
  icon: LucideIcon
  /** False while the feature's milestone is not yet delivered; rendered as "Soon" and not linked. */
  ready: boolean
}

export interface NavSection {
  title?: string
  items: NavItem[]
}

const overview: NavItem = { label: 'Overview', to: '/dashboard', icon: LayoutDashboard, ready: true }
const calendar: NavItem = { label: 'Year calendar', to: '/calendar', icon: CalendarDays, ready: true }

/** Sidebar menu per portal. Paths follow the IndustryReady feature guide. */
export const NAVIGATION: Record<Role, NavSection[]> = {
  Student: [
    { items: [overview, calendar] },
    {
      title: 'Explore',
      items: [
        { label: 'Career pathways', to: '/pathways', icon: Compass, ready: true },
        { label: 'Skills baseline', to: '/baseline', icon: Gauge, ready: true },
        { label: 'Learning roadmap', to: '/roadmap', icon: Map, ready: true },
        { label: 'Readiness & gaps', to: '/readiness', icon: Target, ready: true },
      ],
    },
    {
      title: 'Practise',
      items: [
        { label: 'Industry tasks', to: '/tasks', icon: Wrench, ready: true },
        { label: 'Team simulation', to: '/team-simulation', icon: FolderKanban, ready: true },
        { label: 'Mentor center', to: '/mentor-center', icon: MessagesSquare, ready: true },
        { label: 'Events & hackathons', to: '/events', icon: CalendarHeart, ready: true },
        { label: 'Portfolio', to: '/portfolio', icon: Award, ready: true },
        { label: 'Achievements', to: '/achievements', icon: Trophy, ready: true },
      ],
    },
    {
      title: 'Interview',
      items: [
        { label: 'AI interviewer', to: '/ai-interview', icon: Bot, ready: true },
        { label: 'Soft-skills practice', to: '/interview-prep', icon: Mic, ready: true },
        { label: 'Technical interview', to: '/interview', icon: BookOpenCheck, ready: true },
        { label: 'Interview report', to: '/interview-report', icon: FileText, ready: true },
      ],
    },
    {
      title: 'Grow',
      items: [
        { label: 'CV studio', to: '/cv-studio', icon: NotebookPen, ready: true },
        { label: 'Career center', to: '/career-center', icon: Briefcase, ready: true },
        { label: 'My applications', to: '/applications', icon: ClipboardCheck, ready: true },
        { label: 'Planner', to: '/planner', icon: CalendarCheck, ready: true },
      ],
    },
  ],
  Lecturer: [
    { items: [overview, calendar] },
    {
      title: 'Students',
      items: [
        { label: 'Student directory', to: '/lecturer', icon: Users, ready: true },
        { label: 'Student insights', to: '/lecturer-insights', icon: UserSearch, ready: true },
        { label: 'Badge reviews', to: '/badge-reviews', icon: BadgeCheck, ready: true },
      ],
    },
    {
      title: 'Research',
      items: [{ label: 'Research analytics', to: '/research', icon: BarChart3, ready: true }],
    },
  ],
  Industry: [
    { items: [overview, calendar] },
    {
      title: 'Opportunities',
      items: [
        { label: 'Industry challenges', to: '/industry', icon: Wrench, ready: true },
        { label: 'Vacancies', to: '/industry-hub', icon: Briefcase, ready: true },
        { label: 'Candidate review', to: '/candidate-review', icon: UserSearch, ready: true },
        { label: 'Mentoring & reviews', to: '/industry-feedback', icon: MessagesSquare, ready: true },
        { label: 'Events & hackathons', to: '/industry-events', icon: CalendarHeart, ready: true },
      ],
    },
  ],
  Admin: [
    { items: [overview, calendar] },
    {
      title: 'Moderation',
      items: [
        { label: 'Task approvals', to: '/admin', icon: ShieldCheck, ready: true },
        { label: 'Vacancy approvals', to: '/admin-vacancies', icon: ListChecks, ready: true },
        { label: 'Event approvals', to: '/admin-events', icon: CalendarHeart, ready: true },
      ],
    },
    {
      title: 'Platform',
      items: [
        { label: 'Users', to: '/users', icon: UserCog, ready: true },
        { label: 'Announcements', to: '/announcements', icon: Megaphone, ready: true },
        { label: 'Hall allocations', to: '/hall-bookings', icon: CalendarClock, ready: true },
        { label: 'Partner enquiries', to: '/partner-enquiries', icon: Inbox, ready: true },
        { label: 'Research overview', to: '/research', icon: BarChart3, ready: true },
        { label: 'Settings', to: '/settings', icon: Settings, ready: true },
      ],
    },
  ],
  Registrar: [
    { items: [overview, calendar] },
    {
      title: 'Halls',
      items: [
        { label: 'Hall allocations', to: '/hall-bookings', icon: CalendarClock, ready: true },
        { label: 'Venues', to: '/venues', icon: Building2, ready: true },
      ],
    },
  ],
}
