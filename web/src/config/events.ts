import { Lightbulb, Mic, Trophy, Wrench, type LucideIcon } from 'lucide-react'
import type { BadgeTone } from '@/components/ui/Badge'

/** Mirrors Domain.Enums.EventFormat. */
export const EVENT_FORMATS = ['TechTalk', 'Workshop', 'Challenge', 'Hackathon'] as const
export type EventFormat = (typeof EVENT_FORMATS)[number]

export const EVENT_FORMAT_META: Record<EventFormat, { label: string; tone: BadgeTone; icon: LucideIcon }> = {
  TechTalk: { label: 'Tech talk', tone: 'accent', icon: Mic },
  Workshop: { label: 'Workshop', tone: 'primary', icon: Wrench },
  Challenge: { label: 'Challenge', tone: 'warning', icon: Lightbulb },
  Hackathon: { label: 'Hackathon', tone: 'success', icon: Trophy },
}

/** "Teams of 2–4" or "Individual". */
export const teamSizeLabel = (min: number | null, max: number | null) =>
  max === null ? 'Individual' : min === max ? `Teams of ${max}` : `Teams of ${min}–${max}`
