import { useState } from 'react'
import { Copy, Crown, LogOut, UserPlus, Users } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { Badge } from '@/components/ui/Badge'
import { Button } from '@/components/ui/Button'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { controlClass } from '@/components/ui/controlClass'
import { teamSizeLabel } from '@/config/events'
import { cn } from '@/lib/cn'
import { getErrorMessage } from '@/lib/http'
import { useCreateTeam, useJoinTeam, useLeaveTeam } from '../api/eventHubApi'
import type { EventDetail } from '../types'

const onError = (e: unknown) => toast.error(getErrorMessage(e))

/** Create a team, join one with a code, or see your team and share its code. */
export function TeamPanel({ detail }: { detail: EventDetail }) {
  const { event, myTeam, teams } = detail
  const create = useCreateTeam()
  const join = useJoinTeam()
  const leave = useLeaveTeam()
  const [name, setName] = useState('')
  const [code, setCode] = useState('')
  const editable = event.registrationOpen
  const control = cn(controlClass(false), 'h-10')

  const copyCode = (joinCode: string) =>
    navigator.clipboard.writeText(joinCode).then(() => toast.success('Join code copied. Share it with your teammates.'))

  return (
    <Card>
      <CardHeader title="Your team" description={teamSizeLabel(event.minTeamSize, event.maxTeamSize)} />
      <CardBody className="space-y-4 text-sm">
        {myTeam ? (
          <>
            <div className="rounded-xl bg-primary-soft p-4">
              <p className="font-semibold text-heading">{myTeam.name}</p>
              <ul className="mt-2 space-y-1">
                {myTeam.members.map((m) => (
                  <li key={m.name} className="flex items-center gap-1.5">
                    {m.isCaptain ? <Crown className="size-3.5 text-warning" aria-label="Captain" /> : <Users className="size-3.5 text-muted" />}
                    {m.name}{m.isMe && <span className="text-xs text-muted">(you)</span>}
                  </li>
                ))}
              </ul>
              <div className="mt-3 flex items-center gap-2">
                <span className="text-xs text-muted">Join code</span>
                <code className="rounded bg-surface px-2 py-1 font-mono text-sm font-bold tracking-widest text-heading">{myTeam.joinCode}</code>
                <Button variant="ghost" size="sm" aria-label="Copy join code" onClick={() => copyCode(myTeam.joinCode)}><Copy className="size-4" /></Button>
              </div>
            </div>
            {editable && (
              <Button variant="secondary" size="sm" loading={leave.isPending} onClick={() => leave.mutate(event.id, { onError })}>
                <LogOut className="size-4" /> Leave team
              </Button>
            )}
          </>
        ) : !editable ? (
          <p className="text-muted">Teams can no longer be changed for this event.</p>
        ) : (
          <>
            <form className="flex gap-2" onSubmit={(e) => { e.preventDefault(); create.mutate({ id: event.id, name }, { onSuccess: () => setName(''), onError }) }}>
              <input aria-label="New team name" className={control} value={name} onChange={(e) => setName(e.target.value)} placeholder="Team name" maxLength={60} />
              <Button type="submit" loading={create.isPending} disabled={name.trim().length < 2}><UserPlus className="size-4" /> Create</Button>
            </form>
            <form className="flex gap-2" onSubmit={(e) => { e.preventDefault(); join.mutate({ id: event.id, joinCode: code }, { onSuccess: () => setCode(''), onError }) }}>
              <input aria-label="Team join code" className={cn(control, 'font-mono uppercase tracking-widest')} value={code} onChange={(e) => setCode(e.target.value)} placeholder="Join code" maxLength={6} />
              <Button type="submit" variant="secondary" loading={join.isPending} disabled={code.trim().length !== 6}>Join</Button>
            </form>
          </>
        )}
        {teams.length > 0 && (
          <div>
            <p className="mb-1.5 text-xs font-semibold uppercase tracking-wide text-muted">Teams so far</p>
            <div className="flex flex-wrap gap-1.5">
              {teams.map((t) => <Badge key={t.id} tone={t.isFull ? 'neutral' : 'primary'}>{t.name} · {t.memberCount}{t.isFull ? ' (full)' : ''}</Badge>)}
            </div>
          </div>
        )}
      </CardBody>
    </Card>
  )
}
