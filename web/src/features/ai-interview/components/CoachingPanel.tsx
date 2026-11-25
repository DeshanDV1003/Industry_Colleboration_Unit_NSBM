import { Camera, CameraOff, EyeOff, Lightbulb, Loader2, ShieldCheck } from 'lucide-react'
import { Button } from '@/components/ui/Button'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { ProgressBar } from '@/components/ui/ProgressBar'
import { cn } from '@/lib/cn'
import { liveTips, percent } from '../coaching/faceMetrics'
import { useFaceCoach } from '../coaching/useFaceCoach'

function Meter({ label, value }: { label: string; value: number }) {
  return (
    <div>
      <div className="mb-1 flex justify-between text-xs"><span>{label}</span><span className="font-semibold text-heading">{value}%</span></div>
      <ProgressBar value={value} label={label} />
    </div>
  )
}

/**
 * Optional, private camera coaching: framing, facing the camera, light and steadiness. Runs on this device only.
 * It never judges expressions, emotions, honesty or attention, and nothing is saved or shared.
 */
export function CoachingPanel() {
  const { videoRef, state, error, reading, totals, start, stop } = useFaceCoach()
  const tips = reading ? liveTips(reading) : []
  const on = state === 'running' || state === 'loading'

  return (
    <Card>
      <CardHeader
        title="Camera coach (optional)"
        description="Private tips on how you come across on video."
        action={on
          ? <Button size="sm" variant="ghost" onClick={stop}><CameraOff className="size-4" /> Turn off</Button>
          : <Button size="sm" variant="secondary" onClick={start}><Camera className="size-4" /> Turn on</Button>}
      />
      <CardBody className="space-y-4 text-sm">
        <div className={cn('relative grid aspect-video place-items-center overflow-hidden rounded-xl bg-code', !on && 'hidden')}>
          <video ref={videoRef} muted playsInline className="size-full -scale-x-100 object-cover" aria-label="Your camera preview" />
          {state === 'loading' && <span className="absolute inset-0 grid place-items-center bg-black/40 text-white"><Loader2 className="size-6 animate-spin" /></span>}
        </div>

        {state === 'running' && (
          tips.length === 0
            ? <p className="flex items-center gap-2 font-medium text-success"><ShieldCheck className="size-4" /> Looking good: well framed and well lit.</p>
            : <ul className="space-y-1">{tips.slice(0, 2).map((t) => <li key={t} className="flex gap-2 text-heading"><Lightbulb className="mt-0.5 size-4 shrink-0 text-warning" /> {t}</li>)}</ul>
        )}

        {totals.samples > 0 && (
          <div className="space-y-2.5">
            <p className="text-xs font-semibold uppercase tracking-wide text-muted">This session</p>
            <Meter label="Face visible" value={percent(totals.faceVisible, totals.samples)} />
            <Meter label="Facing the camera" value={percent(totals.facingCamera, totals.faceVisible)} />
            <Meter label="Well framed" value={percent(totals.centred, totals.faceVisible)} />
            <Meter label="Good lighting" value={percent(totals.goodLight, totals.samples)} />
            <Meter label="Steady" value={percent(totals.steady, totals.faceVisible)} />
          </div>
        )}

        {error && <p role="alert" className="text-danger">{error}</p>}

        <p className="flex gap-2 rounded-lg bg-surface-subtle p-2.5 text-xs text-muted">
          <EyeOff className="mt-0.5 size-3.5 shrink-0" />
          Runs only on this device. Video is never recorded, saved or sent, and these tips are not shared with lecturers or companies.
          It does not read expressions or emotions and cannot tell honesty, confidence or attention.
        </p>
      </CardBody>
    </Card>
  )
}
