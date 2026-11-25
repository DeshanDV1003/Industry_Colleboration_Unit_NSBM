import { useMemo, useState } from 'react'
import { useSearchParams } from 'react-router-dom'
import { Briefcase, Info, Search } from 'lucide-react'
import { PageHeader } from '@/components/layout/PageHeader'
import { EmptyState } from '@/components/ui/EmptyState'
import { LinkButton } from '@/components/ui/LinkButton'
import { PageSpinner } from '@/components/ui/Spinner'
import { controlClass } from '@/components/ui/controlClass'
import { MATCH_DISCLAIMER, WORK_MODES, WORK_MODE_LABEL } from '@/config/opportunities'
import { useCareerPaths } from '@/features/careers'
import { MySkillsCard } from '@/features/skills-profile'
import { cn } from '@/lib/cn'
import { getErrorMessage } from '@/lib/http'
import type { WorkMode } from '@/types/opportunities'
import { useVacancySearch } from '../api/careerCenterApi'
import { VacancyMatchCard } from '../components/VacancyMatchCard'
import { VacancyMatchDialog } from '../components/VacancyMatchDialog'
import type { VacancySearch } from '../types'

const control = cn(controlClass(false), 'h-10')

/** Filters live in the URL; "role=all" clears the default (the student's chosen career path). */
function useSearchFilters(defaultPathId: number | undefined) {
  const [params, setParams] = useSearchParams()
  const filters = useMemo<VacancySearch>(() => {
    const role = params.get('role')
    return {
      careerPathId: role === 'all' ? undefined : role ? Number(role) : defaultPathId,
      workMode: (params.get('mode') as WorkMode | null) ?? undefined,
      search: params.get('q') ?? undefined,
    }
  }, [params, defaultPathId])

  const setFilters = (next: VacancySearch) => {
    const p = new URLSearchParams()
    p.set('role', next.careerPathId ? String(next.careerPathId) : 'all')
    if (next.workMode) p.set('mode', next.workMode)
    if (next.search) p.set('q', next.search)
    setParams(p, { replace: true })
  }
  return [filters, setFilters] as const
}

export default function CareerCenterPage() {
  const { data: paths, isLoading: pathsLoading } = useCareerPaths()
  const [filters, setFilters] = useSearchFilters(paths?.find((p) => p.isSelected)?.id)
  const { data: vacancies, isLoading, isError, error, isFetching } = useVacancySearch(filters)
  const [openId, setOpenId] = useState<number | null>(null)
  const set = (patch: VacancySearch) => setFilters({ ...filters, ...patch })

  if (pathsLoading || isLoading) return <PageSpinner label="Loading internships…" />
  if (isError) return <EmptyState icon={Briefcase} title="Internships could not be loaded" description={getErrorMessage(error)} />

  return (
    <>
      <PageHeader
        title="Internship match center"
        description="Fictional practice vacancies from our partners, each compared with the skills in your profile."
        actions={<LinkButton to="/applications" variant="secondary">My applications</LinkButton>}
      />

      <div className="grid gap-6 xl:grid-cols-[minmax(0,1fr)_340px]">
        <div className="space-y-5">
          <div className="grid gap-3 sm:grid-cols-[2fr_1fr_1fr]">
            <label className="relative">
              <span className="sr-only">Search internships</span>
              <Search className="pointer-events-none absolute left-3.5 top-1/2 size-4 -translate-y-1/2 text-muted" />
              <input type="search" value={filters.search ?? ''} onChange={(e) => set({ search: e.target.value || undefined })}
                placeholder="Search by title, company or location" className={cn(control, 'pl-10')} />
            </label>
            <select aria-label="Job role" className={control} value={filters.careerPathId ?? ''}
              onChange={(e) => set({ careerPathId: e.target.value ? Number(e.target.value) : undefined })}>
              <option value="">All roles</option>
              {paths?.map((p) => <option key={p.id} value={p.id}>{p.name}</option>)}
            </select>
            <select aria-label="Work mode" className={control} value={filters.workMode ?? ''}
              onChange={(e) => set({ workMode: (e.target.value || undefined) as WorkMode | undefined })}>
              <option value="">Any work mode</option>
              {WORK_MODES.map((m) => <option key={m} value={m}>{WORK_MODE_LABEL[m]}</option>)}
            </select>
          </div>

          <p className="flex gap-2 text-xs text-muted"><Info className="mt-0.5 size-3.5 shrink-0" /> {MATCH_DISCLAIMER}</p>

          {vacancies?.length === 0 ? (
            <EmptyState icon={Briefcase} title="No open internships match these filters" description="Try another role or work mode." />
          ) : (
            <div className={cn('grid gap-4 lg:grid-cols-2', isFetching && 'opacity-70')}>
              {vacancies?.map((v) => <VacancyMatchCard key={v.id} vacancy={v} onOpen={() => setOpenId(v.id)} />)}
            </div>
          )}
        </div>

        <aside><MySkillsCard /></aside>
      </div>

      <VacancyMatchDialog vacancy={vacancies?.find((v) => v.id === openId) ?? null} onClose={() => setOpenId(null)} />
    </>
  )
}
