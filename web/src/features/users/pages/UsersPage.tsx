import { useMemo, useState } from 'react'
import { FileUp, KeyRound, Pencil, Plus, Search, UserCheck, UserX, Users } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { PageHeader } from '@/components/layout/PageHeader'
import { Badge } from '@/components/ui/Badge'
import { Button } from '@/components/ui/Button'
import { Card } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { SegmentedControl } from '@/components/ui/SegmentedControl'
import { PageSpinner } from '@/components/ui/Spinner'
import { controlClass } from '@/components/ui/controlClass'
import { ROLES, ROLE_META, type Role } from '@/config/roles'
import { cn } from '@/lib/cn'
import { getErrorMessage } from '@/lib/http'
import { useSession } from '@/stores/authStore'
import { useSetUserActive, useUsers } from '../api/usersApi'
import { ImportStudentsDialog } from '../components/ImportStudentsDialog'
import { ResetPasswordDialog } from '../components/ResetPasswordDialog'
import { UserFormDialog } from '../components/UserFormDialog'
import type { UserSummary } from '../types'

type RoleFilter = Role | 'All'
type Open =
  | { kind: 'create'; role: Role }
  | { kind: 'edit'; userId: string }
  | { kind: 'reset'; user: UserSummary }
  | { kind: 'import' }

/** Every account on the platform: add, edit, deactivate, reset passwords and import a cohort. */
export default function UsersPage() {
  const myId = useSession()?.userId
  const { data: users, isLoading, isError, error } = useUsers()
  const setActive = useSetUserActive()
  const [role, setRole] = useState<RoleFilter>('All')
  const [search, setSearch] = useState('')
  const [open, setOpen] = useState<Open | null>(null)

  const visible = useMemo(() => {
    const term = search.trim().toLowerCase()
    return (users ?? []).filter((u) => (role === 'All' || u.role === role)
      && (!term || `${u.fullName} ${u.email} ${u.summary}`.toLowerCase().includes(term)))
  }, [users, role, search])

  if (isLoading) return <PageSpinner label="Loading accounts…" />
  if (isError || !users) return <EmptyState icon={Users} title="Accounts could not be loaded" description={getErrorMessage(error)} />

  const count = (r: RoleFilter) => users.filter((u) => r === 'All' || u.role === r).length

  const toggleActive = (u: UserSummary) => setActive.mutate({ userId: u.userId, active: !u.isActive }, {
    onSuccess: () => toast.success(u.isActive ? `${u.fullName} can no longer sign in.` : `${u.fullName} can sign in again.`),
    onError: (e) => toast.error(getErrorMessage(e)),
  })

  return (
    <>
      <PageHeader
        title="Users"
        description="Accounts for students, lecturers, industry partners, the registrar and Unit admins. Deactivating keeps their history."
        actions={<>
          <Button variant="secondary" onClick={() => setOpen({ kind: 'import' })}><FileUp className="size-4" /> Import students</Button>
          <Button onClick={() => setOpen({ kind: 'create', role: role === 'All' ? 'Student' : role })}><Plus className="size-4" /> Add account</Button>
        </>}
      />

      <div className="mb-4 flex flex-wrap items-center gap-3">
        <SegmentedControl ariaLabel="Filter by role" value={role} onChange={setRole}
          options={[{ value: 'All' as RoleFilter, label: 'All', count: count('All') },
            ...ROLES.map((r) => ({ value: r as RoleFilter, label: ROLE_META[r].label, count: count(r) }))]} />
        <label className="relative ml-auto w-full sm:w-72">
          <Search className="pointer-events-none absolute left-3 top-1/2 size-4 -translate-y-1/2 text-muted" />
          <input aria-label="Search accounts" placeholder="Name, e-mail or number" value={search} onChange={(e) => setSearch(e.target.value)}
            className={cn(controlClass(false), 'h-10 pl-9')} />
        </label>
      </div>

      {visible.length === 0 ? <EmptyState icon={Users} title="No matching accounts" /> : (
        <Card className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead className="border-b border-line text-left text-xs uppercase tracking-wider text-muted">
              <tr>
                <th className="px-5 py-3">Name</th>
                <th className="px-5 py-3">Role</th>
                <th className="px-5 py-3">Details</th>
                <th className="px-5 py-3">Status</th>
                <th className="px-5 py-3"><span className="sr-only">Actions</span></th>
              </tr>
            </thead>
            <tbody>
              {visible.map((u) => (
                <tr key={u.userId} className={cn('border-b border-line last:border-0', !u.isActive && 'opacity-60')}>
                  <td className="px-5 py-3">
                    <p className="font-semibold text-heading">{u.fullName}{u.userId === myId && <span className="ml-1 text-xs font-normal text-muted">(you)</span>}</p>
                    <p className="text-xs text-muted">{u.email}</p>
                  </td>
                  <td className="px-5 py-3">{ROLE_META[u.role].label}</td>
                  <td className="px-5 py-3 text-muted">{u.summary}</td>
                  <td className="px-5 py-3">{u.isActive ? <Badge tone="success">Active</Badge> : <Badge tone="neutral">Deactivated</Badge>}</td>
                  <td className="whitespace-nowrap px-5 py-3 text-right">
                    <Button size="sm" variant="ghost" aria-label={`Edit ${u.fullName}`} onClick={() => setOpen({ kind: 'edit', userId: u.userId })}><Pencil className="size-4" /></Button>
                    <Button size="sm" variant="ghost" aria-label={`Reset password for ${u.fullName}`} onClick={() => setOpen({ kind: 'reset', user: u })}><KeyRound className="size-4" /></Button>
                    {u.userId !== myId && (
                      <Button size="sm" variant="ghost" className={u.isActive ? 'text-danger' : undefined}
                        aria-label={`${u.isActive ? 'Deactivate' : 'Reactivate'} ${u.fullName}`}
                        loading={setActive.isPending && setActive.variables?.userId === u.userId} onClick={() => toggleActive(u)}>
                        {u.isActive ? <UserX className="size-4" /> : <UserCheck className="size-4" />}
                      </Button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </Card>
      )}

      {open?.kind === 'create' && <UserFormDialog userId={null} initialRole={open.role} onClose={() => setOpen(null)} />}
      {open?.kind === 'edit' && <UserFormDialog userId={open.userId} onClose={() => setOpen(null)} />}
      {open?.kind === 'reset' && <ResetPasswordDialog user={open.user} onClose={() => setOpen(null)} />}
      {open?.kind === 'import' && <ImportStudentsDialog onClose={() => setOpen(null)} />}
    </>
  )
}
