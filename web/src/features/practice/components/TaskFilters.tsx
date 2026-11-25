import { Search } from 'lucide-react'
import { controlClass } from '@/components/ui/controlClass'
import { LEARNING_STAGES, LEARNING_STAGE_META } from '@/config/learning'
import { WORK_SITUATIONS, WORK_SITUATION_LABEL } from '@/config/tasks'
import type { CareerPathSummary } from '@/features/careers'
import { cn } from '@/lib/cn'
import type { LearningStage } from '@/types/learning'
import type { WorkSituation } from '@/types/tasks'
import type { TaskLibraryFilters } from '../types'

interface TaskFiltersProps {
  value: TaskLibraryFilters
  paths: CareerPathSummary[]
  onChange: (next: TaskLibraryFilters) => void
}

const select = cn(controlClass(false), 'h-10')

export function TaskFilters({ value, paths, onChange }: TaskFiltersProps) {
  const set = (patch: TaskLibraryFilters) => onChange({ ...value, ...patch })

  return (
    <div className="grid gap-3 sm:grid-cols-2 xl:grid-cols-[2fr_1fr_1fr_1fr]">
      <label className="relative">
        <span className="sr-only">Search tasks</span>
        <Search className="pointer-events-none absolute left-3.5 top-1/2 size-4 -translate-y-1/2 text-muted" />
        <input
          type="search"
          value={value.search ?? ''}
          onChange={(e) => set({ search: e.target.value || undefined })}
          placeholder="Search by title or company"
          className={cn(select, 'pl-10')}
        />
      </label>
      <select aria-label="Job role" className={select} value={value.careerPathId ?? ''} onChange={(e) => set({ careerPathId: e.target.value ? Number(e.target.value) : undefined })}>
        <option value="">All roles</option>
        {paths.map((p) => <option key={p.id} value={p.id}>{p.name}</option>)}
      </select>
      <select aria-label="Level" className={select} value={value.level ?? ''} onChange={(e) => set({ level: (e.target.value || undefined) as LearningStage | undefined })}>
        <option value="">All levels</option>
        {LEARNING_STAGES.map((s) => <option key={s} value={s}>{LEARNING_STAGE_META[s].label}</option>)}
      </select>
      <select aria-label="Work situation" className={select} value={value.situation ?? ''} onChange={(e) => set({ situation: (e.target.value || undefined) as WorkSituation | undefined })}>
        <option value="">All situations</option>
        {WORK_SITUATIONS.map((s) => <option key={s} value={s}>{WORK_SITUATION_LABEL[s]}</option>)}
      </select>
    </div>
  )
}
