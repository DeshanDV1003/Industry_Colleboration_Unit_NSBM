import { Info } from 'lucide-react'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import type { Role } from '@/config/roles'
import { RESPONSIBLE_USE } from '../content'

export function ResponsibleUseNote({ role }: { role: Role }) {
  return (
    <Card>
      <CardHeader title="Good to know" description="How to read what the platform shows you." />
      <CardBody>
        <ul className="space-y-3">
          {RESPONSIBLE_USE[role].map((note) => (
            <li key={note} className="flex gap-3 text-sm">
              <Info className="mt-0.5 size-4 shrink-0 text-accent" />
              <span>{note}</span>
            </li>
          ))}
        </ul>
      </CardBody>
    </Card>
  )
}
