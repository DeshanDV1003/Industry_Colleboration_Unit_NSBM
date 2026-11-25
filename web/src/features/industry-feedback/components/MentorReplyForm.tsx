import { useState } from 'react'
import { toast } from '@/components/feedback/toast'
import { Button } from '@/components/ui/Button'
import { controlClass } from '@/components/ui/controlClass'
import { useReplyToMentorQuestion } from '@/features/mentoring'
import { cn } from '@/lib/cn'
import { getErrorMessage } from '@/lib/http'

export function MentorReplyForm({ questionId }: { questionId: number }) {
  const [reply, setReply] = useState('')
  const send = useReplyToMentorQuestion()

  return (
    <div className="mt-4 space-y-2">
      <textarea aria-label="Your reply" rows={3} value={reply} onChange={(e) => setReply(e.target.value)}
        placeholder="Share practical, encouraging advice…" className={cn(controlClass(false), 'py-2.5')} />
      <Button size="sm" loading={send.isPending} disabled={reply.trim().length < 20}
        onClick={() => send.mutate({ id: questionId, reply }, {
          onSuccess: () => toast.success('Reply sent to the student.'),
          onError: (e) => toast.error(getErrorMessage(e)),
        })}>
        Send reply
      </Button>
    </div>
  )
}
