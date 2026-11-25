// Copies MediaPipe's WebAssembly runtime into public/ so camera coaching loads it from this site, not a CDN.
// Runs on `npm install` (postinstall). The output folder is generated and git-ignored.
import { cpSync, existsSync, mkdirSync } from 'node:fs'
import { dirname, join } from 'node:path'
import { fileURLToPath } from 'node:url'

const root = join(dirname(fileURLToPath(import.meta.url)), '..')
const source = join(root, 'node_modules', '@mediapipe', 'tasks-vision', 'wasm')
const target = join(root, 'public', 'mediapipe', 'wasm')

if (!existsSync(source)) {
  console.warn('[copy-mediapipe-wasm] @mediapipe/tasks-vision is not installed; skipping.')
  process.exit(0)
}
mkdirSync(target, { recursive: true })
cpSync(source, target, { recursive: true })
console.log('[copy-mediapipe-wasm] Copied MediaPipe runtime to public/mediapipe/wasm')
