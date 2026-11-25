import { request, type APIRequestContext } from '@playwright/test'
import { API_URL } from './env'

/** Demo accounts, mirroring Infrastructure/Identity/DemoAccounts and the seeded cohort. */
export const ACCOUNTS = {
  admin: { email: 'icu.head@nsbm.demo', password: 'Admin@123' },
  lecturer: { email: 'lecturer@nsbm.demo', password: 'Lecturer@123' },
  registrar: { email: 'registrar@nsbm.demo', password: 'Registrar@123' },
  student: { email: 'student@nsbm.demo', password: 'Student@123' }, // Sahan Fernando, DEMO-001
  kavindi: { email: 'demo-002@nsbm.demo', password: 'Student@123' },
  tharindu: { email: 'demo-003@nsbm.demo', password: 'Student@123' },
  amaya: { email: 'demo-004@nsbm.demo', password: 'Student@123' },
  partner: { email: 'partner@nsbm.demo', password: 'Industry@123' }, // Lanka CloudWorks
  qaPartner: { email: 'partner3@nsbm.demo', password: 'Industry@123' }, // Ceylon QualityFirst
} as const

export type AccountKey = keyof typeof ACCOUNTS

/** Matches web/src/stores/authStore Session. */
export interface Session {
  accessToken: string
  expiresAt: string
  userId: string
  email: string
  fullName: string
  role: 'Student' | 'Lecturer' | 'Industry' | 'Admin' | 'Registrar'
}

export async function apiContext(): Promise<APIRequestContext> {
  return request.newContext({ baseURL: `${API_URL}/` })
}

export async function signIn(api: APIRequestContext, account: AccountKey): Promise<Session> {
  const response = await api.post('auth/login', { data: ACCOUNTS[account] })
  if (!response.ok()) throw new Error(`Sign-in failed for ${account}: ${response.status()} ${await response.text()}`)
  const body = await response.json()
  return { accessToken: body.accessToken, expiresAt: body.expiresAt, userId: body.userId, email: body.email, fullName: body.fullName, role: body.role }
}

/** Calls an API endpoint as the given account and returns the parsed JSON (or null for empty bodies). */
export async function apiAs<T = unknown>(account: AccountKey, method: 'GET' | 'POST' | 'PUT', path: string, data?: unknown): Promise<T> {
  const api = await apiContext()
  try {
    const session = await signIn(api, account)
    const response = await api.fetch(path, { method, data, headers: { Authorization: `Bearer ${session.accessToken}` } })
    if (!response.ok()) throw new Error(`${method} ${path} as ${account}: ${response.status()} ${await response.text()}`)
    const text = await response.text()
    return (text ? JSON.parse(text) : null) as T
  } finally {
    await api.dispose()
  }
}

/** Restores the seeded demo data, including the seeded accounts (accounts created by tests are removed). */
export async function resetDemoData(): Promise<void> {
  await apiAs('admin', 'POST', 'settings/demo-reset')
}
