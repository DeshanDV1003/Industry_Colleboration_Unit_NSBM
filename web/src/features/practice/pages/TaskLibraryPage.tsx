import { useMemo } from 'react'
import { useSearchParams } from 'react-router-dom'
import { Wrench } from 'lucide-react'
import { PageHeader } from '@/components/layout/PageHeader'
import { EmptyState } from '@/components/ui/EmptyState'
import { PageSpinner } from '@/components/ui/Spinner'
import { useCareerPaths } from '@/features/careers'
import { getErrorMessage } from '@/lib/http'
import type { LearningStage } from '@/types/learning'
import type { WorkSituation } from '@/types/tasks'
import { useTaskLibrary } from '../api/practiceApi'
import { TaskFilters } from '../components/TaskFilters'
import { TaskLibraryCard } from '../components/TaskLibraryCard'
import type { TaskLibraryFilters } from '../types'

/** Filters live in the URL so a filtered view can be bookmarked or shared. "role=all" clears the default role. */
function useLibraryFilters(defaultPathId: number | undefined) {
  const [params, setParams] = useSearchParams()

  const filters = useMemo<TaskLibraryFilters>(() => {
    const role = params.get('role')
    return {
      careerPathId: role === 'all' ? undefined : role ? Number(role) : defaultPathId,
      level: (params.get('level') as LearningStage | null) ?? undefined,
      situation: (params.get('situation') as WorkSituation | null) ?? undefined,
      search: params.get('q') ?? undefined,
    }
  }, [params, defaultPathId])

  const setFilters = (next: TaskLibraryFilters) => {
    const p = new URLSearchParams()
    p.set('role', next.careerPathId ? String(next.careerPathId) : 'all')
    if (next.level) p.set('level', next.level)
    if (next.situation) p.set('situation', next.situation)
    if (next.search) p.set('q', next.search)
    setParams(p, { replace: true })
  }

  return [filters, setFilters] as const
}

export default function TaskLibraryPage() {
  const { data: paths, isLoading: pathsLoading } = useCareerPaths()
  const chosenPathId = paths?.find((p) => p.isSelected)?.id
  const [filters, setFilters] = useLibraryFilters(chosenPathId)
  const { data: tasks, isLoading, isError, error, isFetching } = useTaskLibrary(filters)

  if (pathsLoading || isLoading) return <PageSpinner label="Loading industry tasks…" />
  if (isError) return <EmptyState icon={Wrench} title="Tasks could not be loaded" description={getErrorMessage(error)} />

  return (
    <>
      <PageHeader
        title="Industry tasks"
        description="Company-style tasks set by our industry partners. Pick one, work on it in the workbench, then explain what you did."
      />

      <div className="mb-5">
        <TaskFilters value={filters} paths={paths ?? []} onChange={setFilters} />
      </div>

      {tasks?.length === 0 ? (
        <EmptyState icon={Wrench} title="No tasks match these filters" description="Try another role, level or situation." />
      ) : (
        <div className={`grid gap-4 md:grid-cols-2 2xl:grid-cols-3 ${isFetching ? 'opacity-70' : ''}`}>
          {tasks?.map((task) => <TaskLibraryCard key={task.id} task={task} />)}
        </div>
      )}
    </>
  )
}
