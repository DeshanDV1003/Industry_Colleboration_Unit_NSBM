import { Flame } from 'lucide-react'
import { Card, CardBody } from '@/components/ui/Card'
import { cn } from '@/lib/cn'
import type { Progress } from '../types'

/** Weekly practice streak with the last weeks as dots, newest on the right. */
export function StreakCard({ progress, compact = false }: { progress: Progress; compact?: boolean }) {
  const { currentStreak, bestStreak, activeThisWeek, recentWeeks } = progress
  return (
    <Card>
      <CardBody className={cn('flex flex-wrap items-center gap-5', compact && 'py-4')}>
        <div className="flex items-center gap-3">
          <span className={cn('grid size-12 place-items-center rounded-2xl', currentStreak > 0 ? 'bg-warning-soft text-warning' : 'bg-surface-subtle text-muted')}>
            <Flame className="size-6" />
          </span>
          <div>
            <p className="font-display text-2xl font-bold text-heading">{currentStreak} week{currentStreak === 1 ? '' : 's'}</p>
            <p className="text-xs text-muted">Practice streak · best {bestStreak}</p>
          </div>
        </div>
        <div className="min-w-0 flex-1">
          <div className="flex gap-1" aria-label={`Practice in the last ${recentWeeks.length} weeks`}>
            {recentWeeks.map((active, i) => (
              <span key={i} title={active ? 'Practised' : 'No practice'}
                className={cn('h-6 flex-1 rounded', active ? 'bg-warning' : 'bg-surface-subtle', i === recentWeeks.length - 1 && 'ring-2 ring-warning-soft')} />
            ))}
          </div>
          <p className="mt-1.5 text-xs text-muted">
            {activeThisWeek ? 'You have practised this week. Nice!' : currentStreak > 0 ? 'Practise once before Sunday to keep your streak.' : 'Practise once this week to start a streak.'}
          </p>
        </div>
      </CardBody>
    </Card>
  )
}
