import { Link } from 'react-router-dom'
import { ArrowRight } from 'lucide-react'
import { Badge } from '@/components/ui/Badge'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { NAVIGATION } from '@/config/navigation'
import type { Role } from '@/config/roles'
import { cn } from '@/lib/cn'

/** Every area of the portal at a glance, grouped like the sidebar. */
export function PortalWorkspace({ role }: { role: Role }) {
  const sections = NAVIGATION[role].filter((section) => section.title)

  return (
    <Card>
      <CardHeader title="Your workspace" description="Everything available in this portal." />
      <CardBody className="grid gap-6 md:grid-cols-2 xl:grid-cols-4">
        {sections.map((section) => (
          <div key={section.title}>
            <p className="mb-2 text-xs font-bold uppercase tracking-wider text-muted">{section.title}</p>
            <ul className="space-y-1.5">
              {section.items.map(({ label, to, icon: Icon, ready }) => {
                const content = (
                  <>
                    <span className="grid size-8 shrink-0 place-items-center rounded-lg bg-primary-soft text-primary">
                      <Icon className="size-4" />
                    </span>
                    <span className="flex-1 truncate font-medium text-heading">{label}</span>
                    {ready ? <ArrowRight className="size-4 text-muted" /> : <Badge className="text-[10px]">Soon</Badge>}
                  </>
                )
                const className = 'flex items-center gap-3 rounded-xl border border-line px-3 py-2.5 text-sm transition-colors'

                return (
                  <li key={to}>
                    {ready ? (
                      <Link to={to} className={cn(className, 'hover:border-primary hover:bg-primary-soft')}>
                        {content}
                      </Link>
                    ) : (
                      <div className={cn(className, 'opacity-70')}>{content}</div>
                    )}
                  </li>
                )
              })}
            </ul>
          </div>
        ))}
      </CardBody>
    </Card>
  )
}
