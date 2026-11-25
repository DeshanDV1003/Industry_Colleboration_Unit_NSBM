/** Saves a blob as a file in the browser (nothing is uploaded). */
export function downloadBlob(fileName: string, blob: Blob) {
  const url = URL.createObjectURL(blob)
  const link = Object.assign(document.createElement('a'), { href: url, download: fileName })
  link.click()
  URL.revokeObjectURL(url)
}

/** Saves text as a file in the browser (nothing is uploaded). */
export function downloadText(fileName: string, content: string) {
  downloadBlob(fileName, new Blob([content], { type: 'text/plain;charset=utf-8' }))
}

/** The file name a server suggested in Content-Disposition, or the fallback. */
export function fileNameFrom(contentDisposition: string | undefined, fallback: string): string {
  const match = contentDisposition?.match(/filename\*?=(?:UTF-8'')?"?([^";]+)"?/i)
  return match ? decodeURIComponent(match[1]) : fallback
}
