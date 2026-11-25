import { useCallback, useEffect, useRef, useState } from 'react'
import type { FaceLandmarker } from '@mediapipe/tasks-vision'
import {
  EMPTY_TOTALS, addReading, averageLuma, framing, isFacingCamera, isSteady, lightingFrom,
  type CoachingTotals, type FaceReading, type Point,
} from './faceMetrics'

/** The MediaPipe runtime is served from this site (copied at install); the face model URL is configurable. */
const WASM_BASE = '/mediapipe/wasm'
const MODEL_URL = import.meta.env.VITE_FACE_MODEL_URL
  || 'https://storage.googleapis.com/mediapipe-models/face_landmarker/face_landmarker/float16/1/face_landmarker.task'
const SAMPLE_MS = 250
const RECENT_POINTS = 8

let landmarkerPromise: Promise<FaceLandmarker> | null = null

/** Loads the face landmarker once per page. Expressions (blendshapes) are deliberately not requested. */
function loadLandmarker(): Promise<FaceLandmarker> {
  landmarkerPromise ??= import('@mediapipe/tasks-vision').then(async ({ FaceLandmarker, FilesetResolver }) => {
    const fileset = await FilesetResolver.forVisionTasks(WASM_BASE)
    return FaceLandmarker.createFromOptions(fileset, {
      baseOptions: { modelAssetPath: MODEL_URL, delegate: 'GPU' },
      runningMode: 'VIDEO',
      numFaces: 1,
      outputFaceBlendshapes: false,
      outputFacialTransformationMatrixes: true,
    })
  }).catch((e) => {
    landmarkerPromise = null
    throw e
  })
  return landmarkerPromise
}

export type CoachState = 'off' | 'loading' | 'running' | 'error'

/**
 * On-device camera coaching. Frames are analysed in memory and dropped; nothing is recorded, stored or uploaded,
 * and the readings stay in this component.
 */
export function useFaceCoach() {
  const videoRef = useRef<HTMLVideoElement>(null)
  const streamRef = useRef<MediaStream | null>(null)
  const timerRef = useRef<number | null>(null)
  const canvasRef = useRef<HTMLCanvasElement | null>(null)
  const recentRef = useRef<Point[]>([])
  const [state, setState] = useState<CoachState>('off')
  const [error, setError] = useState<string | null>(null)
  const [reading, setReading] = useState<FaceReading | null>(null)
  const [totals, setTotals] = useState<CoachingTotals>(EMPTY_TOTALS)

  const stop = useCallback(() => {
    if (timerRef.current !== null) window.clearInterval(timerRef.current)
    timerRef.current = null
    streamRef.current?.getTracks().forEach((t) => t.stop())
    streamRef.current = null
    if (videoRef.current) videoRef.current.srcObject = null
    recentRef.current = []
    setReading(null)
    setState('off')
  }, [])

  useEffect(() => stop, [stop])

  const sample = useCallback((landmarker: FaceLandmarker) => {
    const video = videoRef.current
    if (!video || video.readyState < 2) return

    const canvas = (canvasRef.current ??= document.createElement('canvas'))
    canvas.width = 32
    canvas.height = 18
    const context = canvas.getContext('2d', { willReadFrequently: true })
    if (!context) return
    context.drawImage(video, 0, 0, canvas.width, canvas.height)
    const lighting = lightingFrom(averageLuma(context.getImageData(0, 0, canvas.width, canvas.height).data))

    const result = landmarker.detectForVideo(video, performance.now())
    const landmarks = result.faceLandmarks[0]
    let next: FaceReading
    if (!landmarks) {
      recentRef.current = []
      next = { faceVisible: false, facingCamera: false, centred: false, distance: 'ok', lighting, steady: true }
    } else {
      const frame = framing(landmarks)
      recentRef.current = [...recentRef.current, frame.nose].slice(-RECENT_POINTS)
      next = {
        faceVisible: true,
        facingCamera: isFacingCamera(result.facialTransformationMatrixes?.[0]?.data),
        centred: frame.centred,
        distance: frame.distance,
        lighting,
        steady: isSteady(recentRef.current),
      }
    }
    setReading(next)
    setTotals((t) => addReading(t, next))
  }, [])

  const start = useCallback(async () => {
    setError(null)
    setState('loading')
    try {
      const stream = await navigator.mediaDevices.getUserMedia({ video: { width: 640, height: 360 }, audio: false })
      streamRef.current = stream
      if (videoRef.current) {
        videoRef.current.srcObject = stream
        await videoRef.current.play().catch(() => undefined)
      }
      const landmarker = await loadLandmarker()
      if (!streamRef.current) return // turned off while loading
      timerRef.current = window.setInterval(() => sample(landmarker), SAMPLE_MS)
      setState('running')
    } catch (e) {
      stop()
      setState('error')
      setError(e instanceof DOMException && e.name === 'NotAllowedError'
        ? 'Camera access was not allowed. The interview works without it.'
        : 'Camera coaching could not start on this device. The interview works without it.')
    }
  }, [sample, stop])

  const reset = useCallback(() => setTotals(EMPTY_TOTALS), [])

  return { videoRef, state, error, reading, totals, start, stop, reset }
}
