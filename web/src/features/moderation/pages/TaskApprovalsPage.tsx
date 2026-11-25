import { useState } from 'react'
import { ShieldCheck } from 'lucide-react'
import { PageHeader } from '@/components/layout/PageHeader'
import { Badge } from '@/components/ui/Badge'
import { EmptyState } from '@/components/ui/EmptyState'
import { PageSpinner } from '@/components/ui/Spinner'
import { LEARNING_STAGE_META } from '@/config/learning'
import { WORK_SITUATION_LABEL } from '@/config/tasks'
import { getErrorMessage } from '@/lib/http'
import { useTaskReviewQueue } from '../api/moderationApi'
import { ModerationBoard } from '../components/ModerationBoard'
import { TaskReviewDialog } from '../components/TaskReviewDialog'

export default function TaskApprovalsPage() {
  const { data: tasks, isLoading, isError, error } = useTaskReviewQueue()
  const [reviewId, setReviewId] = useState<number | null>(null)

  if (isLoading) return <PageSpinner label="Loading the review queue…" />
  if (isError || !tasks) return <EmptyState icon={ShieldCheck} title="Review queue could not be loaded" description={getErrorMessage(error)} />

  return (
    <>
      <PageHeader
        title="Task approvals"
        description="Check each industry task before students can see it. Approve realistic entry-level work; reject with a clear note so the company can improve it."
      />
      <ModerationBoard
        items={tasks}
        emptyHint="New and edited industry tasks will appear here."
        onOpen={(task) => setReviewId(task.id)}
        renderTags={(task) => (
          <>
            <Badge tone="primary">{task.careerPathName}</Badge>
            <Badge tone={LEARNING_STAGE_META[task.level].tone}>{LEARNING_STAGE_META[task.level].label}</Badge>
            <Badge>{WORK_SITUATION_LABEL[task.situation]}</Badge>
          </>
        )}
      />
      <TaskReviewDialog taskId={reviewId} onClose={() => setReviewId(null)} />
    </>
  )
}
