import { Briefcase, Building2, GraduationCap, ShieldCheck, Presentation, type LucideIcon } from 'lucide-react'

/** Mirrors NSBM.ICU.Domain.Constants.Roles on the API. */
export const ROLES = ['Student', 'Lecturer', 'Industry', 'Admin', 'Registrar'] as const
export type Role = (typeof ROLES)[number]

interface RoleMeta {
  label: string
  portal: string
  description: string
  icon: LucideIcon
}

export const ROLE_META: Record<Role, RoleMeta> = {
  Student: {
    label: 'Student',
    portal: 'Student portal',
    description: 'Learn, practise, prepare for interviews and apply for internships.',
    icon: GraduationCap,
  },
  Lecturer: {
    label: 'Lecturer',
    portal: 'Lecturer portal',
    description: 'Follow student progress, assign practice and write support notes.',
    icon: Presentation,
  },
  Industry: {
    label: 'Industry partner',
    portal: 'Industry portal',
    description: 'Publish tasks and vacancies, review applicants and mentor students.',
    icon: Briefcase,
  },
  Admin: {
    label: 'Unit admin',
    portal: 'Admin portal',
    description: 'Approve tasks and vacancies, view research data and manage the platform.',
    icon: ShieldCheck,
  },
  Registrar: {
    label: 'Assistant Registrar',
    portal: 'Registrar portal',
    description: 'Allocate lecture halls to student cohorts and halls to events, without clashes.',
    icon: Building2,
  },
}
