import type { SkillCategory, SkillImportance } from '@/types/learning'

/** Mirrors Features/Careers/Dtos/SkillDto. */
export interface Skill {
  id: number
  name: string
  category: SkillCategory
}

/** Mirrors Features/Careers/Dtos/CareerPathSummaryDto. */
export interface CareerPathSummary {
  id: number
  slug: string
  name: string
  tagline: string
  entryRoles: string[]
  coreSkills: string[]
  skillCount: number
  roadmapStepCount: number
  isSelected: boolean
}

export interface CareerSkill {
  skillId: number
  name: string
  category: SkillCategory
  importance: SkillImportance
  description: string | null
}

/** Mirrors Features/Careers/Dtos/CareerPathDetailDto. */
export interface CareerPathDetail {
  id: number
  slug: string
  name: string
  tagline: string
  description: string
  entryRoles: string[]
  skills: CareerSkill[]
  isSelected: boolean
}
