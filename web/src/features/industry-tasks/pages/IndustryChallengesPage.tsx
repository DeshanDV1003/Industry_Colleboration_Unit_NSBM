import { useState } from 'react'
import { Link } from 'react-router-dom'
import { Pencil, Plus, Users, Wrench } from 'lucide-react'
import { PageHeader } from '@/components/layout/PageHeader'
import { Badge } from '@/components/ui/Badge'
import { Button } from '@/components/ui/Button'
import { Card } from '@/components/ui/Card'
import { Dialog } from '@/components/ui/Dialog'
import { EmptyState } from '@/components/ui/EmptyState'
import { LinkButton } from '@/components/ui/LinkButton'
import { PageSpinner, Spinner } from '@/components/ui/Spinner'
import { LEARNING_STAGE_META } from '@/config/learning'
import { WORK_SITUATION_LABEL } from '@/config/tasks'
import { formatDate } from '@/lib/format'
import { getErrorMessage } from '@/lib/http'
import { useCompanyTasks, useIndustryTask } from '../api/industryTasksApi'
import { TaskDetailView } from '../components/TaskDetailView'
import { ModerationStatusBadge } from '@/components/ModerationStatusBadge'

function TaskPreviewDialog({ taskId, onClose }: { taskId: number | null; onClose: () => void }) {
  const { data: task } = useIndustryTask(taskId)
  return (
    <Dialog
      open={taskId !== null}
      onClose={onClose}
      title={task?.title ?? 'Task'}
      description={task && <ModerationStatusBadge status={task.status} />}
      footer={task?.canBeEdited && <LinkButton to={`/industry/tasks/${task.id}/edit`}><Pencil className="size-4" /> Edit</LinkButton>}
    >
      {task ? <TaskDetailView task={task} /> : <Spinner className="py-10" />}
    </Dialog>
  )
}

export default function IndustryChallengesPage() {
  const { data: tasks, isLoading, isError, error } = useCompanyTasks()
  const [previewId, setPreviewId] = useState<number | null>(null)

  if (isLoading) return <PageSpinner label="Loading your challenges…" />
  if (isError) return <EmptyState icon={Wrench} title="Challenges could not be loaded" description={getErrorMessage(error)} />

  const newButton = <LinkButton to="/industry/tasks/new"><Plus className="size-4" /> New challenge</LinkButton>

  return (
    <>
      <PageHeader
        title="Industry challenges"
        description="Company-style tasks your team has published for students. New and edited tasks wait for Unit approval."
        actions={newButton}
      />

      {tasks?.length === 0 ? (
        <EmptyState icon={Wrench} title="No challenges yet" description="Show students what entry-level work at your company looks like." action={newButton} />
      ) : (
        <Card className="overflow-hidden">
          <table className="w-full text-sm">
            <thead className="border-b border-line bg-surface-subtle text-left text-xs font-semibold uppercase tracking-wider text-muted">
              <tr>
                <th className="px-5 py-3">Challenge</th>
                <th className="hidden px-5 py-3 md:table-cell">Role</th>
                <th className="px-5 py-3">Status</th>
                <th className="hidden px-5 py-3 lg:table-cell">Attempts</th>
                <th className="px-5 py-3"><span className="sr-only">Actions</span></th>
              </tr>
            </thead>
            <tbody className="divide-y divide-line">
              {tasks?.map((task) => (
                <tr key={task.id} className="align-top">
                  <td className="px-5 py-4">
                    <button onClick={() => setPreviewId(task.id)} className="text-left font-semibold text-heading hover:text-primary">{task.title}</button>
                    <div className="mt-1 flex flex-wrap gap-1.5">
                      <Badge tone={LEARNING_STAGE_META[task.level].tone}>{LEARNING_STAGE_META[task.level].label}</Badge>
                      <Badge>{WORK_SITUATION_LABEL[task.situation]}</Badge>
                    </div>
                    {task.status === 'Rejected' && task.reviewNote && (
                      <p className="mt-2 text-xs text-danger">Reviewer: {task.reviewNote}</p>
                    )}
                  </td>
                  <td className="hidden px-5 py-4 md:table-cell">{task.careerPathName}</td>
                  <td className="px-5 py-4">
                    <ModerationStatusBadge status={task.status} />
                    <p className="mt-1 text-xs text-muted">{formatDate(task.reviewedAt ?? task.createdAt)}</p>
                  </td>
                  <td className="hidden px-5 py-4 lg:table-cell">
                    <span className="flex items-center gap-1.5 text-muted"><Users className="size-4" /> {task.attemptCount}</span>
                  </td>
                  <td className="px-5 py-4 text-right">
                    {task.status === 'Approved'
                      ? <Button variant="ghost" size="sm" onClick={() => setPreviewId(task.id)}>View</Button>
                      : <Link to={`/industry/tasks/${task.id}/edit`} className="text-sm font-semibold">Edit</Link>}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </Card>
      )}

      <TaskPreviewDialog taskId={previewId} onClose={() => setPreviewId(null)} />
    </>
  )
}
