import { Download, FileText } from 'lucide-react'
import { PageHeader } from '@/components/layout/PageHeader'
import { Badge } from '@/components/ui/Badge'
import { Button } from '@/components/ui/Button'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { LinkButton } from '@/components/ui/LinkButton'
import { PageSpinner } from '@/components/ui/Spinner'
import { getErrorMessage } from '@/lib/http'
import { useInterviewReport } from '../api/interviewsApi'
import { AnswerFeedbackPanel } from '../components/AnswerFeedbackPanel'
import { downloadText } from '@/lib/download'
import { buildInterviewLog } from '../interviewLog'
import type { InterviewStage } from '../types'

const STAGES: { stage: InterviewStage; title: string }[] = [
  { stage: 'SoftSkills', title: 'Stage 1 · Soft skills' },
  { stage: 'Technical', title: 'Stage 2 · Technical' },
]

export default function InterviewReportPage() {
  const { data: report, isLoading, isError, error } = useInterviewReport()

  if (isLoading) return <PageSpinner label="Building your report…" />
  if (isError || !report) return <EmptyState icon={FileText} title="Report could not be loaded" description={getErrorMessage(error)} />

  if (report.items.length === 0) {
    return (
      <EmptyState icon={FileText} title="No practice answers yet"
        description="Answer a few questions in Stage 1 and Stage 2 to build your report."
        action={<LinkButton to="/interview-prep">Start Stage 1</LinkButton>} />
    )
  }

  return (
    <>
      <PageHeader
        title="Interview report"
        description="Your latest answer to each question you practised, with improvement notes. Your lecturer can use it to give feedback."
        actions={
          <Button onClick={() => downloadText(`interview-practice-${report.studentNo}.txt`, buildInterviewLog(report))}>
            <Download className="size-4" /> Download practice log
          </Button>
        }
      />

      <div className="space-y-6">
        {STAGES.map(({ stage, title }) => {
          const items = report.items.filter((i) => i.stage === stage)
          return (
            <Card key={stage}>
              <CardHeader title={title} description={items.length === 0 ? 'Not practised yet.' : `${items.length} answer${items.length === 1 ? '' : 's'}`} />
              {items.length > 0 && (
                <CardBody className="space-y-5">
                  {items.map((item) => (
                    <div key={item.answer.answerId} className={item.isFollowUp ? 'ml-6' : ''}>
                      <div className="mb-1 flex flex-wrap items-center gap-2">
                        <Badge>{item.topic}</Badge>
                        {item.isFollowUp && <Badge tone="accent">Follow-up</Badge>}
                        {item.careerPathName && <Badge tone="primary">{item.careerPathName}</Badge>}
                      </div>
                      <p className="font-medium text-heading">{item.prompt}</p>
                      <p className="my-2 whitespace-pre-line text-sm">{item.answer.text}</p>
                      <AnswerFeedbackPanel feedback={item.answer} />
                    </div>
                  ))}
                </CardBody>
              )}
            </Card>
          )
        })}
      </div>
    </>
  )
}
