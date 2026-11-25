import { Compass, GraduationCap, Mic, Wrench, ShieldCheck, type LucideIcon } from 'lucide-react'
import type { Role } from '@/config/roles'

export interface JourneyStep {
  title: string
  description: string
  icon: LucideIcon
}

/** The five-step student journey from the feature guide. */
export const STUDENT_JOURNEY: JourneyStep[] = [
  { title: 'Explore', description: 'Choose a career path', icon: Compass },
  { title: 'Practise', description: 'Do industry-style tasks', icon: Wrench },
  { title: 'Verify', description: 'Show what you really understand', icon: ShieldCheck },
  { title: 'Interview', description: 'Practise soft-skills and technical interviews', icon: Mic },
  { title: 'Grow', description: 'Build a portfolio and a CV', icon: GraduationCap },
]

/** One-line promise of each portal shown in the dashboard hero. */
export const PORTAL_INTRO: Record<Role, string> = {
  Student: 'Pick a career role, practise company-style work, prepare for interviews and compare your skills with internship vacancies.',
  Lecturer: 'Find learning needs early, assign practice and leave support notes. The platform never ranks students automatically.',
  Industry: 'Show students what entry-level work looks like — publish challenges and vacancies, review applicants and mentor.',
  Registrar: 'Allocate lecture halls to cohorts and halls to events. The system checks capacity and clashes and tells everyone affected.',
  Admin: 'Keep the platform organised: review industry tasks and vacancies before students see them, and follow participation.',
}

/** Responsible-use reminders from the feature guide, tailored to each portal. */
export const RESPONSIBLE_USE: Record<Role, string[]> = {
  Student: [
    'Baseline and progress numbers are practice signals, not certified measures of ability.',
    'Match percentages count overlapping skills only — they do not predict hiring.',
    'Camera and voice features are optional. Nothing is recorded and text-only practice is always available.',
  ],
  Lecturer: [
    'Support signals are prompts to reach out, not judgements of a student.',
    'Interview practice has no live AI grading — feedback uses simple topic checks.',
  ],
  Industry: [
    'Candidate evidence helps a human reviewer. It never makes an automatic hiring decision.',
    'Tasks and vacancies are published to students only after Unit approval.',
  ],
  Registrar: [
    'Students, the host company and Unit admins are notified of every allocation and cancellation.',
    'Past bookings are kept as a record and cannot be cancelled.',
  ],
  Admin: [
    'Research counters are sample measures for planning, not research findings.',
    'Approve content that reflects realistic entry-level work and fair requirements.',
  ],
}
