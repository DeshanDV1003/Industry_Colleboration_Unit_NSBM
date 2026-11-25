import { MessageSquareQuote } from 'lucide-react'
import { formatDate } from '@/lib/format'

export interface ReviewComment {
  reviewerName: string
  comment: string
  improvements: string | null
  reviewedAt: string
}

/** Industry review comments and improvement ideas on a student's work. */
export function ReviewComments({ reviews }: { reviews: ReviewComment[] }) {
  if (reviews.length === 0) return null
  return (
    <ul className="space-y-2">
      {reviews.map((r) => (
        <li key={`${r.reviewerName}-${r.reviewedAt}`} className="rounded-xl border border-line bg-accent-soft p-3 text-sm">
          <p className="flex items-center gap-1.5 text-xs font-semibold text-accent">
            <MessageSquareQuote className="size-3.5" /> {r.reviewerName} · {formatDate(r.reviewedAt)}
          </p>
          <p className="mt-1 text-heading">{r.comment}</p>
          {r.improvements && <p className="mt-1 text-muted"><span className="font-semibold">To improve:</span> {r.improvements}</p>}
        </li>
      ))}
    </ul>
  )
}
