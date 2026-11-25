import { useState } from 'react'
import { ClipboardList, NotebookPen } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { Button } from '@/components/ui/Button'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { controlClass } from '@/components/ui/controlClass'
import { cn } from '@/lib/cn'
import { getErrorMessage } from '@/lib/http'
import { useAddSupportNote, useAssignableTasks, useAssignTask, type StudentInsights } from '../api/lecturerApi'

/** Write a feedback note (shown in the student's portfolio) or assign a practice task. */
export function LecturerActions({ student }: { student: StudentInsights }) {
  const [note, setNote] = useState('')
  const [taskId, setTaskId] = useState('')
  const [taskNote, setTaskNote] = useState('')
  const addNote = useAddSupportNote()
  const assign = useAssignTask()
  const { data: tasks } = useAssignableTasks()
  const assigned = new Set(student.assignments.map((a) => a.taskId))

  return (
    <div className="space-y-6">
      <Card>
        <CardHeader title="Write a support note" description="Kind, specific guidance. The student sees it in their portfolio." />
        <CardBody className="space-y-2">
          <textarea aria-label="Support note" rows={4} value={note} onChange={(e) => setNote(e.target.value)}
            placeholder="e.g. Great progress on test design — next, try automating one test case." className={cn(controlClass(false), 'py-2.5')} />
          <Button size="sm" loading={addNote.isPending} disabled={note.trim().length < 10}
            onClick={() => addNote.mutate({ studentId: student.id, text: note }, {
              onSuccess: () => { setNote(''); toast.success('Note saved and shared with the student.') },
              onError: (e) => toast.error(getErrorMessage(e)),
            })}>
            <NotebookPen className="size-4" /> Save note
          </Button>
        </CardBody>
      </Card>

      <Card>
        <CardHeader title="Assign a practice task" description="It appears first in the student's task library." />
        <CardBody className="space-y-2">
          <select aria-label="Task to assign" value={taskId} onChange={(e) => setTaskId(e.target.value)} className={cn(controlClass(false), 'h-10')}>
            <option value="">Choose a task…</option>
            {tasks?.map((t) => (
              <option key={t.id} value={t.id} disabled={assigned.has(t.id)}>
                {t.careerPathName} · {t.title}{assigned.has(t.id) ? ' (assigned)' : ''}
              </option>
            ))}
          </select>
          <input aria-label="Message for the student" value={taskNote} onChange={(e) => setTaskNote(e.target.value)}
            placeholder="Optional message" className={cn(controlClass(false), 'h-10')} />
          <Button size="sm" loading={assign.isPending} disabled={!taskId}
            onClick={() => assign.mutate({ studentId: student.id, taskId: Number(taskId), note: taskNote.trim() || null }, {
              onSuccess: () => { setTaskId(''); setTaskNote(''); toast.success('Task assigned.') },
              onError: (e) => toast.error(getErrorMessage(e)),
            })}>
            <ClipboardList className="size-4" /> Assign task
          </Button>
        </CardBody>
      </Card>
    </div>
  )
}
