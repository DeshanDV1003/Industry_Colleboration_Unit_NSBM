/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_API_URL?: string
  readonly VITE_DEMO_MODE?: string
  /** Where camera coaching loads the MediaPipe face model from. Defaults to Google's published model. */
  readonly VITE_FACE_MODEL_URL?: string
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}
