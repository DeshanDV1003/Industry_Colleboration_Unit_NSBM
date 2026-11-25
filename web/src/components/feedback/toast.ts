import { create } from 'zustand'

export type ToastTone = 'success' | 'error' | 'info'

export interface ToastItem {
  id: number
  message: string
  tone: ToastTone
}

interface ToastState {
  items: ToastItem[]
  push: (message: string, tone: ToastTone) => void
  dismiss: (id: number) => void
}

const DISMISS_AFTER_MS = 4000
let nextId = 1

export const useToastStore = create<ToastState>((set, get) => ({
  items: [],
  push: (message, tone) => {
    const id = nextId++
    set((s) => ({ items: [...s.items, { id, message, tone }] }))
    setTimeout(() => get().dismiss(id), DISMISS_AFTER_MS)
  },
  dismiss: (id) => set((s) => ({ items: s.items.filter((t) => t.id !== id) })),
}))

/** Imperative API so non-component code (mutations, handlers) can raise notifications. */
export const toast = {
  success: (message: string) => useToastStore.getState().push(message, 'success'),
  error: (message: string) => useToastStore.getState().push(message, 'error'),
  info: (message: string) => useToastStore.getState().push(message, 'info'),
}
