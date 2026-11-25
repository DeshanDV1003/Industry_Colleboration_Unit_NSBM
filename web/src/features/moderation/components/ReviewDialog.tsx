import { useRef, useState, type ReactNode } from 'react'
import { CheckCircle2, XCircle } from 'lucide-react'
import { ModerationStatusBadge } from '@/components/ModerationStatusBadge'
import { Button } from '@/components/ui/Button'
import { Dialog } from '@/components/ui/Dialog'
import { FormField } from '@/components/ui/FormField'
import { Spinner } from '@/components/ui/Spinner'
import { controlClass } from '@/components/ui/controlClass'
import { cn } from '@/lib/cn'
import type { ModerationStatus } from '@/types/tasks'
import type { ReviewDecision } from '../api/moderationApi'

/** The fields every reviewable item shares. */
export interface ReviewableSummary {
  title: string
  companyName: string
  status: ModerationStatus
  canBeReviewed: boolean
}

interface ReviewDialogProps {
  open: boolean
  item: ReviewableSummary | undefined
  pendingDecision: ReviewDecision | null
  onDecide: (decision: ReviewDecision, note: string | null, done: () => void) => void
  onClose: () => void
  children: ReactNode
}

const NOTE_REQUIRED = 'Tell the company why this was rejected so they can improve it.'

/** Shell for reviewing partner content: shows the item, a note field and Approve / Reject. */
export function ReviewDialog({ open, item, pendingDecision, onDecide, onClose, children }: ReviewDialogProps) {
  const [note, setNote] = useState('')
  const [noteError, setNoteError] = useState<string | null>(null)
  const noteRef = useRef<HTMLTextAreaElement>(null)

  const close = () => {
    setNote('')
    setNoteError(null)
    onClose()
  }

  const decide = (decision: ReviewDecision) => {
    if (decision === 'Reject' && !note.trim()) {
      setNoteError(NOTE_REQUIRED)
      noteRef.current?.scrollIntoView({ behavior: 'smooth', block: 'start' })
      noteRef.current?.focus({ preventScroll: true })
      return
    }
    onDecide(decision, note.trim() || null, close)
  }

  return (
    <Dialog
      open={open}
      onClose={close}
      className="max-w-3xl"
      title={item?.title ?? 'Review'}
      description={item && <span className="flex items-center gap-2">{item.companyName} <ModerationStatusBadge status={item.status} /></span>}
      footer={item?.canBeReviewed && (
        <>
          <Button variant="danger" loading={pendingDecision === 'Reject'} disabled={pendingDecision !== null} onClick={() => decide('Reject')}>
            <XCircle className="size-4" /> Reject
          </Button>
          <Button variant="accent" loading={pendingDecision === 'Approve'} disabled={pendingDecision !== null} onClick={() => decide('Approve')}>
            <CheckCircle2 className="size-4" /> Approve
          </Button>
        </>
      )}
    >
      {!item ? <Spinner className="py-10" /> : (
        <>
          {children}
          {item.canBeReviewed && (
            <FormField label="Note to the company" htmlFor="review-note" error={noteError ?? undefined} hint="Required when rejecting; optional when approving." className="mt-6">
              <textarea
                id="review-note"
                ref={noteRef}
                rows={3}
                value={note}
                onChange={(e) => { setNote(e.target.value); setNoteError(null) }}
                className={cn(controlClass(!!noteError), 'py-2.5')}
                placeholder="e.g. Please make the required skills more specific."
              />
            </FormField>
          )}
        </>
      )}
    </Dialog>
  )
}
