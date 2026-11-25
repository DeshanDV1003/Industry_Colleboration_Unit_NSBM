import type { BadgeTone } from '@/components/ui/Badge'
import type { LearningStage, SkillSignal } from '@/types/learning'

export const SKILL_SIGNAL_META: Record<SkillSignal, { label: string; tone: BadgeTone }> = {
  Strong: { label: 'Strong', tone: 'success' },
  Developing: { label: 'Developing', tone: 'warning' },
  NeedsPractice: { label: 'Needs practice', tone: 'danger' },
}

export const LEARNING_STAGES: LearningStage[] = ['Beginner', 'Intermediate', 'Advanced']

export const LEARNING_STAGE_META: Record<LearningStage, { label: string; tone: BadgeTone }> = {
  Beginner: { label: 'Beginner', tone: 'primary' },
  Intermediate: { label: 'Intermediate', tone: 'accent' },
  Advanced: { label: 'Advanced', tone: 'warning' },
}
