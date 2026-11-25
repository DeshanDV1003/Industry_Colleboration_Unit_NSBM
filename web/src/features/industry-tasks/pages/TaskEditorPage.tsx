import { useNavigate, useParams } from 'react-router-dom'
import type { UseFormSetError } from 'react-hook-form'
import { Lock, Wrench } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { PageHeader } from '@/components/layout/PageHeader'
import { EmptyState } from '@/components/ui/EmptyState'
import { LinkButton } from '@/components/ui/LinkButton'
import { PageSpinner } from '@/components/ui/Spinner'
import { applyServerErrors } from '@/lib/formErrors'
import { getErrorMessage } from '@/lib/http'
import { useCreateIndustryTask, useIndustryTask, useUpdateIndustryTask } from '../api/industryTasksApi'
import { TaskEditorForm } from '../components/TaskEditorForm'
import { emptyTaskForm, toFormValues, toInput, type TaskFormValues } from '../taskForm'

const LIST_PATH = '/industry'

/** Creates a new challenge (/industry/tasks/new) or edits one that is pending or rejected (/industry/tasks/:id/edit). */
export default function TaskEditorPage() {
  const { id } = useParams()
  const taskId = id ? Number(id) : null
  const navigate = useNavigate()
  const { data: task, isLoading, isError, error } = useIndustryTask(taskId)
  const create = useCreateIndustryTask()
  const update = useUpdateIndustryTask()
  const mutation = taskId === null ? create : update

  if (taskId !== null && isLoading) return <PageSpinner label="Loading task…" />
  if (taskId !== null && (isError || !task)) return <EmptyState icon={Wrench} title="Task could not be loaded" description={getErrorMessage(error)} />

  if (task && !task.canBeEdited) {
    return (
      <EmptyState
        icon={Lock}
        title="This task is live"
        description="Approved tasks are visible to students and can no longer be edited."
        action={<LinkButton to={LIST_PATH} variant="secondary">Back to challenges</LinkButton>}
      />
    )
  }

  const handleSubmit = (values: TaskFormValues, setError: UseFormSetError<TaskFormValues>) => {
    const input = toInput(values)
    const onError = (e: unknown) => {
      if (!applyServerErrors(e, setError, 'Task.')) toast.error(getErrorMessage(e))
    }
    const onSuccess = () => {
      toast.success('Submitted for review. The Industry Collaboration Unit will check it before students can see it.')
      navigate(LIST_PATH)
    }

    if (taskId === null) create.mutate(input, { onSuccess, onError })
    else update.mutate({ id: taskId, input }, { onSuccess, onError })
  }

  return (
    <>
      <PageHeader
        title={task ? 'Edit challenge' : 'New industry challenge'}
        description="Describe a realistic entry-level task. It is saved as Pending until the Unit reviews it."
      />
      <TaskEditorForm
        key={task?.id ?? 'new'}
        defaultValues={task ? toFormValues(task) : emptyTaskForm}
        submitLabel={task ? 'Save and resubmit' : 'Submit for review'}
        submitting={mutation.isPending}
        onSubmit={handleSubmit}
        onCancel={() => navigate(LIST_PATH)}
      />
    </>
  )
}
