import { Mail } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { Button } from '@/components/ui/Button'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { getErrorMessage } from '@/lib/http'
import { useSendTestEmail } from '../api/settingsApi'

/** Checks the e-mail settings after a deployment by sending a message to the signed-in admin. */
export function TestEmailCard() {
  const send = useSendTestEmail()

  return (
    <Card>
      <CardHeader title="E-mail delivery" description="Updates, announcements, security alerts and the weekly summary are also sent by e-mail." />
      <CardBody className="space-y-3 text-sm">
        <p className="text-muted">Send yourself a test message to check that this server can deliver e-mail.</p>
        <Button size="sm" variant="secondary" loading={send.isPending}
          onClick={() => send.mutate(undefined, {
            onSuccess: ({ sentTo }) => toast.success(`Test e-mail sent to ${sentTo}.`),
            onError: (e) => toast.error(getErrorMessage(e)),
          })}>
          <Mail className="size-4" /> Send test e-mail
        </Button>
      </CardBody>
    </Card>
  )
}
