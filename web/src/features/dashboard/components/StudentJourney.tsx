import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { STUDENT_JOURNEY } from '../content'

export function StudentJourney() {
  return (
    <Card>
      <CardHeader title="Your journey" description="Five steps from choosing a career to landing an internship." />
      <CardBody>
        <ol className="grid gap-3 sm:grid-cols-2 lg:grid-cols-5">
          {STUDENT_JOURNEY.map(({ title, description, icon: Icon }, index) => (
            <li key={title} className="relative rounded-xl border border-line bg-surface-subtle p-4">
              <div className="flex items-center gap-3">
                <span className="grid size-9 place-items-center rounded-lg bg-primary text-white">
                  <Icon className="size-[18px]" />
                </span>
                <span className="text-xs font-bold text-muted">STEP {index + 1}</span>
              </div>
              <p className="mt-3 font-semibold text-heading">{title}</p>
              <p className="mt-0.5 text-sm text-muted">{description}</p>
            </li>
          ))}
        </ol>
      </CardBody>
    </Card>
  )
}
