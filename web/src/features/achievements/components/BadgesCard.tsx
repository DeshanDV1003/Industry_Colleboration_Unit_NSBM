import { useState } from 'react'
import { BadgeCheck, Clock, Send, XCircle } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { Badge } from '@/components/ui/Badge'
import { Button } from '@/components/ui/Button'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { Dialog } from '@/components/ui/Dialog'
import { FormField } from '@/components/ui/FormField'
import { controlClass } from '@/components/ui/controlClass'
import { cn } from '@/lib/cn'
import { formatDate } from '@/lib/format'
import { getErrorMessage } from '@/lib/http'
import { useRequestBadge } from '../api/achievementsApi'
import type { Achievements, BadgeEligibleSkill } from '../types'

function RequestDialog({ skill, onClose }: { skill: BadgeEligibleSkill | null; onClose: () => void }) {
  const request = useRequestBadge()
  const [note, setNote] = useState('')

  const close = () => { setNote(''); onClose() }
  const submit = () => skill && request.mutate({ skillId: skill.skillId, note: note.trim() || null }, {
    onSuccess: () => { toast.success('Sent to your lecturers for review.'); close() },
    onError: (e) => toast.error(getErrorMessage(e)),
  })

  return (
    <Dialog
      open={skill !== null}
      onClose={close}
      title={`Ask for a ${skill?.skillName} badge`}
      description="A lecturer checks your practice evidence and decides."
      footer={<><Button variant="secondary" onClick={close}>Cancel</Button><Button loading={request.isPending} onClick={submit}><Send className="size-4" /> Send request</Button></>}
    >
      {skill && (
        <div className="space-y-4 text-sm">
          <div>
            <p className="mb-1.5 font-medium text-heading">Evidence the lecturer will see</p>
            <ul className="list-disc space-y-1 pl-5">{skill.sources.map((s) => <li key={s}>{s}</li>)}</ul>
          </div>
          <FormField label="Anything to add? (optional)" htmlFor="badge-note" hint="For example, a project where you used this skill.">
            <textarea id="badge-note" rows={3} maxLength={500} value={note} onChange={(e) => setNote(e.target.value)} className={cn(controlClass(false), 'py-2')} />
          </FormField>
        </div>
      )}
    </Dialog>
  )
}

/** Earned and requested badges, plus the skills with enough practice evidence to ask for one. */
export function BadgesCard({ data }: { data: Achievements }) {
  const [requesting, setRequesting] = useState<BadgeEligibleSkill | null>(null)
  const canAsk = data.eligibleSkills.filter((s) => s.badgeStatus === null || s.badgeStatus === 'Rejected')

  return (
    <Card>
      <CardHeader title="Skill badges" description="Practise a skill, then ask a lecturer to review it. Approved badges appear on your profile card." />
      <CardBody className="space-y-5 text-sm">
        {data.badges.length > 0 && (
          <ul className="space-y-2">
            {data.badges.map((b) => (
              <li key={b.id} className="flex flex-wrap items-start gap-3 rounded-xl border border-line p-3">
                {b.status === 'Approved' ? <BadgeCheck className="size-5 text-success" /> : b.status === 'Pending' ? <Clock className="size-5 text-warning" /> : <XCircle className="size-5 text-danger" />}
                <div className="min-w-0 flex-1">
                  <p className="font-semibold text-heading">{b.skillName}</p>
                  <p className="text-xs text-muted">
                    {b.status === 'Approved' && <>Approved by {b.reviewerName} on {formatDate(b.reviewedAt!)} · {b.verificationCode}</>}
                    {b.status === 'Pending' && <>Requested {formatDate(b.requestedAt)} · waiting for a lecturer</>}
                    {b.status === 'Rejected' && <>Not yet ({b.reviewerName}): {b.reviewNote}</>}
                  </p>
                </div>
                <Badge tone={b.status === 'Approved' ? 'success' : b.status === 'Pending' ? 'warning' : 'danger'}>
                  {b.status === 'Approved' ? 'Earned' : b.status === 'Pending' ? 'In review' : 'Not yet'}
                </Badge>
              </li>
            ))}
          </ul>
        )}

        <div>
          <p className="mb-2 text-xs font-semibold uppercase tracking-wide text-muted">Ready to ask for</p>
          {canAsk.length === 0
            ? <p className="text-muted">{data.eligibleSkills.length === 0 ? 'Complete industry tasks, knowledge checks or the baseline to build practice evidence.' : 'You have asked for every skill you have evidence for.'}</p>
            : (
              <div className="flex flex-wrap gap-2">
                {canAsk.map((s) => (
                  <Button key={s.skillId} size="sm" variant="secondary" onClick={() => setRequesting(s)}>
                    <BadgeCheck className="size-4" /> {s.skillName}{s.badgeStatus === 'Rejected' && ' (ask again)'}
                  </Button>
                ))}
              </div>
            )}
        </div>
      </CardBody>
      <RequestDialog skill={requesting} onClose={() => setRequesting(null)} />
    </Card>
  )
}
