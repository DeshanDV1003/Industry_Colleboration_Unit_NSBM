// Shared API enums and shapes for vacancies, applications and skill evidence. Mirrors the API DTOs.

export type WorkMode = 'OnSite' | 'Hybrid' | 'Remote'
export type ApplicationStatus = 'Submitted' | 'UnderReview' | 'Shortlisted' | 'InterviewInvited' | 'NotSelected'
export type SkillEvidence = 'Missing' | 'ListedOnly' | 'PracticeSignal'

/** Mirrors Common/Evidence/SkillEvidenceDto. */
export interface SkillEvidenceRow {
  skillId: number
  skillName: string
  evidence: SkillEvidence
  practiceSources: string[]
}
