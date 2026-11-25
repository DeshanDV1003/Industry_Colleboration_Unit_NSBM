import type { ModerationStatus } from '@/types/tasks'

/** Mirrors Domain.Enums.ProgressLevel. */
export type ProgressLevel = 'Explorer' | 'Builder' | 'Practitioner' | 'Advanced' | 'RoadmapComplete'

export interface Progress {
  currentStreak: number
  bestStreak: number
  activeThisWeek: boolean
  /** Oldest first, ending this week. */
  recentWeeks: boolean[]
  level: ProgressLevel
  roadmapCompleted: number
  roadmapTotal: number
}

export interface Badge {
  id: number
  skillId: number
  skillName: string
  status: ModerationStatus
  requestedAt: string
  reviewedAt: string | null
  reviewerName: string | null
  reviewNote: string | null
  verificationCode: string | null
}

export interface BadgeEligibleSkill { skillId: number; skillName: string; sources: string[]; badgeStatus: ModerationStatus | null }

export interface ProfileCardSettings { isShared: boolean; slug: string | null }

/** Mirrors Features/Achievements/Dtos/AchievementsDto. */
export interface Achievements {
  progress: Progress
  badges: Badge[]
  eligibleSkills: BadgeEligibleSkill[]
  certificateCount: number
  profileCard: ProfileCardSettings
}

export interface BadgeReviewItem {
  id: number
  studentName: string
  studentNo: string
  programme: string
  careerPathName: string | null
  skillName: string
  evidence: string[]
  studentNote: string | null
  requestedAt: string
  status: ModerationStatus
  reviewerName: string | null
  reviewNote: string | null
  reviewedAt: string | null
}

/** Mirrors Features/Achievements/Dtos/ProfileCardDto. */
export interface ProfileCard {
  fullName: string
  programme: string
  careerPathName: string | null
  level: ProgressLevel
  currentStreak: number
  bestStreak: number
  badges: { skillName: string; verificationCode: string; approvedAt: string }[]
  certificates: { eventTitle: string; companyName: string; code: string; issuedAt: string }[]
}
