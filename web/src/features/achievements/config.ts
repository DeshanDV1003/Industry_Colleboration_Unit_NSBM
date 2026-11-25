import type { ProgressLevel } from './types'

/** Level names describe roadmap progress, never ability. */
export const LEVEL_META: Record<ProgressLevel, { label: string; description: string; rank: number }> = {
  Explorer: { label: 'Explorer', description: 'Getting started on the roadmap', rank: 1 },
  Builder: { label: 'Builder', description: 'A quarter of the roadmap done', rank: 2 },
  Practitioner: { label: 'Practitioner', description: 'Half of the roadmap done', rank: 3 },
  Advanced: { label: 'Advanced', description: 'Three quarters of the roadmap done', rank: 4 },
  RoadmapComplete: { label: 'Roadmap complete', description: 'Every roadmap step done', rank: 5 },
}

export const LEVEL_COUNT = 5

/** The public page an employer opens from a shared profile card link. */
export const profileCardUrl = (slug: string) => `${window.location.origin}/card/${encodeURIComponent(slug)}`
