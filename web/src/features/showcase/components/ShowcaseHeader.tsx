import { Link } from 'react-router-dom'
import { LayoutDashboard, LogIn } from 'lucide-react'
import { BrandLogo } from '@/components/brand/BrandLogo'
import { BRAND } from '@/config/brand'
import { useSession } from '@/stores/authStore'

const NAV = [
  { href: '#career-paths', label: 'Career paths' },
  { href: '#internships', label: 'Internships' },
  { href: '#events', label: 'Events' },
  { href: '#partners', label: 'For companies' },
]

export function ShowcaseHeader() {
  const session = useSession()
  return (
    <header className="sticky top-0 z-30 border-b border-line bg-surface">
      <div className="mx-auto flex h-16 max-w-6xl items-center gap-4 px-4">
        <Link to="/" className="flex items-center gap-3" aria-label={`${BRAND.unit} home`}>
          <BrandLogo className="h-9" />
          <span className="hidden text-sm font-semibold leading-tight text-heading sm:block">{BRAND.unit}<br /><span className="text-xs font-normal text-muted">{BRAND.faculty}</span></span>
        </Link>
        <nav className="ml-auto hidden gap-5 text-sm md:flex">
          {NAV.map((n) => <a key={n.href} href={n.href} className="text-body hover:text-primary">{n.label}</a>)}
        </nav>
        <Link to={session ? '/dashboard' : '/login'}
          className="ml-auto inline-flex h-9 items-center gap-1.5 rounded-control bg-primary px-4 text-sm font-semibold text-white hover:bg-primary-hover hover:text-white md:ml-0">
          {session ? <><LayoutDashboard className="size-4" /> My portal</> : <><LogIn className="size-4" /> Sign in</>}
        </Link>
      </div>
    </header>
  )
}
