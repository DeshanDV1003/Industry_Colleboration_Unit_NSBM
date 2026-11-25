import { useState } from 'react'
import { ArrowRight, Compass } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { PageHeader } from '@/components/layout/PageHeader'
import { EmptyState } from '@/components/ui/EmptyState'
import { LinkButton } from '@/components/ui/LinkButton'
import { PageSpinner } from '@/components/ui/Spinner'
import { getErrorMessage } from '@/lib/http'
import { useCareerPaths, useSelectCareerPath } from '../api/careersApi'
import { CareerPathCard } from '../components/CareerPathCard'
import { CareerPathDetailDialog } from '../components/CareerPathDetailDialog'

export default function CareerPathsPage() {
  const { data: paths, isLoading, isError, error } = useCareerPaths()
  const selectPath = useSelectCareerPath()
  const [detailSlug, setDetailSlug] = useState<string | null>(null)

  const choose = (id: number) =>
    selectPath.mutate(id, {
      onSuccess: () => {
        setDetailSlug(null)
        toast.success('Career path saved. Your learning pages now follow this path.')
      },
      onError: (e) => toast.error(getErrorMessage(e)),
    })

  if (isLoading) return <PageSpinner label="Loading career paths…" />
  if (isError) return <EmptyState icon={Compass} title="Career paths could not be loaded" description={getErrorMessage(error)} />

  const hasSelection = paths?.some((p) => p.isSelected)

  return (
    <>
      <PageHeader
        title="Explore career roles"
        description="Read about each entry-level IT role, then pick the one you want to prepare for. You can change your choice at any time."
        actions={hasSelection && (
          <LinkButton to="/baseline">
            Next: skills baseline <ArrowRight className="size-4" />
          </LinkButton>
        )}
      />

      <div className="grid gap-5 md:grid-cols-2 2xl:grid-cols-4">
        {paths?.map((path) => (
          <CareerPathCard
            key={path.id}
            path={path}
            selecting={selectPath.isPending && selectPath.variables === path.id}
            onDetails={() => setDetailSlug(path.slug)}
            onSelect={() => choose(path.id)}
          />
        ))}
      </div>

      <CareerPathDetailDialog
        slug={detailSlug}
        selecting={selectPath.isPending}
        onClose={() => setDetailSlug(null)}
        onSelect={choose}
      />
    </>
  )
}
