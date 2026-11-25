import type { Role } from '@/config/roles'

/** Mirrors Domain.Enums.AuditCategory. */
export const AUDIT_CATEGORIES = ['Accounts', 'Moderation', 'Halls', 'Communication', 'Applications', 'Platform'] as const
export type AuditCategory = (typeof AUDIT_CATEGORIES)[number]

/** Mirrors Features/Audit/Queries/GetAuditLog/AuditEntryDto. */
export interface AuditEntry {
  id: number
  at: string
  actorName: string
  actorRole: Role | null
  category: AuditCategory
  action: string
  summary: string
}

export interface AuditPage {
  items: AuditEntry[]
  total: number
  page: number
  pageSize: number
}

/** Mirrors Features/Audit/AuditFilter. Empty strings mean "any". */
export interface AuditFilter {
  category: AuditCategory | ''
  search: string
  from: string
  to: string
}
