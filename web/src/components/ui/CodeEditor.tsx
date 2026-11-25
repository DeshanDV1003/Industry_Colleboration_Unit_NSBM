import { useRef, type KeyboardEvent } from 'react'
import { cn } from '@/lib/cn'

interface CodeEditorProps {
  value: string
  onChange: (value: string) => void
  language?: string | null
  minRows?: number
  readOnly?: boolean
  ariaLabel: string
  className?: string
}

const INDENT = '  '

/** A lightweight monospace editor with line numbers and Tab-to-indent. It edits text only; nothing is executed. */
export function CodeEditor({ value, onChange, language, minRows = 14, readOnly, ariaLabel, className }: CodeEditorProps) {
  const gutterRef = useRef<HTMLDivElement>(null)
  const lineCount = Math.max(minRows, value.split('\n').length)

  const handleKeyDown = (e: KeyboardEvent<HTMLTextAreaElement>) => {
    if (e.key !== 'Tab' || readOnly) return
    e.preventDefault()
    const textarea = e.currentTarget
    const { selectionStart, selectionEnd } = textarea
    const next = value.slice(0, selectionStart) + INDENT + value.slice(selectionEnd)
    onChange(next)
    const caret = selectionStart + INDENT.length
    requestAnimationFrame(() => textarea.setSelectionRange(caret, caret))
  }

  return (
    <div className={cn('overflow-hidden rounded-xl border border-line-strong bg-code text-code-text', className)}>
      {language && (
        <div className="border-b border-code-line px-4 py-2 text-[11px] font-semibold uppercase tracking-wider text-code-muted">{language}</div>
      )}
      <div className="flex font-mono text-[13px] leading-6">
        <div ref={gutterRef} aria-hidden className="select-none overflow-hidden border-r border-code-line px-3 py-3 text-right text-code-muted">
          {Array.from({ length: lineCount }, (_, i) => <div key={i}>{i + 1}</div>)}
        </div>
        <textarea
          aria-label={ariaLabel}
          value={value}
          readOnly={readOnly}
          spellCheck={false}
          autoCapitalize="off"
          autoCorrect="off"
          rows={lineCount}
          onChange={(e) => onChange(e.target.value)}
          onKeyDown={handleKeyDown}
          onScroll={(e) => gutterRef.current && (gutterRef.current.scrollTop = e.currentTarget.scrollTop)}
          className="min-w-0 flex-1 resize-y whitespace-pre bg-transparent px-4 py-3 outline-none"
        />
      </div>
    </div>
  )
}
