import { Link } from 'react-router-dom'
import { ArrowRight, Sparkles } from 'lucide-react'
import { Badge } from '@/components/ui/Badge'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { Spinner } from '@/components/ui/Spinner'
import { entryMeta, formatRange } from '@/features/calendar'
import { useWeeklyDigest } from '../api/notificationsApi'
import { describeStep } from '../config'

/** "Your week": the next seven days, unread count and suggested next steps. */
export function WeeklyDigestCard() {
  const { data: digest, isLoading } = useWeeklyDigest()

  return (
    <Card>
      <CardHeader
        title={<span className="flex items-center gap-2"><Sparkles className="size-4 text-accent" /> Your week</span>}
        description={digest ? `${formatRange(digest.from, digest.to)} · ${digest.unreadCount} unread notification${digest.unreadCount === 1 ? '' : 's'}` : undefined}
        action={<Link to="/calendar" className="text-xs font-semibold">Calendar</Link>}
      />
      <CardBody className="space-y-5">
        {isLoading || !digest ? <Spinner /> : (
          <>
            {digest.nextSteps.length > 0 && (
              <section>
                <h4 className="mb-2 text-xs font-semibold uppercase tracking-wide text-muted">Next steps</h4>
                <ul className="space-y-1.5">
                  {digest.nextSteps.map((step) => {
                    const view = describeStep(step)
                    return (
                      <li key={step.kind}>
                        <Link to={view.to} className="flex items-center gap-2.5 rounded-lg px-2 py-1.5 text-sm text-heading hover:bg-surface-subtle">
                          <view.icon className="size-4 shrink-0 text-primary" />
                          <span className="flex-1">{view.text}</span>
                          <ArrowRight className="size-3.5 text-muted" />
                        </Link>
                      </li>
                    )
                  })}
                </ul>
              </section>
            )}
            <section>
              <h4 className="mb-2 text-xs font-semibold uppercase tracking-wide text-muted">Coming up</h4>
              {digest.upcoming.length === 0 ? <p className="text-sm text-muted">Nothing on your calendar in the next seven days.</p> : (
                <ul className="space-y-2">
                  {digest.upcoming.map((entry) => (
                    <li key={entry.key} className="flex items-start gap-2.5 text-sm">
                      <span className="w-20 shrink-0 text-xs font-semibold text-muted">{formatRange(entry.startDate, entry.endDate)}</span>
                      <span className="min-w-0 flex-1 text-heading">{entry.title}</span>
                      <Badge tone={entryMeta(entry).tone}>{entryMeta(entry).label}</Badge>
                    </li>
                  ))}
                </ul>
              )}
            </section>
          </>
        )}
      </CardBody>
    </Card>
  )
}
