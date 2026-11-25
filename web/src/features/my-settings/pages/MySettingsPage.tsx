import { useState } from 'react'
import { KeyRound } from 'lucide-react'
import { PageHeader } from '@/components/layout/PageHeader'
import { Button } from '@/components/ui/Button'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { ChangePasswordDialog } from '@/features/auth'
import { EmailPreferencesCard } from '../components/EmailPreferencesCard'

/** Personal settings for every role: e-mail choices and password. */
export default function MySettingsPage() {
  const [changingPassword, setChangingPassword] = useState(false)

  return (
    <>
      <PageHeader title="My settings" description="Your e-mail choices and password." />
      <div className="grid max-w-5xl items-start gap-6 lg:grid-cols-2">
        <EmailPreferencesCard />
        <Card>
          <CardHeader title="Password" description="Replace a temporary password, or change yours at any time." />
          <CardBody>
            <Button variant="secondary" size="sm" onClick={() => setChangingPassword(true)}><KeyRound className="size-4" /> Change password</Button>
          </CardBody>
        </Card>
      </div>
      {changingPassword && <ChangePasswordDialog onClose={() => setChangingPassword(false)} />}
    </>
  )
}
