import { Link } from 'react-router-dom'
import { Building2, CheckCircle2, Clock } from 'lucide-react'
import { Badge } from '@/components/ui/Badge'
import { Card } from '@/components/ui/Card'
import { LEARNING_STAGE_META } from '@/config/learning'
import { WORK_SITUATION_LABEL } from '@/config/tasks'
import type { TaskLibraryItem } from '../types'

function ProgressNote({ task }: { task: TaskLibraryItem }) {
  if (task.myAttemptCount === 0) return <span className="text-muted">Not started</span>
  const allPassed = task.myBestPassedChecks === task.checkCount
  return (
    <span className={allPassed ? 'flex items-center gap-1 text-success' : 'text-warning'}>
      {allPassed && <CheckCircle2 className="size-4" />}
      Best: {task.myBestPassedChecks}/{task.checkCount} checks · {task.myAttemptCount} attempt{task.myAttemptCount === 1 ? '' : 's'}
    </span>
  )
}

export function TaskLibraryCard({ task }: { task: TaskLibraryItem }) {
  return (
    <Card className="flex flex-col p-5 transition-shadow hover:shadow-pop">
      <div className="flex flex-wrap gap-1.5">
        <Badge tone="primary">{task.careerPathName}</Badge>
        <Badge tone={LEARNING_STAGE_META[task.level].tone}>{LEARNING_STAGE_META[task.level].label}</Badge>
        <Badge>{WORK_SITUATION_LABEL[task.situation]}</Badge>
        {task.assignedBy && <Badge tone="accent">Assigned by {task.assignedBy}</Badge>}
      </div>
      <Link to={`/tasks/${task.id}`} className="mt-3 text-base font-semibold text-heading hover:text-primary">{task.title}</Link>
      <p className="mt-1 line-clamp-2 text-sm text-muted">{task.summary}</p>
      <div className="mt-3 flex flex-wrap gap-1">
        {task.skills.map((s) => <Badge key={s} className="text-[10px]">{s}</Badge>)}
      </div>
      <div className="mt-auto flex flex-wrap items-center justify-between gap-2 border-t border-line pt-3 text-xs">
        <span className="flex items-center gap-3 text-muted">
          <span className="flex items-center gap-1"><Building2 className="size-3.5" /> {task.companyName}</span>
          <span className="flex items-center gap-1"><Clock className="size-3.5" /> ~{task.estimatedMinutes} min</span>
        </span>
        <ProgressNote task={task} />
      </div>
    </Card>
  )
}
