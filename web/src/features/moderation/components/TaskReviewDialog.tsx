import { toast } from '@/components/feedback/toast'
import { TaskDetailView, useIndustryTask } from '@/features/industry-tasks'
import { getErrorMessage } from '@/lib/http'
import { useReviewTask } from '../api/moderationApi'
import { ReviewDialog } from './ReviewDialog'

export function TaskReviewDialog({ taskId, onClose }: { taskId: number | null; onClose: () => void }) {
  const { data: task } = useIndustryTask(taskId)
  const review = useReviewTask()

  return (
    <ReviewDialog
      open={taskId !== null}
      item={task}
      pendingDecision={review.isPending ? review.variables.decision : null}
      onClose={onClose}
      onDecide={(decision, note, done) =>
        review.mutate({ id: taskId!, decision, note }, {
          onSuccess: () => {
            toast.success(decision === 'Approve' ? 'Task approved — students can now see it.' : 'Task rejected and returned to the company.')
            done()
          },
          onError: (e) => toast.error(getErrorMessage(e)),
        })}
    >
      {task && <TaskDetailView task={task} />}
    </ReviewDialog>
  )
}
