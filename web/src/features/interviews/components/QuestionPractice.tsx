import { useState } from 'react'
import { ChevronDown, Lightbulb, Mic, MicOff, Send } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { Badge } from '@/components/ui/Badge'
import { Button } from '@/components/ui/Button'
import { controlClass } from '@/components/ui/controlClass'
import { cn } from '@/lib/cn'
import { getErrorMessage } from '@/lib/http'
import { useSubmitInterviewAnswer } from '../api/interviewsApi'
import { useSpeechToText } from '../hooks/useSpeechToText'
import type { AnswerInputMode, InterviewQuestion } from '../types'
import { AnswerFeedbackPanel } from './AnswerFeedbackPanel'

interface QuestionPracticeProps {
  question: InterviewQuestion
  index: number
  isFollowUp?: boolean
}

/** One question: answer by typing or voice, get topic feedback, then optionally try its follow-up scenarios. */
export function QuestionPractice({ question, index, isFollowUp = false }: QuestionPracticeProps) {
  const [text, setText] = useState('')
  const [usedVoice, setUsedVoice] = useState(false)
  const [showTip, setShowTip] = useState(false)
  const [showFollowUps, setShowFollowUps] = useState(false)
  const submit = useSubmitInterviewAnswer()
  const speech = useSpeechToText((spoken) => {
    setUsedVoice(true)
    setText((current) => (current ? `${current} ${spoken}` : spoken))
  })

  const send = () => {
    const inputMode: AnswerInputMode = usedVoice ? 'Voice' : 'Typed'
    submit.mutate(
      { questionId: question.id, text, inputMode },
      {
        onSuccess: () => { setText(''); setUsedVoice(false) },
        onError: (e) => toast.error(getErrorMessage(e)),
      },
    )
  }

  return (
    <div className={cn('rounded-card border border-line bg-surface p-5', isFollowUp ? 'ml-6 border-dashed' : 'shadow-card')}>
      <div className="flex flex-wrap items-center gap-2">
        <span className="text-xs font-bold text-muted">{isFollowUp ? 'FOLLOW-UP' : `QUESTION ${index + 1}`}</span>
        <Badge>{question.topic}</Badge>
      </div>
      <p className="mt-2 font-medium text-heading">{question.prompt}</p>

      <button onClick={() => setShowTip((v) => !v)} className="mt-2 flex items-center gap-1 text-xs font-semibold" aria-expanded={showTip}>
        <Lightbulb className="size-3.5" /> {showTip ? 'Hide tip' : 'Show tip'}
      </button>
      {showTip && <p className="mt-1 text-sm text-muted">{question.tip}</p>}

      <textarea
        aria-label={`Your answer to: ${question.prompt}`}
        rows={4}
        value={text}
        onChange={(e) => setText(e.target.value)}
        placeholder={question.latestAnswer ? 'Try answering again…' : 'Type your answer, or use the microphone…'}
        className={cn(controlClass(false), 'mt-3 py-2.5')}
      />

      <div className="mt-3 flex flex-wrap items-center justify-between gap-2">
        <div className="flex items-center gap-2">
          {speech.supported && (
            <Button size="sm" variant={speech.listening ? 'danger' : 'secondary'} onClick={speech.listening ? speech.stop : speech.start}>
              {speech.listening ? <><MicOff className="size-4" /> Stop</> : <><Mic className="size-4" /> Speak</>}
            </Button>
          )}
          {speech.error && <span className="text-xs text-danger">{speech.error}</span>}
        </div>
        <Button size="sm" loading={submit.isPending} disabled={!text.trim()} onClick={send}>
          <Send className="size-4" /> {question.latestAnswer ? 'Save new answer' : 'Save answer'}
        </Button>
      </div>

      {question.latestAnswer && (
        <div className="mt-4">
          <AnswerFeedbackPanel feedback={question.latestAnswer} />
          <details className="mt-2 text-sm">
            <summary className="cursor-pointer text-xs font-semibold text-muted">Your saved answer</summary>
            <p className="mt-1 whitespace-pre-line">{question.latestAnswer.text}</p>
          </details>
        </div>
      )}

      {question.followUps.length > 0 && question.latestAnswer && (
        <div className="mt-4">
          <button onClick={() => setShowFollowUps((v) => !v)} aria-expanded={showFollowUps}
            className="flex items-center gap-1 text-sm font-semibold">
            <ChevronDown className={cn('size-4 transition-transform', showFollowUps && 'rotate-180')} />
            {showFollowUps ? 'Hide follow-up scenario' : 'Try a follow-up scenario'}
          </button>
          {showFollowUps && (
            <div className="mt-3 space-y-3">
              {question.followUps.map((f, i) => <QuestionPractice key={f.id} question={f} index={i} isFollowUp />)}
            </div>
          )}
        </div>
      )}
    </div>
  )
}
