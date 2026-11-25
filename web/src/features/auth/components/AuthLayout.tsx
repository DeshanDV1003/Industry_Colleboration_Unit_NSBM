import type { ReactNode } from 'react'
import { Briefcase, Compass, MessagesSquare, Moon, Sun, Wrench } from 'lucide-react'
import { BrandLogo } from '@/components/brand/BrandLogo'
import { BRAND, COPYRIGHT } from '@/config/brand'
import { useThemeStore } from '@/stores/themeStore'

const HIGHLIGHTS = [
  { icon: Compass, text: 'Choose an IT career path and follow a clear roadmap' },
  { icon: Wrench, text: 'Practise real, company-style tasks from industry partners' },
  { icon: MessagesSquare, text: 'Prepare for soft-skills and technical interviews' },
  { icon: Briefcase, text: 'Build your CV and match it with internship vacancies' },
]

interface AuthLayoutProps {
  title: string
  description: ReactNode
  children: ReactNode
}

/** The branded two-column shell shared by the sign-in, forgot-password and reset-password pages. */
export function AuthLayout({ title, description, children }: AuthLayoutProps) {
  const { theme, toggleTheme } = useThemeStore()

  return (
    // Fixed to the window on desktop: the page itself never scrolls.
    <div className="flex min-h-screen bg-page lg:h-screen lg:overflow-hidden">
      <aside className="relative hidden w-[46%] max-w-[680px] flex-col justify-between overflow-hidden bg-brand p-10 text-white lg:flex">
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

      {/* On an unusually short window only this column scrolls, so nothing is cut off. */}
      <main className="relative flex flex-1 overflow-y-auto px-6 py-4">
        <button
          onClick={toggleTheme}
          className="absolute right-6 top-6 grid size-10 place-items-center rounded-xl text-muted hover:bg-surface-subtle hover:text-heading"
          aria-label={theme === 'light' ? 'Switch to dark theme' : 'Switch to light theme'}
        >
          {theme === 'light' ? <Moon className="size-5" /> : <Sun className="size-5" />}
        </button>

        {/* m-auto centres the form, and unlike items-center it never pushes the top out of reach when space is short. */}
        <div className="animate-enter m-auto w-full max-w-[420px]">
          <div className="mb-6 lg:hidden">
            <BrandLogo className="h-12" />
          </div>

          <h2 className="text-2xl font-bold">{title}</h2>
          <p className="mt-1.5 text-muted">{description}</p>

          <div className="mt-6">{children}</div>

          <p className="mt-6 text-center text-xs text-muted">{COPYRIGHT}</p>
        </div>
      </main>
    </div>
  )
}
