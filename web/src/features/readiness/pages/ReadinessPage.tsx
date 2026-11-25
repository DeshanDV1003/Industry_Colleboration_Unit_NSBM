import { Link, useSearchParams } from 'react-router-dom'
import { ArrowLeft, Compass, Target, Wrench } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { MatchMeter } from '@/components/evidence/MatchMeter'
import { SkillEvidenceList } from '@/components/evidence/SkillEvidenceList'
import { PageHeader } from '@/components/layout/PageHeader'
import { Badge } from '@/components/ui/Badge'
import { Button } from '@/components/ui/Button'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { LinkButton } from '@/components/ui/LinkButton'
import { PageSpinner } from '@/components/ui/Spinner'
import { SKILL_EVIDENCE_META, SKILL_EVIDENCE_ORDER } from '@/config/opportunities'
import { useAddPlannerItem } from '@/features/planner'
import { MySkillsCard } from '@/features/skills-profile'
import { getErrorMessage } from '@/lib/http'
import { useReadiness } from '../api/readinessApi'

/** Compares the profile with the career path (default) or with one vacancy (?vacancy=id). */
export default function ReadinessPage() {
  const [params] = useSearchParams()
  const vacancyId = Number(params.get('vacancy')) || null
  const { data: readiness, isLoading, isError, error } = useReadiness(vacancyId)
  const addToPlanner = useAddPlannerItem()

  if (isLoading) return <PageSpinner label="Comparing your skills…" />
  if (isError || !readiness) return <EmptyState icon={Target} title="Readiness could not be loaded" description={getErrorMessage(error)} />

  if (readiness.targetName === null) {
    return (
      <EmptyState
        icon={Compass}
        title="Choose a career path first"
        description="Readiness compares the skills a role needs with the skills in your profile."
        action={<LinkButton to="/pathways">Explore career roles</LinkButton>}
      />
    )
  }

  const all = [...readiness.required, ...readiness.preferred]
  const missingRequired = readiness.required.filter((r) => r.evidence === 'Missing')

  return (
    <>
      {vacancyId && <Link to="/career-center" className="mb-4 inline-flex items-center gap-1.5 text-sm font-semibold"><ArrowLeft className="size-4" /> Career center</Link>}
      <PageHeader
        title={vacancyId ? 'Readiness for this internship' : 'Career readiness and gaps'}
        description={<>
          Your profile compared with <strong className="text-heading">{readiness.targetName}</strong>
          {readiness.companyName && <> at {readiness.companyName}</>}
          {vacancyId ? ' (required and preferred skills).' : ' (core and supporting skills).'}
        </>}
      />

      <div className="grid gap-6 xl:grid-cols-[minmax(0,1fr)_360px]">
        <div className="space-y-6">
          <Card>
            <CardBody><MatchMeter percent={readiness.matchPercent} /></CardBody>
          </Card>

          <div className="grid gap-4 md:grid-cols-3">
            {SKILL_EVIDENCE_ORDER.map((evidence) => {
              const meta = SKILL_EVIDENCE_META[evidence]
              const rows = all.filter((r) => r.evidence === evidence)
              return (
                <Card key={evidence} className="p-5">
                  <div className="flex items-center justify-between">
                    <Badge tone={meta.tone}>{meta.label}</Badge>
                    <span className="font-display text-2xl font-bold text-heading">{rows.length}</span>
                  </div>
                  <p className="mt-2 text-xs text-muted">{meta.description}</p>
                </Card>
              )
            })}
          </div>

          <div className="grid gap-6 lg:grid-cols-2">
            <Card>
              <CardHeader title={vacancyId ? 'Required skills' : 'Core skills'} description="These count towards the match." />
              <CardBody><SkillEvidenceList rows={readiness.required} showSources /></CardBody>
            </Card>
            <Card>
              <CardHeader title={vacancyId ? 'Preferred skills' : 'Supporting skills'} description="Nice to have." />
              <CardBody><SkillEvidenceList rows={readiness.preferred} showSources /></CardBody>
            </Card>
          </div>
        </div>

        <div className="space-y-6">
          <MySkillsCard />
          {missingRequired.length > 0 && (
            <Card>
              <CardHeader title="Suggested next practice" description="Start with a missing required skill." />
              <CardBody className="space-y-3">
                <ul className="space-y-2">
                  {missingRequired.map((r) => (
                    <li key={r.skillId} className="flex items-center justify-between gap-2">
                      <Badge tone="danger">{r.skillName}</Badge>
                      <Button size="sm" variant="ghost" loading={addToPlanner.isPending && addToPlanner.variables?.title.includes(r.skillName)}
                        onClick={() => addToPlanner.mutate({ kind: 'Goal', title: `Practise ${r.skillName}`, notes: `Skill gap for ${readiness.targetName}`, dueDate: null },
                          { onSuccess: () => toast.success(`"Practise ${r.skillName}" added to your planner.`) })}>
                        Add to planner
                      </Button>
                    </li>
                  ))}
                </ul>
                <LinkButton to="/tasks" variant="secondary" className="w-full"><Wrench className="size-4" /> Find a practice task</LinkButton>
              </CardBody>
            </Card>
          )}
        </div>
      </div>
    </>
  )
}
