import { useState } from 'react'
import { CalendarCheck, Plus, Trash2 } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { PageHeader } from '@/components/layout/PageHeader'
import { Badge, type BadgeTone } from '@/components/ui/Badge'
import { Button } from '@/components/ui/Button'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { PageSpinner } from '@/components/ui/Spinner'
import { controlClass } from '@/components/ui/controlClass'
import { cn } from '@/lib/cn'
import { formatDate } from '@/lib/format'
import { getErrorMessage } from '@/lib/http'
import { useAddPlannerItem, useDeletePlannerItem, usePlanner, useSetPlannerItemDone, type PlannerItemKind } from '../api/plannerApi'

const KINDS: { value: PlannerItemKind; tone: BadgeTone }[] = [
  { value: 'Goal', tone: 'primary' },
  { value: 'Task', tone: 'accent' },
  { value: 'Deadline', tone: 'warning' },
]
const toneOf = (kind: PlannerItemKind) => KINDS.find((k) => k.value === kind)!.tone
const today = () => new Date().toISOString().slice(0, 10)

function AddItemForm() {
  const add = useAddPlannerItem()
  const [kind, setKind] = useState<PlannerItemKind>('Task')
  const [title, setTitle] = useState('')
  const [dueDate, setDueDate] = useState('')
  const control = cn(controlClass(false), 'h-10')

  const submit = () =>
    add.mutate({ kind, title, notes: null, dueDate: dueDate || null }, {
      onSuccess: () => { setTitle(''); setDueDate('') },
      onError: (e) => toast.error(getErrorMessage(e)),
    })

  return (
    <form className="grid gap-3 md:grid-cols-[140px_minmax(0,1fr)_170px_auto]" onSubmit={(e) => { e.preventDefault(); submit() }}>
      <select aria-label="Kind" className={control} value={kind} onChange={(e) => setKind(e.target.value as PlannerItemKind)}>
        {KINDS.map((k) => <option key={k.value} value={k.value}>{k.value}</option>)}
      </select>
      <input aria-label="Title" className={control} value={title} onChange={(e) => setTitle(e.target.value)} placeholder="e.g. Practise API testing with Postman" />
      <input aria-label="Due date" type="date" className={control} value={dueDate} onChange={(e) => setDueDate(e.target.value)} />
      <Button type="submit" loading={add.isPending} disabled={!title.trim() || (kind === 'Deadline' && !dueDate)}><Plus className="size-4" /> Add</Button>
    </form>
  )
}

export default function PlannerPage() {
  const { data: items, isLoading, isError, error } = usePlanner()
  const setDone = useSetPlannerItemDone()
  const remove = useDeletePlannerItem()

  if (isLoading) return <PageSpinner label="Loading your planner…" />
  if (isError || !items) return <EmptyState icon={CalendarCheck} title="Planner could not be loaded" description={getErrorMessage(error)} />

  const open = items.filter((i) => !i.isDone)
  const done = items.filter((i) => i.isDone)

  const row = (item: (typeof items)[number]) => {
    const overdue = !item.isDone && item.dueDate !== null && item.dueDate < today()
    return (
      <li key={item.id} className="flex items-center gap-3 py-3">
        <input type="checkbox" aria-label={`Mark "${item.title}" as ${item.isDone ? 'not done' : 'done'}`} checked={item.isDone}
          onChange={() => setDone.mutate({ id: item.id, done: !item.isDone })} className="size-4 accent-[var(--color-primary)]" />
        <div className="min-w-0 flex-1">
          <p className={cn('font-medium text-heading', item.isDone && 'text-muted line-through')}>{item.title}</p>
          {item.notes && <p className="text-xs text-muted">{item.notes}</p>}
        </div>
        <Badge tone={toneOf(item.kind)}>{item.kind}</Badge>
        {item.dueDate && <span className={cn('w-28 text-right text-xs', overdue ? 'font-semibold text-danger' : 'text-muted')}>{overdue ? 'Overdue · ' : ''}{formatDate(item.dueDate)}</span>}
        <button onClick={() => remove.mutate(item.id)} aria-label={`Delete "${item.title}"`} className="text-muted hover:text-danger"><Trash2 className="size-4" /></button>
      </li>
    )
  }

  return (
    <>
      <PageHeader title="Preparation planner" description="Goals, tasks and deadlines from your skill gaps and interview practice." />
      <Card className="mb-6"><CardBody><AddItemForm /></CardBody></Card>
      <div className="grid gap-6 lg:grid-cols-2">
        <Card>
          <CardHeader title={`Upcoming (${open.length})`} />
          <CardBody>{open.length === 0 ? <p className="text-sm text-muted">Nothing planned. Add a goal from a skill gap.</p> : <ul className="divide-y divide-line">{open.map(row)}</ul>}</CardBody>
        </Card>
        <Card>
          <CardHeader title={`Done (${done.length})`} />
          <CardBody>{done.length === 0 ? <p className="text-sm text-muted">Completed items appear here.</p> : <ul className="divide-y divide-line">{done.map(row)}</ul>}</CardBody>
        </Card>
      </div>
    </>
  )
}
