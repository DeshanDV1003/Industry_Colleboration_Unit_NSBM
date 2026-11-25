import { useState } from 'react'
import { Pencil } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { Badge } from '@/components/ui/Badge'
import { Button } from '@/components/ui/Button'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { Dialog } from '@/components/ui/Dialog'
import { Spinner } from '@/components/ui/Spinner'
import { SkillPicker, useSkills } from '@/features/careers'
import { getErrorMessage } from '@/lib/http'
import { useMySkillProfile, useSetProfileSharing, useUpdateMySkills } from '../api/skillsProfileApi'

/** The student's self-reported skills and partner-sharing choice, editable in place. */
export function MySkillsCard() {
  const { data: profile, isLoading } = useMySkillProfile()
  const { data: skills } = useSkills()
  const updateSkills = useUpdateMySkills()
  const setSharing = useSetProfileSharing()
  const [editing, setEditing] = useState<number[] | null>(null)

  if (isLoading || !profile) return <Card className="grid h-40 place-items-center"><Spinner /></Card>

  const names = skills?.filter((s) => profile.skillIds.includes(s.id)).map((s) => s.name) ?? []

  const save = () =>
    updateSkills.mutate(editing!, {
      onSuccess: () => { setEditing(null); toast.success('Your skills were updated.') },
      onError: (e) => toast.error(getErrorMessage(e)),
    })

  const toggleSharing = () =>
    setSharing.mutate(!profile.shareProfileWithPartners, {
      onSuccess: () => toast.info(profile.shareProfileWithPartners ? 'Partners can no longer view your profile.' : 'Partner companies can now view your profile evidence.'),
      onError: (e) => toast.error(getErrorMessage(e)),
    })

  return (
    <Card>
      <CardHeader
        title="My skills"
        description="Skills you say you have. Listing a skill is a claim — practice adds the evidence."
        action={<Button variant="ghost" size="sm" onClick={() => setEditing(profile.skillIds)}><Pencil className="size-4" /> Edit</Button>}
      />
      <CardBody className="space-y-5">
        {names.length === 0
          ? <p className="text-sm text-muted">You have not listed any skills yet.</p>
          : <div className="flex flex-wrap gap-1.5">{names.map((n) => <Badge key={n}>{n}</Badge>)}</div>}

        <div className="flex items-start justify-between gap-4 rounded-xl border border-line p-3">
          <div>
            <p className="text-sm font-medium text-heading">Share my profile with partner companies</p>
            <p className="text-xs text-muted">Partners can see your skills and practice signals even before you apply.</p>
          </div>
          <button
            role="switch"
            aria-checked={profile.shareProfileWithPartners}
            aria-label="Share my profile with partner companies"
            disabled={setSharing.isPending}
            onClick={toggleSharing}
            className={`relative h-6 w-11 shrink-0 rounded-full transition-colors ${profile.shareProfileWithPartners ? 'bg-primary' : 'bg-line-strong'}`}
          >
            <span className={`absolute left-0.5 top-0.5 size-5 rounded-full bg-white shadow transition-transform ${profile.shareProfileWithPartners ? 'translate-x-5' : ''}`} />
          </button>
        </div>
      </CardBody>

      <Dialog
        open={editing !== null}
        onClose={() => setEditing(null)}
        title="Edit my skills"
        description="Pick the skills you would list on your CV."
        footer={<>
          <Button variant="secondary" onClick={() => setEditing(null)}>Cancel</Button>
          <Button loading={updateSkills.isPending} onClick={save}>Save skills</Button>
        </>}
      >
        {editing && <SkillPicker value={editing} onChange={setEditing} />}
      </Dialog>
    </Card>
  )
}
