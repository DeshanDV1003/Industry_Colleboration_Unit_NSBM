import type { ApplicationStatus, SkillEvidenceRow } from '@/types/opportunities'

/** Mirrors Features/Candidates/Dtos/CompanyApplicationDto. */
export interface CompanyApplication {
  id: number
  vacancyId: number
  vacancyTitle: string
  studentId: number
  studentName: string
  studentNo: string
  programme: string
  status: ApplicationStatus
  submittedAt: string
  matchPercentAtSubmission: number
  coverNote: string
}

/** Mirrors Features/Candidates/Dtos/SharedProfileDto. */
export interface SharedProfile {
  studentId: number
  studentName: string
  studentNo: string
  programme: string
  careerPathName: string | null
}

/** Mirrors Features/Candidates/Dtos/CandidateEvidenceDto. */
export interface CandidateEvidence {
  studentId: number
  studentName: string
  studentNo: string
  programme: string
  careerPathName: string | null
  vacancyId: number
  vacancyTitle: string
  matchPercent: number
  required: SkillEvidenceRow[]
  preferred: SkillEvidenceRow[]
  applicationId: number | null
  applicationStatus: ApplicationStatus | null
  coverNote: string | null
}
