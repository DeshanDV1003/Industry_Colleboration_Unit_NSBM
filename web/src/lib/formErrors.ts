import { isAxiosError } from 'axios'
import type { FieldValues, Path, UseFormSetError } from 'react-hook-form'

/**
 * Copies API validation errors onto react-hook-form fields.
 * "Task.Checks[0].Pattern" becomes "checks.0.pattern" once the `stripPrefix` ("Task.") is removed.
 * Returns true when at least one field error was applied.
 */
export function applyServerErrors<T extends FieldValues>(error: unknown, setError: UseFormSetError<T>, stripPrefix = ''): boolean {
  if (!isAxiosError<{ errors?: Record<string, string[]> }>(error)) return false
  const errors = error.response?.data?.errors
  if (!errors) return false

  for (const [key, messages] of Object.entries(errors)) {
    const path = key
      .replace(stripPrefix, '')
      .replace(/\[(\d+)\]/g, '.$1')
      .split('.')
      .map((part) => (/^\d+$/.test(part) ? part : part.charAt(0).toLowerCase() + part.slice(1)))
      .join('.')
    setError(path as Path<T>, { type: 'server', message: messages[0] })
  }
  return true
}
