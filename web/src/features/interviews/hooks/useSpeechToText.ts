import { useCallback, useEffect, useRef, useState } from 'react'

/** Minimal typing for the browser Web Speech API (not in the standard DOM lib). */
interface SpeechRecognitionResultLike { 0: { transcript: string }; isFinal: boolean }
interface SpeechRecognitionEventLike { resultIndex: number; results: ArrayLike<SpeechRecognitionResultLike> }
interface SpeechRecognitionLike {
  lang: string
  continuous: boolean
  interimResults: boolean
  onresult: ((event: SpeechRecognitionEventLike) => void) | null
  onend: (() => void) | null
  onerror: ((event: { error: string }) => void) | null
  start: () => void
  stop: () => void
}
type SpeechRecognitionCtor = new () => SpeechRecognitionLike

const getRecognition = (): SpeechRecognitionCtor | undefined => {
  const w = window as unknown as { SpeechRecognition?: SpeechRecognitionCtor; webkitSpeechRecognition?: SpeechRecognitionCtor }
  return w.SpeechRecognition ?? w.webkitSpeechRecognition
}

/**
 * Optional voice input using the browser's speech service. The browser may send audio to its maker's servers;
 * the platform never stores audio, only the text the student chooses to submit.
 */
export function useSpeechToText(onFinalText: (text: string) => void) {
  const supported = typeof window !== 'undefined' && !!getRecognition()
  const [listening, setListening] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const recognitionRef = useRef<SpeechRecognitionLike | null>(null)
  const callbackRef = useRef(onFinalText)

  useEffect(() => { callbackRef.current = onFinalText }, [onFinalText])
  useEffect(() => () => recognitionRef.current?.stop(), [])

  const start = useCallback(() => {
    const Recognition = getRecognition()
    if (!Recognition) return
    const recognition = new Recognition()
    recognition.lang = 'en-GB'
    recognition.continuous = true
    recognition.interimResults = false
    recognition.onresult = (event) => {
      for (let i = event.resultIndex; i < event.results.length; i++) {
        if (event.results[i].isFinal) callbackRef.current(event.results[i][0].transcript.trim())
      }
    }
    recognition.onerror = (event) => setError(event.error === 'not-allowed' ? 'Microphone permission was denied.' : 'Speech recognition stopped. You can keep typing.')
    recognition.onend = () => setListening(false)
    recognitionRef.current = recognition
    setError(null)
    recognition.start()
    setListening(true)
  }, [])

  const stop = useCallback(() => recognitionRef.current?.stop(), [])

  return { supported, listening, error, start, stop }
}
