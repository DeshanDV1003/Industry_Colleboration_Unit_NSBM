import { Navigate } from 'react-router-dom'
import { Briefcase, Compass, MessagesSquare, Moon, Sun, Wrench } from 'lucide-react'
import { BrandLogo } from '@/components/brand/BrandLogo'
import { BRAND, COPYRIGHT } from '@/config/brand'
import { useSession } from '@/stores/authStore'
import { useThemeStore } from '@/stores/themeStore'
import { DemoAccessPanel } from '../components/DemoAccessPanel'
import { LoginForm } from '../components/LoginForm'

const HIGHLIGHTS = [
  { icon: Compass, text: 'Choose an IT career path and follow a clear roadmap' },
  { icon: Wrench, text: 'Practise real, company-style tasks from industry partners' },
  { icon: MessagesSquare, text: 'Prepare for soft-skills and technical interviews' },
  { icon: Briefcase, text: 'Build your CV and match it with internship vacancies' },
]

const DEMO_MODE = import.meta.env.VITE_DEMO_MODE === 'true'

export default function LoginPage() {
  const session = useSession()
  const { theme, toggleTheme } = useThemeStore()

  if (session) return <Navigate to="/dashboard" replace />

  return (
    <div className="flex min-h-screen bg-page">
      <aside className="relative hidden w-[46%] max-w-[680px] flex-col justify-between overflow-hidden bg-brand p-12 text-white lg:flex">
        <div aria-hidden className="absolute -right-24 -top-24 size-96 rounded-full bg-white/10" />
        <div aria-hidden className="absolute -bottom-16 -left-20 size-72 rounded-full bg-white/5" />

        <div className="relative flex items-center gap-4">
          <div className="rounded-xl bg-white px-3 py-2 shadow-lg">
            <BrandLogo inverted={false} className="h-11" />
          </div>
        </div>

        <div className="relative max-w-md">
          <p className="text-sm font-semibold uppercase tracking-[0.18em] text-white/70">{BRAND.faculty}</p>
          <h1 className="mt-3 font-display text-4xl font-bold leading-tight text-white">{BRAND.unit}</h1>
          <p className="mt-4 text-base text-white/85">
            A shared space where students, lecturers and industry partners work together to make graduates industry-ready.
          </p>
          <ul className="mt-8 space-y-3.5">
            {HIGHLIGHTS.map(({ icon: Icon, text }) => (
              <li key={text} className="flex items-center gap-3 text-[15px] text-white/90">
                <span className="grid size-9 shrink-0 place-items-center rounded-lg bg-white/15">
                  <Icon className="size-[18px]" />
                </span>
                {text}
              </li>
            ))}
          </ul>
        </div>

        <div className="relative text-sm text-white/75">
          <p className="font-semibold text-white">{BRAND.unitHead.name}</p>
          <p>{BRAND.unitHead.title}</p>
        </div>
      </aside>

      <main className="relative flex flex-1 items-center justify-center px-6 py-12">
        <button
          onClick={toggleTheme}
          className="absolute right-6 top-6 grid size-10 place-items-center rounded-xl text-muted hover:bg-surface-subtle hover:text-heading"
          aria-label={theme === 'light' ? 'Switch to dark theme' : 'Switch to light theme'}
        >
          {theme === 'light' ? <Moon className="size-5" /> : <Sun className="size-5" />}
        </button>

        <div className="animate-enter w-full max-w-[420px]">
          <div className="mb-8 lg:hidden">
            <BrandLogo className="h-12" />
          </div>

          <h2 className="text-2xl font-bold">Welcome back</h2>
          <p className="mt-1.5 text-muted">Sign in to the {BRAND.unit} platform.</p>

          <div className="mt-8">
            <LoginForm />
          </div>

          {DEMO_MODE && (
            <div className="mt-8">
              <DemoAccessPanel />
            </div>
          )}

          <p className="mt-10 text-center text-xs text-muted">
            {COPYRIGHT}
          </p>
        </div>
      </main>
    </div>
  )
}
