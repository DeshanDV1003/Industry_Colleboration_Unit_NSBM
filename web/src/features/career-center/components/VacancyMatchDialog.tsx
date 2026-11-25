import { useState } from 'react'
import { Target } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { ApplicationStatusBadge } from '@/components/ApplicationStatusBadge'
import { MatchMeter } from '@/components/evidence/MatchMeter'
import { SkillEvidenceList } from '@/components/evidence/SkillEvidenceList'
import { Button } from '@/components/ui/Button'
import { Dialog } from '@/components/ui/Dialog'
import { FormField } from '@/components/ui/FormField'
import { LinkButton } from '@/components/ui/LinkButton'
import { controlClass } from '@/components/ui/controlClass'
import { WORK_MODE_LABEL } from '@/config/opportunities'
import { cn } from '@/lib/cn'
import { formatDate } from '@/lib/format'
import { getErrorMessage } from '@/lib/http'
import { useApplyToVacancy } from '../api/careerCenterApi'
import type { VacancyMatch } from '../types'

/** Mirrors ApplyToVacancyCommandValidator. */
const MIN_COVER_NOTE = 30

export function VacancyMatchDialog({ vacancy, onClose }: { vacancy: VacancyMatch | null; onClose: () => void }) {
  const [coverNote, setCoverNote] = useState('')
  const apply = useApplyToVacancy()

  const close = () => {
    setCoverNote('')
    onClose()
  }

  if (!vacancy) return null

  const submit = () =>
    apply.mutate(
      { vacancyId: vacancy.id, coverNote },
      {
        onSuccess: () => {
          toast.success(`Application sent to ${vacancy.companyName}. Track it in My applications.`)
          close()
        },
        onError: (e) => toast.error(getErrorMessage(e)),
      },
    )

  const remaining = MIN_COVER_NOTE - coverNote.trim().length

  return (
    <Dialog
      open
      onClose={close}
      className="max-w-3xl"
      title={vacancy.title}
      description={`${vacancy.companyName} · ${WORK_MODE_LABEL[vacancy.workMode]} · ${vacancy.location} · ${vacancy.durationMonths} months · closes ${formatDate(vacancy.closingDate)}`}
      footer={
        <>
          <LinkButton to={`/readiness?vacancy=${vacancy.id}`} variant="secondary"><Target className="size-4" /> Full readiness view</LinkButton>
          {!vacancy.myApplicationStatus && (
            <Button loading={apply.isPending} disabled={remaining > 0} onClick={submit}>Send practice application</Button>
          )}
        </>
      }
    >
      <div className="space-y-6">
        <p className="whitespace-pre-line">{vacancy.description}</p>
        <MatchMeter percent={vacancy.matchPercent} />

        <div className="grid gap-6 md:grid-cols-2">
          <section>
            <h3 className="mb-1 text-xs font-bold uppercase tracking-wider text-muted">Required skills</h3>
            <SkillEvidenceList rows={vacancy.required} compact />
          </section>
          <section>
            <h3 className="mb-1 text-xs font-bold uppercase tracking-wider text-muted">Preferred skills</h3>
            <SkillEvidenceList rows={vacancy.preferred} compact />
          </section>
        </div>

        {vacancy.myApplicationStatus ? (
          <div className="flex items-center justify-between rounded-xl bg-surface-subtle px-4 py-3 text-sm">
            <span className="font-medium text-heading">You applied to this vacancy.</span>
            <ApplicationStatusBadge status={vacancy.myApplicationStatus} />
          </div>
        ) : (
          <FormField
            label="Cover note"
            htmlFor="cover-note"
            hint={remaining > 0 ? `${remaining} more characters needed` : 'This is a practice application to a fictional vacancy.'}
          >
            <textarea
              id="cover-note"
              rows={4}
              value={coverNote}
              onChange={(e) => setCoverNote(e.target.value)}
              className={cn(controlClass(false), 'py-2.5')}
              placeholder="Why are you interested, and what have you practised that fits this role?"
            />
          </FormField>
        )}
      </div>
    </Dialog>
  )
}
