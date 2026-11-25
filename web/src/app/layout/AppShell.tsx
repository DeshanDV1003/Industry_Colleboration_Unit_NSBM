import { Suspense } from 'react'
import { Outlet } from 'react-router-dom'
import { PageSpinner } from '@/components/ui/Spinner'
import { COPYRIGHT } from '@/config/brand'
import { useUiStore } from '@/stores/uiStore'
import { Sidebar } from './Sidebar'
import { Topbar } from './Topbar'

export function AppShell() {
  const { mobileNavOpen, setMobileNavOpen } = useUiStore()

  return (
    <div className="flex min-h-screen bg-page">
      <Sidebar />
      {mobileNavOpen && (
        <div aria-hidden className="fixed inset-0 z-30 bg-black/40 lg:hidden" onClick={() => setMobileNavOpen(false)} />
      )}

      <div className="flex min-w-0 flex-1 flex-col">
        <Topbar />
        <main className="flex-1 px-4 py-6 lg:px-8 lg:py-8">
          <div className="animate-enter mx-auto max-w-[1360px]">
            <Suspense fallback={<PageSpinner />}>
              <Outlet />
            </Suspense>
          </div>
        </main>
        <footer className="flex flex-wrap justify-between gap-x-6 gap-y-1 border-t border-line px-8 py-5 text-xs text-muted">
          <span>{COPYRIGHT}</span>
          <span>Demo data is fictional. The platform supports people's decisions — it never makes hiring decisions.</span>
        </footer>
      </div>
    </div>
  )
}
