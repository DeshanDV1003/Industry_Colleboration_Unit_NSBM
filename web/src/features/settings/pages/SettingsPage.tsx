import { useState } from 'react'
import { RotateCcw, Save, TriangleAlert } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { PageHeader } from '@/components/layout/PageHeader'
import { Button } from '@/components/ui/Button'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { Dialog } from '@/components/ui/Dialog'
import { FormField } from '@/components/ui/FormField'
import { controlClass } from '@/components/ui/controlClass'
import { useCurrentUser } from '@/features/auth'
import { cn } from '@/lib/cn'
import { getErrorMessage } from '@/lib/http'
import { useResetDemoData, useUpdateAdminProfile } from '../api/settingsApi'

function ProfileCard({ initial }: { initial: string }) {
  const [designation, setDesignation] = useState(initial)
  const update = useUpdateAdminProfile()

  return (
    <Card>
      <CardHeader title="Demo profile" description="How your role appears to other users." />
      <CardBody className="space-y-3">
        <FormField label="Designation" htmlFor="designation">
          <input id="designation" value={designation} onChange={(e) => setDesignation(e.target.value)} className={cn(controlClass(false), 'h-10')} />
        </FormField>
        <Button size="sm" loading={update.isPending} disabled={!designation.trim() || designation === initial}
          onClick={() => update.mutate(designation, { onSuccess: () => toast.success('Profile updated.'), onError: (e) => toast.error(getErrorMessage(e)) })}>
          <Save className="size-4" /> Save
        </Button>
      </CardBody>
    </Card>
  )
}

function ResetCard() {
  const [confirming, setConfirming] = useState(false)
  const reset = useResetDemoData()

  return (
    <Card>
      <CardHeader title="Reset demo data" description="Restore the original fictional sample data." />
      <CardBody className="space-y-3 text-sm">
        <p>Removes all student activity (baselines, roadmap ticks, attempts, interviews, CVs, planners, applications), lecturer notes and assignments, mentor questions, and partner-created tasks and vacancies — then reloads the original demo tasks, vacancies and cohort.</p>
        <p className="text-muted">Accounts, companies and the career catalogue are kept. This changes only demo records — it is not university-wide control.</p>
        <Button variant="danger" size="sm" onClick={() => setConfirming(true)}><RotateCcw className="size-4" /> Reset demo data</Button>
      </CardBody>

      <Dialog
        open={confirming}
        onClose={() => setConfirming(false)}
        title="Reset all demo data?"
        description="This cannot be undone."
        footer={<>
          <Button variant="secondary" onClick={() => setConfirming(false)}>Cancel</Button>
          <Button variant="danger" loading={reset.isPending}
            onClick={() => reset.mutate(undefined, {
              onSuccess: () => { setConfirming(false); toast.success('Demo data restored to its original state.') },
              onError: (e) => toast.error(getErrorMessage(e)),
            })}>
            Yes, reset
          </Button>
        </>}
      >
        <p className="flex gap-2 text-sm"><TriangleAlert className="mt-0.5 size-4 shrink-0 text-danger" /> Everything students, lecturers and partners have done in the demo will be removed.</p>
      </Dialog>
    </Card>
  )
}

export default function SettingsPage() {
  const { data: me } = useCurrentUser()

  return (
    <>
      <PageHeader title="Demo settings" description="Update the demo profile or restore the original demo data." />
      <div className="grid max-w-5xl gap-6 lg:grid-cols-2">
        {me && <ProfileCard key={me.headline} initial={me.headline ?? ''} />}
        <ResetCard />
      </div>
    </>
  )
}
