import { ROLE_META, type Role } from '@/config/roles'
import type { CurrentUser } from '@/features/auth'
import { PORTAL_INTRO } from '../content'

const greeting = () => {
  const hour = new Date().getHours()
  return hour < 12 ? 'Good morning' : hour < 17 ? 'Good afternoon' : 'Good evening'
}

interface WelcomeHeroProps {
  role: Role
  fullName: string
  profile?: CurrentUser
}

export function WelcomeHero({ role, fullName, profile }: WelcomeHeroProps) {
  const { icon: Icon, portal } = ROLE_META[role]
  const firstName = fullName.split(' ')[0]

  return (
    <section className="relative overflow-hidden rounded-card bg-brand p-7 text-white shadow-card lg:p-9">
      <div aria-hidden className="absolute -right-16 -top-20 size-72 rounded-full bg-white/10" />
      <div aria-hidden className="absolute -bottom-24 right-40 size-56 rounded-full bg-white/5" />

      <div className="relative flex flex-wrap items-start justify-between gap-6">
        <div className="max-w-2xl">
          <p className="flex items-center gap-2 text-xs font-semibold uppercase tracking-[0.16em] text-white/75">
            <Icon className="size-4" /> {portal}
          </p>
          <h1 className="mt-3 font-display text-3xl font-bold text-white">
            {greeting()}, {firstName}
          </h1>
          <p className="mt-3 text-[15px] leading-relaxed text-white/85">{PORTAL_INTRO[role]}</p>
        </div>

        {(profile?.headline || profile?.organisation) && (
          <div className="rounded-xl bg-white/15 px-4 py-3 text-sm backdrop-blur">
            {profile.headline && <p className="font-semibold text-white">{profile.headline}</p>}
            {profile.organisation && <p className="text-white/80">{profile.organisation}</p>}
          </div>
        )}
      </div>
    </section>
  )
}
