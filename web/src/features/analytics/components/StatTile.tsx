import { Card } from '@/components/ui/Card'

/** A single headline figure: label, value, and the context that makes it meaningful. */
export function StatTile({ label, value, context }: { label: string; value: string | number; context: string }) {
  return (
    <Card className="p-5">
      <p className="text-sm text-muted">{label}</p>
      <p className="mt-1 text-3xl font-bold text-heading">{value}</p>
      <p className="mt-1 text-xs text-muted">{context}</p>
    </Card>
  )
}
