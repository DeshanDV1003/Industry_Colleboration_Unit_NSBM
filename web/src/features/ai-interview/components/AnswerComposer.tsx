import { useRef, useState } from 'react'
import { Mic, MicOff, Send } from 'lucide-react'
import { Badge } from '@/components/ui/Badge'
import { Button } from '@/components/ui/Button'
import { controlClass } from '@/components/ui/controlClass'
import { useSpeechToText } from '@/features/interviews'
import { cn } from '@/lib/cn'
import { fillerCounts, paceAdvice, wordCount, wordsPerMinute } from '../coaching/speechMetrics'

const MAX_LENGTH = 4000

interface AnswerComposerProps {
  sending: boolean
  onSend: (answer: string) => void
}

/** Type or dictate an answer. Live word count and filler-word hints are worked out in the browser only. */
export function AnswerComposer({ sending, onSend }: AnswerComposerProps) {
  const [text, setText] = useState('')
  const spokenSecondsRef = useRef(0)
  const listenStartRef = useRef<number | null>(null)
  const [spokenSeconds, setSpokenSeconds] = useState(0)
  const speech = useSpeechToText((chunk) => setText((t) => (t ? `${t} ${chunk}` : chunk)))

  const toggleMic = () => {
    if (speech.listening) {
      speech.stop()
      if (listenStartRef.current !== null) {
        spokenSecondsRef.current += (Date.now() - listenStartRef.current) / 1000
        setSpokenSeconds(spokenSecondsRef.current)
      }
      listenStartRef.current = null
    } else {
      listenStartRef.current = Date.now()
      speech.start()
    }
  }

  const words = wordCount(text)
  const fillers = Object.entries(fillerCounts(text))
  const wpm = wordsPerMinute(words, spokenSeconds)

  const send = () => {
    if (speech.listening) speech.stop()
    onSend(text.trim())
    setText('')
    spokenSecondsRef.current = 0
    setSpokenSeconds(0)
  }

  return (
    <form className="space-y-2" onSubmit={(e) => { e.preventDefault(); if (text.trim()) send() }}>
      <textarea
        aria-label="Your answer"
        rows={5}
        maxLength={MAX_LENGTH}
        value={text}
        onChange={(e) => setText(e.target.value)}
        onKeyDown={(e) => { if (e.key === 'Enter' && (e.ctrlKey || e.metaKey) && text.trim()) { e.preventDefault(); send() } }}
        placeholder="Answer as you would in a real interview. Use an example: the situation, what you did and the result."
        className={cn(controlClass(false), 'py-2.5')}
      />
      <div className="flex flex-wrap items-center gap-2 text-xs text-muted">
        <span>{words} words</span>
        {wpm !== null && <Badge tone="accent">{wpm} words/min · {paceAdvice(wpm)}</Badge>}
        {fillers.length > 0 && <Badge tone="warning">Fillers: {fillers.map(([f, n]) => `${f} ×${n}`).join(', ')}</Badge>}
        <span className="ml-auto flex gap-2">
          {speech.supported && (
            <Button type="button" size="sm" variant="secondary" onClick={toggleMic} aria-pressed={speech.listening}>
              {speech.listening ? <><MicOff className="size-4" /> Stop</> : <><Mic className="size-4" /> Speak</>}
            </Button>
          )}
          <Button type="submit" size="sm" loading={sending} disabled={!text.trim()}><Send className="size-4" /> Send answer</Button>
        </span>
      </div>
      {speech.error && <p className="text-xs text-danger">{speech.error}</p>}
      {speech.supported && <p className="text-[11px] text-muted">Speaking uses your browser's speech service, which may process audio on its maker's servers. Only the text you send is shared with the AI interviewer.</p>}
    </form>
  )
}
