import { Badge } from '@/components/ui/Badge'
import { Button } from '@/components/ui/Button'
import { Dialog } from '@/components/ui/Dialog'
import { Spinner } from '@/components/ui/Spinner'
import { useCareerPath } from '../api/careersApi'
import type { CareerSkill } from '../types'

interface CareerPathDetailDialogProps {
  slug: string | null
  selecting: boolean
  onClose: () => void
  onSelect: (id: number) => void
}

function SkillGroup({ title, skills }: { title: string; skills: CareerSkill[] }) {
  if (skills.length === 0) return null
  return (
    <section className="mt-6">
      <h3 className="text-sm font-bold uppercase tracking-wider text-muted">{title}</h3>
      <ul className="mt-3 grid gap-2 sm:grid-cols-2">
        {skills.map((skill) => (
          <li key={skill.skillId} className="rounded-xl border border-line p-3">
            <div className="flex items-center justify-between gap-2">
              <span className="font-medium text-heading">{skill.name}</span>
              <Badge className="text-[10px]">{skill.category}</Badge>
            </div>
            {skill.description && <p className="mt-1 text-xs text-muted">{skill.description}</p>}
          </li>
        ))}
      </ul>
    </section>
  )
}

export function CareerPathDetailDialog({ slug, selecting, onClose, onSelect }: CareerPathDetailDialogProps) {
  const { data: path, isLoading } = useCareerPath(slug)

  return (
    <Dialog
      open={!!slug}
      onClose={onClose}
      title={path?.name ?? 'Career path'}
      description={path?.tagline}
      footer={
        path && (
          <>
            <Button variant="secondary" onClick={onClose}>Close</Button>
            {!path.isSelected && (
              <Button loading={selecting} onClick={() => onSelect(path.id)}>Choose this path</Button>
            )}
          </>
        )
      }
    >
      {isLoading || !path ? (
        <Spinner className="py-10" />
      ) : (
        <>
          <p className="leading-relaxed">{path.description}</p>
          <div className="mt-4 flex flex-wrap gap-1.5">
            {path.entryRoles.map((role) => <Badge key={role} tone="accent">{role}</Badge>)}
          </div>
          <SkillGroup title="Core skills" skills={path.skills.filter((s) => s.importance === 'Core')} />
          <SkillGroup title="Supporting skills" skills={path.skills.filter((s) => s.importance === 'Supporting')} />
        </>
      )}
    </Dialog>
  )
}
