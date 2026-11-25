import { useEffect, useRef, useState } from 'react'
import { Camera, CameraOff, ScanFace } from 'lucide-react'
import { Button } from '@/components/ui/Button'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'

/** The experimental browser FaceDetector API, available only in some browsers. */
interface FaceDetectorLike { detect: (source: HTMLVideoElement) => Promise<unknown[]> }
type FaceDetectorCtor = new () => FaceDetectorLike
const getFaceDetector = () => (window as unknown as { FaceDetector?: FaceDetectorCtor }).FaceDetector

/**
 * Optional webcam preview with a one-frame face-presence check. Nothing is recorded or uploaded, and the check
 * cannot show honesty, confidence, attention or use of AI tools.
 */
export function CameraPreview() {
  const videoRef = useRef<HTMLVideoElement>(null)
  const [stream, setStream] = useState<MediaStream | null>(null)
  const [message, setMessage] = useState<string | null>(null)

  useEffect(() => {
    if (videoRef.current) videoRef.current.srcObject = stream
    return () => stream?.getTracks().forEach((t) => t.stop())
  }, [stream])

  const start = async () => {
    try {
      setMessage(null)
      setStream(await navigator.mediaDevices.getUserMedia({ video: true, audio: false }))
    } catch {
      setMessage('Camera access was not allowed. Text-only practice works just as well.')
    }
  }

  const stop = () => {
    setStream(null)
    setMessage(null)
  }

  const checkFace = async () => {
    const Detector = getFaceDetector()
    if (!Detector || !videoRef.current) {
      setMessage('Face-presence check is not supported in this browser. The preview still works.')
      return
    }
    try {
      const faces = await new Detector().detect(videoRef.current)
      setMessage(faces.length === 1 ? 'One face is visible.' : faces.length === 0 ? 'No face detected — check your lighting.' : `${faces.length} faces detected.`)
    } catch {
      setMessage('The face-presence check could not run.')
    }
  }

  return (
    <Card>
      <CardHeader title="Camera preview (optional)" description="See how you look on camera. Nothing is recorded." />
      <CardBody className="space-y-3">
        <div className="grid aspect-video place-items-center overflow-hidden rounded-xl bg-code">
          {stream
            ? <video ref={videoRef} autoPlay muted playsInline className="size-full -scale-x-100 object-cover" aria-label="Your camera preview" />
            : <CameraOff className="size-8 text-code-muted" aria-hidden />}
        </div>
        <div className="flex flex-wrap gap-2">
          {stream ? (
            <>
              <Button size="sm" variant="secondary" onClick={checkFace}><ScanFace className="size-4" /> Face-presence check</Button>
              <Button size="sm" variant="ghost" onClick={stop}><CameraOff className="size-4" /> Turn off</Button>
            </>
          ) : (
            <Button size="sm" variant="secondary" onClick={start}><Camera className="size-4" /> Turn on camera</Button>
          )}
        </div>
        {message && <p role="status" className="text-sm text-heading">{message}</p>}
        <p className="text-xs text-muted">The camera cannot show cheating, honesty, confidence or attention. There is no live AI grading.</p>
      </CardBody>
    </Card>
  )
}
