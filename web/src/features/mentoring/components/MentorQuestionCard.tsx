import type { ReactNode } from 'react'
import { MessageCircleReply } from 'lucide-react'
import { Badge } from '@/components/ui/Badge'
import { Card } from '@/components/ui/Card'
import { formatDateTime } from '@/lib/format'
import type { MentorQuestion } from '../api/mentoringApi'

/** A question and, when answered, the mentor's reply. `footer` holds role-specific actions. */
export function MentorQuestionCard({ question, showStudent, footer }: { question: MentorQuestion; showStudent?: boolean; footer?: ReactNode }) {
  return (
    <Card className="p-5">
      <div className="flex flex-wrap items-start justify-between gap-2">
        <div>
          <h3 className="font-semibold">{question.title}</h3>
          <p className="text-xs text-muted">
            {showStudent && `${question.studentName} · `}{question.careerPathName && `${question.careerPathName} · `}asked {formatDateTime(question.askedAt)}
          </p>
        </div>
        <Badge tone={question.reply ? 'success' : 'warning'}>{question.reply ? 'Answered' : 'Waiting for a mentor'}</Badge>
      </div>
      <p className="mt-3 whitespace-pre-line text-sm">{question.body}</p>
      {question.reply && (
        <div className="mt-4 rounded-xl bg-primary-soft p-4 text-sm">
          <p className="flex items-center gap-1.5 text-xs font-semibold text-primary">
            <MessageCircleReply className="size-3.5" /> {question.repliedBy} · {question.repliedByCompany}
          </p>
          <p className="mt-1 whitespace-pre-line text-heading">{question.reply}</p>
        </div>
      )}
      {footer}
    </Card>
  )
}
