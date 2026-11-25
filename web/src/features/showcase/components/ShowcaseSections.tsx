import type { ReactNode } from 'react'
import { BadgeCheck, Briefcase, Building2, CalendarDays, Compass, MapPin } from 'lucide-react'
import { Badge } from '@/components/ui/Badge'
import { WORK_MODE_LABEL } from '@/config/opportunities'
import { formatRange } from '@/features/calendar'
import { formatDate } from '@/lib/format'
import { eventKindLabel } from '../config'
import type { Showcase } from '../types'

/** Section shell: an anchor for the header links, a heading and a short intro. */
export function Section({ id, title, intro, children, tinted = false }: { id: string; title: string; intro: string; children: ReactNode; tinted?: boolean }) {
  return (
    <section id={id} className={tinted ? 'bg-surface-subtle' : undefined}>
      <div className="mx-auto max-w-6xl scroll-mt-20 px-4 py-14">
        <h2 className="font-display text-2xl font-bold text-heading md:text-3xl">{title}</h2>
        <p className="mt-2 max-w-2xl text-muted">{intro}</p>
        <div className="mt-8">{children}</div>
      </div>
    </section>
  )
}

export function StatsBand({ stats }: { stats: Showcase['stats'] }) {
  const items = [
    { value: stats.partnerCompanies, label: 'Active partner companies' },
    { value: stats.openInternships, label: 'Open internships' },
    { value: stats.industryTasks, label: 'Industry tasks to practise' },
    { value: stats.studentsOnCareerPaths, label: 'Students on a career path' },
    { value: stats.badgesAwarded, label: 'Lecturer-verified badges' },
    { value: stats.certificatesIssued, label: 'Event certificates' },
  ]
  return (
    <div className="mx-auto -mt-10 grid max-w-6xl grid-cols-2 gap-px overflow-hidden rounded-card border border-line bg-line shadow-card sm:grid-cols-3 lg:grid-cols-6">
      {items.map((i) => (
        <div key={i.label} className="bg-surface px-4 py-5 text-center">
          <p className="font-display text-3xl font-bold text-primary">{i.value}</p>
          <p className="mt-1 text-xs text-muted">{i.label}</p>
        </div>
      ))}
    </div>
  )
}

export function CareerPathsGrid({ paths }: { paths: Showcase['careerPaths'] }) {
  return (
    <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
      {paths.map((p) => (
        <div key={p.slug} className="rounded-card border border-line bg-surface p-5 shadow-card">
          <Compass className="size-6 text-primary" />
          <h3 className="mt-3 font-semibold text-heading">{p.name}</h3>
          <p className="mt-1 text-sm text-muted">{p.tagline}</p>
          <p className="mt-3 text-xs font-semibold text-primary">{p.skillCount} skills on the roadmap</p>
        </div>
      ))}
    </div>
  )
}

export function InternshipList({ internships }: { internships: Showcase['internships'] }) {
  if (internships.length === 0) return <p className="text-muted">New internships are posted regularly. Check back soon.</p>
  return (
    <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
      {internships.map((v) => (
        <div key={`${v.companyName}-${v.title}`} className="rounded-card border border-line bg-surface p-5 shadow-card">
          <div className="flex flex-wrap gap-1.5"><Badge tone="primary">{v.careerPathName}</Badge><Badge>{WORK_MODE_LABEL[v.workMode]}</Badge></div>
          <h3 className="mt-3 font-semibold text-heading">{v.title}</h3>
          <p className="mt-1 flex items-center gap-1.5 text-sm text-muted"><Building2 className="size-4" /> {v.companyName}</p>
          <p className="flex items-center gap-1.5 text-sm text-muted"><MapPin className="size-4" /> {v.location}</p>
          <p className="mt-3 text-xs text-muted">Applications close {formatDate(v.closingDate)}</p>
        </div>
      ))}
    </div>
  )
}

export function EventList({ events }: { events: Showcase['upcomingEvents'] }) {
  if (events.length === 0) return <p className="text-muted">The next programme of events is being planned.</p>
  return (
    <ul className="divide-y divide-line rounded-card border border-line bg-surface shadow-card">
      {events.map((e) => (
        <li key={`${e.title}-${e.startDate}`} className="flex flex-wrap items-center gap-4 px-5 py-4">
          <div className="w-24 shrink-0 text-sm font-semibold text-primary"><CalendarDays className="mb-1 size-4" />{formatRange(e.startDate, e.endDate)}</div>
          <div className="min-w-0 flex-1">
            <p className="font-semibold text-heading">{e.title}</p>
            <p className="text-sm text-muted">{e.host ? `Hosted by ${e.host}` : 'Industry Collaboration Unit'}{e.location && ` · ${e.location}`}</p>
          </div>
          <Badge tone="accent">{eventKindLabel(e.kind)}</Badge>
        </li>
      ))}
    </ul>
  )
}

/** Shown in the narrow column beside the enquiry form, so one partner per row. */
export function PartnerList({ partners }: { partners: Showcase['partners'] }) {
  return (
    <div className="grid gap-3">
      {partners.map((p) => (
        <div key={p.name} className="flex items-start gap-3 rounded-card border border-line bg-surface p-5 shadow-card">
          <span className="grid size-10 shrink-0 place-items-center rounded-xl bg-primary-soft text-primary"><Briefcase className="size-5" /></span>
          <div>
            <p className="font-semibold text-heading">{p.name}</p>
            <p className="text-xs text-muted">{[p.tasks && `${p.tasks} tasks`, p.internships && `${p.internships} internships`, p.events && `${p.events} events`].filter(Boolean).join(' · ')}</p>
          </div>
        </div>
      ))}
    </div>
  )
}

export function ProfileList({ profiles }: { profiles: Showcase['featuredProfiles'] }) {
  return (
    <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
      {profiles.map((p) => (
        <a key={p.cardSlug} href={`/card/${p.cardSlug}`} className="rounded-card border border-line bg-surface p-5 shadow-card transition-shadow hover:shadow-pop">
          <p className="font-semibold text-heading">{p.fullName}</p>
          <p className="text-xs text-muted">{p.programme}{p.careerPathName && ` · ${p.careerPathName}`}</p>
          <div className="mt-3 flex flex-wrap gap-1.5">
            {p.badges.length === 0 ? <span className="text-xs text-muted">Working towards their first badge</span>
              : p.badges.map((b) => <Badge key={b} tone="success"><BadgeCheck className="size-3" /> {b}</Badge>)}
          </div>
        </a>
      ))}
    </div>
  )
}
