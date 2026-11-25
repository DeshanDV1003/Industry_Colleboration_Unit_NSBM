/** Delivery measurements from the student's own answer text. Computed in the browser and never sent anywhere. */

// Multi-word fillers first, so "you know" is counted once, not as "you" + "know".
const FILLERS = ['you know', 'i mean', 'kind of', 'sort of', 'um', 'umm', 'uh', 'erm', 'like', 'basically', 'actually', 'literally']

export const wordCount = (text: string) => (text.trim() ? text.trim().split(/\s+/).length : 0)

export function fillerCounts(text: string): Record<string, number> {
  let rest = ` ${text.toLowerCase().replace(/[^a-z' ]+/g, ' ')} `
  const counts: Record<string, number> = {}
  for (const filler of FILLERS) {
    const pattern = new RegExp(`\\s${filler}(?=\\s)`, 'g')
    const found = rest.match(pattern)?.length ?? 0
    if (found > 0) {
      counts[filler] = found
      rest = rest.replace(pattern, ' ')
    }
  }
  return counts
}

/** Words per minute, only meaningful for spoken answers. Comfortable interview pace is roughly 120–160. */
export const wordsPerMinute = (words: number, seconds: number) => (seconds < 5 ? null : Math.round((words / seconds) * 60))

export function paceAdvice(wpm: number | null): string | null {
  if (wpm === null) return null
  if (wpm < 100) return 'A little slow. Keep sentences moving.'
  if (wpm > 170) return 'Quite fast. Pause between points.'
  return 'Comfortable pace.'
}
