import { useState } from 'react'
import { Plus, ScanSearch } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { Badge } from '@/components/ui/Badge'
import { Button } from '@/components/ui/Button'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { controlClass } from '@/components/ui/controlClass'
import { useMySkillProfile, useUpdateMySkills } from '@/features/skills-profile'
import { cn } from '@/lib/cn'
import { getErrorMessage } from '@/lib/http'
import { useFindCvKeywords } from '../api/cvApi'

/** Paste text from an old CV to see which known skills it mentions. Not a skill check and not an ATS score. */
export function CvKeywordFinder() {
  const [text, setText] = useState('')
  const find = useFindCvKeywords()
  const { data: profile } = useMySkillProfile()
  const updateSkills = useUpdateMySkills()

  const matches = find.data ?? []
  const listed = new Set(profile?.skillIds ?? [])
  const newOnes = matches.filter((m) => !listed.has(m.skillId))

  const addAll = () =>
    updateSkills.mutate([...listed, ...newOnes.map((m) => m.skillId)], {
      onSuccess: () => toast.success(`${newOnes.length} skill${newOnes.length === 1 ? '' : 's'} added to your profile.`),
      onError: (e) => toast.error(getErrorMessage(e)),
    })

  return (
    <Card>
      <CardHeader title="CV keyword finder" description="Paste plain text from an old CV. We highlight known skills it mentions." />
      <CardBody className="space-y-3">
        <textarea aria-label="CV text" rows={5} value={text} onChange={(e) => setText(e.target.value)}
          placeholder="Paste your CV text here…" className={cn(controlClass(false), 'py-2.5')} />
        <Button variant="secondary" size="sm" loading={find.isPending} disabled={!text.trim()}
          onClick={() => find.mutate(text, { onError: (e) => toast.error(getErrorMessage(e)) })}>
          <ScanSearch className="size-4" /> Find skills
        </Button>

        {find.isSuccess && (
          matches.length === 0 ? <p className="text-sm text-muted">No known skills found in this text.</p> : (
            <>
              <div className="flex flex-wrap gap-1.5">
                {matches.map((m) => <Badge key={m.skillId} tone={listed.has(m.skillId) ? 'success' : 'primary'}>{m.name}{listed.has(m.skillId) && ' ✓'}</Badge>)}
              </div>
              {newOnes.length > 0 && (
                <Button size="sm" loading={updateSkills.isPending} onClick={addAll}>
                  <Plus className="size-4" /> Add {newOnes.length} to my skills
                </Button>
              )}
            </>
          )
        )}
        <p className="text-xs text-muted">Finding a word does not check your skill level, and this is not an ATS score.</p>
      </CardBody>
    </Card>
  )
}
