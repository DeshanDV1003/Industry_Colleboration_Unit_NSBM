import { Badge } from '@/components/ui/Badge'
import { Spinner } from '@/components/ui/Spinner'
import { AnswerFeedbackPanel } from '@/features/interviews'
import { useStudentInterviewReport } from '../api/lecturerApi'

/** The student's saved soft-skills and technical answers, for kind and useful feedback. */
export function InterviewReviewSection({ studentId }: { studentId: number }) {
  const { data: report, isLoading } = useStudentInterviewReport(studentId)

  if (isLoading || !report) return <Spinner className="py-6" />
  if (report.items.length === 0) return <p className="text-sm text-muted">No interview practice yet.</p>

  return (
    <div className="space-y-5">
      {report.items.map((item) => (
        <div key={item.answer.answerId} className={item.isFollowUp ? 'ml-6' : ''}>
          <div className="mb-1 flex flex-wrap gap-1.5">
            <Badge tone="primary">{item.stage === 'SoftSkills' ? 'Soft skills' : `Technical · ${item.careerPathName}`}</Badge>
            <Badge>{item.topic}</Badge>
            {item.isFollowUp && <Badge tone="accent">Follow-up</Badge>}
          </div>
          <p className="font-medium text-heading">{item.prompt}</p>
          <p className="my-2 whitespace-pre-line text-sm">{item.answer.text}</p>
          <AnswerFeedbackPanel feedback={item.answer} />
        </div>
      ))}
    </div>
  )
}
