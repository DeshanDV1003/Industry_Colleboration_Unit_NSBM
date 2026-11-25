import { Award, BadgeCheck, Flame, GraduationCap, Mountain } from 'lucide-react'
import { BRAND } from '@/config/brand'
import { formatDate } from '@/lib/format'
import { LEVEL_META } from '../config'
import type { ProfileCard } from '../types'

/** The shareable profile card, used on the public page. Shows only what the student chose to share. */
export function ProfileCardView({ card }: { card: ProfileCard }) {
  return (
    <article className="overflow-hidden rounded-card border border-line bg-surface shadow-card">
      <header className="bg-primary px-6 py-6 text-white">
        <div className="flex items-start justify-between gap-4">
          <div>
            <h1 className="font-display text-2xl font-bold text-white">{card.fullName}</h1>
            <p className="mt-1 flex items-center gap-1.5 text-sm text-white/90"><GraduationCap className="size-4" /> {card.programme}</p>
            {card.careerPathName && <p className="text-sm text-white/90">Preparing for {card.careerPathName}</p>}
          </div>
          <img src={BRAND.logo.white} alt={BRAND.university} className="h-10" />
        </div>
      </header>
      <div className="grid gap-px bg-line sm:grid-cols-3">
        <div className="bg-surface p-4"><p className="flex items-center gap-1.5 text-xs text-muted"><Mountain className="size-3.5" /> Roadmap level</p><p className="font-semibold text-heading">{LEVEL_META[card.level].label}</p></div>
        <div className="bg-surface p-4"><p className="flex items-center gap-1.5 text-xs text-muted"><Flame className="size-3.5" /> Practice streak</p><p className="font-semibold text-heading">{card.currentStreak} weeks (best {card.bestStreak})</p></div>
        <div className="bg-surface p-4"><p className="flex items-center gap-1.5 text-xs text-muted"><BadgeCheck className="size-3.5" /> Verified badges</p><p className="font-semibold text-heading">{card.badges.length}</p></div>
      </div>
      <div className="space-y-5 p-6 text-sm">
        <section>
          <h2 className="mb-2 text-xs font-semibold uppercase tracking-wide text-muted">Skill badges</h2>
          {card.badges.length === 0 ? <p className="text-muted">No badges yet.</p> : (
            <ul className="flex flex-wrap gap-2">
              {card.badges.map((b) => (
                <li key={b.verificationCode} className="rounded-xl border border-success bg-success-soft px-3 py-2">
                  <p className="flex items-center gap-1.5 font-semibold text-success"><BadgeCheck className="size-4" /> {b.skillName}</p>
                  <p className="text-[11px] text-muted">Reviewed by a lecturer · {formatDate(b.approvedAt)} · {b.verificationCode}</p>
                </li>
              ))}
            </ul>
          )}
        </section>
        {card.certificates.length > 0 && (
          <section>
            <h2 className="mb-2 text-xs font-semibold uppercase tracking-wide text-muted">Event certificates</h2>
            <ul className="space-y-1.5">
              {card.certificates.map((c) => (
                <li key={c.code} className="flex flex-wrap items-center gap-2">
                  <Award className="size-4 text-accent" />
                  <a href={`/verify/${c.code}`} className="font-medium">{c.eventTitle}</a>
                  <span className="text-xs text-muted">{c.companyName} · {formatDate(c.issuedAt)}</span>
                </li>
              ))}
            </ul>
          </section>
        )}
        <p className="border-t border-line pt-3 text-xs text-muted">
          Badges are awarded by {BRAND.faculty} lecturers from practice evidence on the {BRAND.unit} platform. They show
          practice, not a formal qualification.
        </p>
      </div>
    </article>
  )
}
