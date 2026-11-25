import { Badge } from '@/components/ui/Badge'
import { SUPPORT_SIGNAL_META, type SupportSignal } from '@/config/support'

export function SupportSignalBadges({ signals }: { signals: SupportSignal[] }) {
  if (signals.length === 0) return <span className="text-xs text-muted">—</span>
  return (
    <div className="flex flex-wrap gap-1">
      {signals.map((s) => <Badge key={s} tone={SUPPORT_SIGNAL_META[s].tone} title={SUPPORT_SIGNAL_META[s].description}>{SUPPORT_SIGNAL_META[s].label}</Badge>)}
    </div>
  )
}
