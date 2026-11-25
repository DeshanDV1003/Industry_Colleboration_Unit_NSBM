import { useEffect, useRef } from 'react'
import { Bot, Loader2, User } from 'lucide-react'
import { cn } from '@/lib/cn'
import type { AiInterviewTurn } from '../types'

/** The conversation so far. Scrolls to the newest message. */
export function InterviewTranscript({ turns, thinking }: { turns: AiInterviewTurn[]; thinking: boolean }) {
  const endRef = useRef<HTMLDivElement>(null)
  // Block body on purpose: scrollIntoView may return a Promise, which React would mistake for a cleanup function.
  useEffect(() => {
    endRef.current?.scrollIntoView({ behavior: 'smooth', block: 'end' })
  }, [turns.length, thinking])

  return (
    <div className="space-y-4" aria-live="polite">
      {turns.map((t) => {
        const interviewer = t.speaker === 'Interviewer'
        return (
          <div key={t.order} className={cn('flex gap-3', !interviewer && 'flex-row-reverse')}>
            <span className={cn('grid size-8 shrink-0 place-items-center rounded-full', interviewer ? 'bg-primary-soft text-primary' : 'bg-accent-soft text-accent')}>
              {interviewer ? <Bot className="size-4" /> : <User className="size-4" />}
            </span>
            <p className={cn('max-w-[80%] whitespace-pre-line rounded-2xl px-4 py-2.5 text-sm',
              interviewer ? 'rounded-tl-sm bg-surface-subtle text-heading' : 'rounded-tr-sm bg-primary text-white')}>
              <span className="sr-only">{interviewer ? 'Interviewer: ' : 'You: '}</span>{t.text}
            </p>
          </div>
        )
      })}
      {thinking && (
        <div className="flex gap-3">
          <span className="grid size-8 shrink-0 place-items-center rounded-full bg-primary-soft text-primary"><Bot className="size-4" /></span>
          <p className="flex items-center gap-2 rounded-2xl rounded-tl-sm bg-surface-subtle px-4 py-2.5 text-sm text-muted">
            <Loader2 className="size-4 animate-spin" /> The interviewer is thinking…
          </p>
        </div>
      )}
      <div ref={endRef} />
    </div>
  )
}
