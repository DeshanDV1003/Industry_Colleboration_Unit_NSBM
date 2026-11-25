import { z } from 'zod'
import { passwordSchema } from '@/config/passwordPolicy'
import { ROLES, type Role } from '@/config/roles'
import type { UserDetail, UserProfile } from './types'

/** A role's own fields, in form order. Mirrors UserProfileInputValidator on the API. */
export type ProfileField = 'studentNo' | 'programme' | 'academicYear' | 'batch' | 'staffNo' | 'designation' | 'department' | 'companyId' | 'jobTitle' | 'office'

export const ROLE_FIELDS: Record<Role, ProfileField[]> = {
  Student: ['studentNo', 'programme', 'academicYear', 'batch'],
  Lecturer: ['staffNo', 'designation', 'department'],
  Industry: ['companyId', 'jobTitle'],
  Admin: ['designation'],
  Registrar: ['designation', 'office'],
}

export const FIELD_LABEL: Record<ProfileField, string> = {
  studentNo: 'Student number',
  programme: 'Degree programme',
  academicYear: 'Academic year',
  batch: 'Batch',
  staffNo: 'Staff number',
  designation: 'Designation',
  department: 'Department',
  companyId: 'Company',
  jobTitle: 'Job title',
  office: 'Office',
}

export const FIELD_PLACEHOLDER: Partial<Record<ProfileField, string>> = {
  studentNo: 'NSBM-2026-001',
  batch: '2026.1',
  staffNo: 'ST-1100',
}

const profileShape = {
  firstName: z.string().trim().min(1, 'Enter the first name.').max(100),
  lastName: z.string().trim().min(1, 'Enter the last name.').max(100),
  studentNo: z.string().max(30),
  programme: z.string().max(150),
  academicYear: z.string(),
  batch: z.string().max(20),
  staffNo: z.string().max(30),
  designation: z.string().max(150),
  department: z.string().max(150),
  companyId: z.string(),
  jobTitle: z.string().max(150),
  office: z.string().max(150),
}

/** Every value is a string (inputs and selects); a role's fields are required only for that role. */
export const userFormSchema = z.object({
  mode: z.enum(['create', 'edit']),
  role: z.enum(ROLES),
  email: z.string().trim(),
  password: z.string(),
  profile: z.object(profileShape),
}).superRefine((v, ctx) => {
  if (v.mode === 'create') {
    if (!z.string().email().safeParse(v.email).success) ctx.addIssue({ code: 'custom', path: ['email'], message: 'Enter a valid e-mail address.' })
    const password = passwordSchema.safeParse(v.password)
    if (!password.success) ctx.addIssue({ code: 'custom', path: ['password'], message: password.error.issues[0].message })
  }
  for (const field of ROLE_FIELDS[v.role]) {
    if (v.profile[field].trim() === '') ctx.addIssue({ code: 'custom', path: ['profile', field], message: `Enter the ${FIELD_LABEL[field].toLowerCase()}.` })
  }
})

export type UserFormValues = z.infer<typeof userFormSchema>

const emptyProfile = (): UserFormValues['profile'] => ({
  firstName: '', lastName: '', studentNo: '', programme: '', academicYear: '1', batch: '', staffNo: '',
  designation: '', department: '', companyId: '', jobTitle: '', office: '',
})

export const newUserForm = (role: Role): UserFormValues => ({ mode: 'create', role, email: '', password: '', profile: emptyProfile() })

export const editUserForm = (user: UserDetail): UserFormValues => {
  const p = user.profile
  const text = (value: string | number | null) => (value === null ? '' : String(value))
  return {
    mode: 'edit',
    role: user.role,
    email: user.email,
    password: '',
    profile: {
      firstName: p.firstName, lastName: p.lastName, studentNo: text(p.studentNo), programme: text(p.programme),
      academicYear: text(p.academicYear ?? 1), batch: text(p.batch), staffNo: text(p.staffNo), designation: text(p.designation),
      department: text(p.department), companyId: text(p.companyId), jobTitle: text(p.jobTitle), office: text(p.office),
    },
  }
}

/** Sends only the chosen role's fields; the others go as null. */
export function toProfile(v: UserFormValues): UserProfile {
  const own = new Set(ROLE_FIELDS[v.role])
  const text = (field: ProfileField) => (own.has(field) ? v.profile[field].trim() || null : null)
  const number = (field: ProfileField) => (own.has(field) && v.profile[field] ? Number(v.profile[field]) : null)
  return {
    firstName: v.profile.firstName.trim(),
    lastName: v.profile.lastName.trim(),
    studentNo: text('studentNo'),
    programme: text('programme'),
    academicYear: number('academicYear'),
    batch: text('batch'),
    staffNo: text('staffNo'),
    designation: text('designation'),
    department: text('department'),
    companyId: number('companyId'),
    jobTitle: text('jobTitle'),
    office: text('office'),
  }
}
