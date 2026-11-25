import type { Cv } from './api/cvApi'

/** Plain-text CV outline for copying or saving. Empty sections are left out. */
export function buildCvText(cv: Cv): string {
  const section = (title: string, body: string | null | undefined) => (body?.trim() ? ['', title.toUpperCase(), body.trim()] : [])
  const links = [cv.gitHubUrl && `GitHub: ${cv.gitHubUrl}`, cv.linkedInUrl && `LinkedIn: ${cv.linkedInUrl}`].filter(Boolean).join('\n')

  return [
    cv.fullName,
    cv.headline,
    [cv.email, links].filter(Boolean).join('\n'),
    ...section('Summary', cv.summary),
    ...section('Education', cv.education),
    ...section('Skills', cv.skills.join(', ')),
    ...section('Projects', cv.projects),
  ].filter((line) => line !== undefined).join('\n')
}
