import { Check } from 'lucide-react'
import { Spinner } from '@/components/ui/Spinner'
import { useSkills } from '../api/careersApi'
import { cn } from '@/lib/cn'
import type { SkillCategory } from '@/types/learning'

interface SkillPickerProps {
  value: number[]
  onChange: (ids: number[]) => void
}

const CATEGORY_ORDER: SkillCategory[] = ['Programming', 'Testing', 'Infrastructure', 'Professional']

export function SkillPicker({ value, onChange }: SkillPickerProps) {
  const { data: skills, isLoading } = useSkills()
  if (isLoading || !skills) return <Spinner className="py-4" />

  const toggle = (id: number) => onChange(value.includes(id) ? value.filter((v) => v !== id) : [...value, id])

  return (
    <div className="space-y-3">
      {CATEGORY_ORDER.map((category) => (
        <div key={category}>
          <p className="mb-1.5 text-xs font-semibold text-muted">{category}</p>
          <div className="flex flex-wrap gap-1.5">
            {skills.filter((s) => s.category === category).map((skill) => {
              const selected = value.includes(skill.id)
              return (
                <button
                  key={skill.id}
                  type="button"
                  aria-pressed={selected}
                  onClick={() => toggle(skill.id)}
                  className={cn(
                    'inline-flex items-center gap-1 rounded-full border px-3 py-1 text-xs font-medium transition-colors',
                    selected ? 'border-primary bg-primary-soft text-primary' : 'border-line text-body hover:border-line-strong',
                  )}
                >
                  {selected && <Check className="size-3.5" />}
                  {skill.name}
                </button>
              )
            })}
          </div>
        </div>
      ))}
    </div>
  )
}
