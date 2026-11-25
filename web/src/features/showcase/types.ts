import type { WorkMode } from '@/types/opportunities'

/** Mirrors Domain.Enums.PartnerInterest. */
export const PARTNER_INTERESTS = ['IndustryTasks', 'Internships', 'Events', 'Mentoring', 'Other'] as const
export type PartnerInterest = (typeof PARTNER_INTERESTS)[number]

/** Mirrors Features/Showcase/Dtos/ShowcaseDto. */
export interface Showcase {
  stats: {
    partnerCompanies: number
    openInternships: number
    industryTasks: number
    studentsOnCareerPaths: number
    certificatesIssued: number
    badgesAwarded: number
  }
  careerPaths: { slug: string; name: string; tagline: string; skillCount: number }[]
  partners: { name: string; tasks: number; internships: number; events: number }[]
  internships: { title: string; companyName: string; careerPathName: string; workMode: WorkMode; location: string; closingDate: string }[]
  upcomingEvents: { title: string; kind: string; host: string | null; startDate: string; endDate: string; location: string | null }[]
  featuredProfiles: { fullName: string; programme: string; careerPathName: string | null; badges: string[]; cardSlug: string }[]
}

export interface PartnerEnquiryInput {
  organisationName: string
  contactName: string
  email: string
  phone: string | null
  interest: PartnerInterest
  message: string
  /** Honeypot: hidden from people, so it stays empty unless a bot fills it in. */
  website: string
}

export interface PartnerEnquiry {
  id: number
  organisationName: string
  contactName: string
  email: string
  phone: string | null
  interest: PartnerInterest
  message: string
  submittedAt: string
  isHandled: boolean
  handledAt: string | null
  handledBy: string | null
}
