/**
 * Pure presentation-coaching measurements from face landmarks and video frames. They describe how the student
 * appears on camera (framing, facing the camera, light, steadiness) and nothing else: no expressions, emotions,
 * identity, honesty or attention. Everything runs in the browser; no frame or number is sent anywhere.
 */

export interface Point { x: number; y: number }

export type Distance = 'ok' | 'far' | 'close'
export type Lighting = 'ok' | 'dark' | 'bright'

export interface FaceReading {
  faceVisible: boolean
  facingCamera: boolean
  centred: boolean
  distance: Distance
  lighting: Lighting
  steady: boolean
}

export interface CoachingTotals {
  samples: number
  faceVisible: number
  facingCamera: number
  centred: number
  goodLight: number
  steady: number
}

export const EMPTY_TOTALS: CoachingTotals = { samples: 0, faceVisible: 0, facingCamera: 0, centred: 0, goodLight: 0, steady: 0 }

// MediaPipe face-mesh landmark indices.
const NOSE_TIP = 1
const LEFT_CHEEK = 234
const RIGHT_CHEEK = 454

const MAX_TURN_DEGREES = 20
const FAR_FACE_WIDTH = 0.16
const CLOSE_FACE_WIDTH = 0.55
const DARK_LUMA = 60
const BRIGHT_LUMA = 215
const STEADY_SPREAD = 0.02

/** Head yaw and pitch in degrees from MediaPipe's 4×4 column-major facial transformation matrix. */
export function headTurn(matrix: ArrayLike<number>): { yaw: number; pitch: number } {
  const toDeg = (rad: number) => (rad * 180) / Math.PI
  return {
    yaw: toDeg(Math.asin(Math.max(-1, Math.min(1, -matrix[2])))),
    pitch: toDeg(Math.atan2(matrix[6], matrix[10])),
  }
}

export const isFacingCamera = (matrix: ArrayLike<number> | undefined) => {
  if (!matrix) return false
  const { yaw, pitch } = headTurn(matrix)
  return Math.abs(yaw) <= MAX_TURN_DEGREES && Math.abs(pitch) <= MAX_TURN_DEGREES
}

export function framing(landmarks: Point[]): { centred: boolean; distance: Distance; nose: Point } {
  const nose = landmarks[NOSE_TIP]
  const width = Math.abs(landmarks[RIGHT_CHEEK].x - landmarks[LEFT_CHEEK].x)
  return {
    nose,
    centred: nose.x > 0.3 && nose.x < 0.7 && nose.y > 0.25 && nose.y < 0.75,
    distance: width < FAR_FACE_WIDTH ? 'far' : width > CLOSE_FACE_WIDTH ? 'close' : 'ok',
  }
}

/** Average perceived brightness (0–255) of RGBA pixel data. */
export function averageLuma(rgba: Uint8ClampedArray): number {
  let sum = 0
  for (let i = 0; i < rgba.length; i += 4) sum += 0.2126 * rgba[i] + 0.7152 * rgba[i + 1] + 0.0722 * rgba[i + 2]
  return sum / (rgba.length / 4)
}

export const lightingFrom = (luma: number): Lighting => (luma < DARK_LUMA ? 'dark' : luma > BRIGHT_LUMA ? 'bright' : 'ok')

/** Steady when the nose has moved less than a small fraction of the frame over the recent samples. */
export function isSteady(recent: Point[]): boolean {
  if (recent.length < 3) return true
  const spread = (values: number[]) => Math.max(...values) - Math.min(...values)
  return spread(recent.map((p) => p.x)) < STEADY_SPREAD * 4 && spread(recent.map((p) => p.y)) < STEADY_SPREAD * 4
}

export function addReading(totals: CoachingTotals, r: FaceReading): CoachingTotals {
  return {
    samples: totals.samples + 1,
    faceVisible: totals.faceVisible + Number(r.faceVisible),
    facingCamera: totals.facingCamera + Number(r.facingCamera),
    centred: totals.centred + Number(r.centred && r.distance === 'ok'),
    goodLight: totals.goodLight + Number(r.lighting === 'ok'),
    steady: totals.steady + Number(r.steady),
  }
}

export const percent = (part: number, whole: number) => (whole === 0 ? 0 : Math.round((part / whole) * 100))

/** Plain-language tips for the current moment, most useful first. */
export function liveTips(r: FaceReading): string[] {
  if (!r.faceVisible) return ['We cannot see your face. Sit in front of the camera.']
  const tips: string[] = []
  if (r.lighting === 'dark') tips.push('It is quite dark. Face a window or a lamp.')
  if (r.lighting === 'bright') tips.push('Too much light behind or on you. Try dimming it or moving.')
  if (r.distance === 'far') tips.push('Move a little closer to the camera.')
  if (r.distance === 'close') tips.push('Sit back slightly so your shoulders show.')
  if (!r.centred) tips.push('Centre yourself in the frame.')
  if (!r.facingCamera) tips.push('Look towards the camera when you speak.')
  if (!r.steady) tips.push('Try to keep your head steady.')
  return tips
}
