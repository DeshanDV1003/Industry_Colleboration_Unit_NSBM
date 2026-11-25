import { Mail } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { Spinner } from '@/components/ui/Spinner'
import { getErrorMessage } from '@/lib/http'
import { useEmailPreferences, useSaveEmailPreferences } from '../api/mySettingsApi'
import type { EmailPreferencesInput } from '../types'

interface ChoiceProps {
  id: string
  label: string
  description: string
  checked: boolean
  onChange: (checked: boolean) => void
}

function Choice({ id, label, description, checked, onChange }: ChoiceProps) {
  return (
    <label htmlFor={id} className="flex cursor-pointer items-start gap-3 rounded-xl border border-line p-3 hover:bg-surface-subtle">
      <input id={id} type="checkbox" className="mt-0.5 size-4 accent-[var(--color-primary)]" checked={checked}
        onChange={(e) => onChange(e.target.checked)} />
      <span>
        <span className="block text-sm font-medium text-heading">{label}</span>
        <span className="block text-xs text-muted">{description}</span>
      </span>
    </label>
  )
}

/** Which e-mails the person receives. Each change saves at once. */
export function EmailPreferencesCard() {
  const { data, isLoading, isError, error } = useEmailPreferences()
  const save = useSaveEmailPreferences()

  const change = (patch: Partial<EmailPreferencesInput>) => {
    if (!data) return
    save.mutate({ notificationsByEmail: data.notificationsByEmail, weeklyDigestByEmail: data.weeklyDigestByEmail, ...patch }, {
      onSuccess: () => toast.success('E-mail choices saved.'),
      onError: (e) => toast.error(getErrorMessage(e)),
    })
  }

  return (
    <Card>
      <CardHeader title="E-mail" description="Everything always appears under Notifications. Choose what also comes to your inbox." />
      <CardBody className="space-y-3">
        {isLoading ? <Spinner /> : isError || !data ? <p className="text-sm text-danger">{getErrorMessage(error)}</p> : (<>
          {!data.emailEnabled && (
            <p className="flex items-center gap-2 rounded-xl bg-surface-subtle px-3 py-2 text-xs text-muted">
              <Mail className="size-4" /> This server does not send e-mail yet. Your choices apply once it does.
            </p>
          )}
          <Choice id="pref-updates" label="Updates by e-mail" checked={data.notificationsByEmail}
            description="Application news, hall allocations, reviews, announcements and other updates meant for you. New updates arriving together come as one e-mail."
            onChange={(checked) => change({ notificationsByEmail: checked })} />
          <Choice id="pref-digest" label="Weekly summary" checked={data.weeklyDigestByEmail}
            description="Every Monday morning: your unread updates in one e-mail."
            onChange={(checked) => change({ weeklyDigestByEmail: checked })} />
          <p className="text-xs text-muted">Security alerts, such as a password change, are always e-mailed.</p>
        </>)}
      </CardBody>
    </Card>
  )
}
