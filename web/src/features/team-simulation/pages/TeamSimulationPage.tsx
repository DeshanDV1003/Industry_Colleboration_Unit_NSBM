import { FolderKanban, Info } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { PageHeader } from '@/components/layout/PageHeader'
import { Badge } from '@/components/ui/Badge'
import { Button } from '@/components/ui/Button'
import { Card } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { PageSpinner } from '@/components/ui/Spinner'
import { controlClass } from '@/components/ui/controlClass'
import { cn } from '@/lib/cn'
import { getErrorMessage } from '@/lib/http'
import { useMoveSprintItem, useSprintBoard } from '../api/sprintApi'
import type { SprintItem, SprintStatus } from '../types'

const COLUMNS: { status: SprintStatus; label: string }[] = [
  { status: 'ToDo', label: 'To do' },
  { status: 'InProgress', label: 'In progress' },
  { status: 'InReview', label: 'In review' },
  { status: 'Done', label: 'Done' },
]

function SprintCard({ item }: { item: SprintItem }) {
  const move = useMoveSprintItem()
  const go = (status: SprintStatus) => move.mutate({ id: item.id, status }, { onError: (e) => toast.error(getErrorMessage(e)) })

  return (
    <Card className={cn('p-3', item.isMine ? 'border-primary ring-2 ring-primary-soft' : item.canPickUp ? '' : 'opacity-80')}>
      <div className="flex items-center justify-between gap-2">
        <span className="font-mono text-xs text-muted">{item.key}</span>
        <Badge className="text-[10px]">{item.storyPoints} pts</Badge>
      </div>
      <p className="mt-1 text-sm font-semibold text-heading">{item.title}</p>
      <p className="mt-1 text-xs text-muted">{item.description}</p>
      <div className="mt-2 flex items-center justify-between gap-2">
        <Badge tone={item.isMine ? 'primary' : 'neutral'} className="text-[10px]">{item.owner}</Badge>
        <span className="text-[10px] text-muted">{item.roleName}</span>
      </div>
      {item.isMine && (
        <select aria-label={`Move ${item.key}`} value={item.status} disabled={move.isPending} onChange={(e) => go(e.target.value as SprintStatus)}
          className={cn(controlClass(false), 'mt-2 h-8 text-xs')}>
          {COLUMNS.map((c) => <option key={c.status} value={c.status}>{c.label}</option>)}
        </select>
      )}
      {item.canPickUp && (
        <Button size="sm" variant="secondary" className="mt-2 w-full" loading={move.isPending} onClick={() => go('InProgress')}>Pick up</Button>
      )}
    </Card>
  )
}

/** The virtual IT company: a shared practice sprint with Software, QA, DevOps and Cloud work items. */
export default function TeamSimulationPage() {
  const { data: board, isLoading, isError, error } = useSprintBoard()

  if (isLoading) return <PageSpinner label="Loading the sprint board…" />
  if (isError || !board) return <EmptyState icon={FolderKanban} title="Board could not be loaded" description={getErrorMessage(error)} />

  return (
    <>
      <PageHeader
        title="Virtual IT company · Practice sprint"
        description="An order-tracking release built by a cross-functional team. Pick up the cards for your role and move them across the board."
      />
      <p className="mb-4 flex gap-2 text-sm text-muted">
        <Info className="mt-0.5 size-4 shrink-0" />
        {board.myRoleName
          ? <>You are the <strong className="text-heading">{board.myRoleName}</strong> team member. Teammates are fictional and their cards move on their own schedule.</>
          : 'Choose a career path to join the sprint as a team member.'}
      </p>
      <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
        {COLUMNS.map((column) => {
          const items = board.items.filter((i) => i.status === column.status)
          return (
            <section key={column.status} className="rounded-card bg-surface-subtle p-3">
              <h2 className="mb-3 flex items-center justify-between px-1 text-sm font-semibold">
                {column.label} <span className="text-xs text-muted">{items.length}</span>
              </h2>
              <div className="space-y-3">{items.map((item) => <SprintCard key={item.id} item={item} />)}</div>
            </section>
          )
        })}
      </div>
    </>
  )
}
