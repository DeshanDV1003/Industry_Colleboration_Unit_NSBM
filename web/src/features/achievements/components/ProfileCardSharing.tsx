import { ExternalLink, Link2, Share2 } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { Button } from '@/components/ui/Button'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { getErrorMessage } from '@/lib/http'
import { useSetProfileCardSharing } from '../api/achievementsApi'
import { profileCardUrl } from '../config'
import type { ProfileCardSettings } from '../types'

/** Turns the public profile card on or off and copies its link. Off until the student turns it on. */
export function ProfileCardSharing({ settings }: { settings: ProfileCardSettings }) {
  const sharing = useSetProfileCardSharing()
  const toggle = (share: boolean) => sharing.mutate(share, {
    onSuccess: () => toast.success(share ? 'Your profile card is now public to anyone with the link.' : 'Your profile card is now private.'),
    onError: (e) => toast.error(getErrorMessage(e)),
  })
  const url = settings.slug ? profileCardUrl(settings.slug) : null

  return (
    <Card>
      <CardHeader title="Shareable profile card" description="A public page with your badges, certificates, streak and level. Add it to your CV or LinkedIn." />
      <CardBody className="space-y-3 text-sm">
        <label className="flex items-center gap-2 font-medium text-heading">
          <input type="checkbox" className="size-4 accent-[var(--color-primary)]" checked={settings.isShared} disabled={sharing.isPending} onChange={(e) => toggle(e.target.checked)} />
          Anyone with the link can see my profile card
        </label>
        {url ? (
          <div className="flex flex-wrap gap-2">
            <Button size="sm" variant="secondary" onClick={() => navigator.clipboard.writeText(url).then(() => toast.success('Link copied.'))}><Link2 className="size-4" /> Copy link</Button>
            <a href={url} target="_blank" rel="noreferrer" className="inline-flex h-8 items-center gap-1.5 rounded-control border border-line-strong px-3 text-xs font-semibold text-heading hover:bg-surface-subtle"><ExternalLink className="size-4" /> Open card</a>
          </div>
        ) : (
          <p className="flex items-center gap-2 text-muted"><Share2 className="size-4" /> Private. Only you can see your achievements.</p>
        )}
        <p className="text-xs text-muted">The card never shows grades, scores, interview feedback or contact details.</p>
      </CardBody>
    </Card>
  )
}
