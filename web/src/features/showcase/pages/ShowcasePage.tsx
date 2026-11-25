import { Link } from 'react-router-dom'
import { ArrowRight, Bot, CalendarHeart, ExternalLink, GraduationCap, ShieldCheck, Trophy, Wrench } from 'lucide-react'
import { PageSpinner } from '@/components/ui/Spinner'
import { BRAND, COPYRIGHT } from '@/config/brand'
import { useShowcase } from '../api/showcaseApi'
import { PartnerEnquiryForm } from '../components/PartnerEnquiryForm'
import { ShowcaseHeader } from '../components/ShowcaseHeader'
import { CareerPathsGrid, EventList, InternshipList, PartnerList, ProfileList, Section, StatsBand } from '../components/ShowcaseSections'

const FEATURES = [
  { icon: Wrench, title: 'Real industry tasks', text: 'Practise company-style work set by partner engineers, with instant checks and feedback.' },
  { icon: Bot, title: 'AI interview practice', text: 'Rehearse with an AI interviewer that asks follow-ups, then get rubric feedback. Private to you.' },
  { icon: CalendarHeart, title: 'Talks and hackathons', text: 'Meet companies at events and earn verifiable certificates.' },
  { icon: Trophy, title: 'Verified skill badges', text: 'Lecturers review your practice evidence and award badges you can share with employers.' },
]

/** The public homepage for prospective students, parents and companies. Built from live platform data. */
export default function ShowcasePage() {
  const { data, isLoading, isError } = useShowcase()

  return (
    <div className="min-h-screen bg-page">
      <ShowcaseHeader />

      <section className="bg-brand text-white">
        <div className="mx-auto max-w-6xl px-4 pb-24 pt-16">
          <p className="text-sm font-semibold uppercase tracking-[0.2em] text-white/80">{BRAND.university} · {BRAND.faculty}</p>
          <h1 className="mt-4 max-w-3xl font-display text-4xl font-bold leading-tight text-white md:text-5xl">
            From the classroom to your first tech role, with industry beside you.
          </h1>
          <p className="mt-5 max-w-2xl text-lg text-white/90">
            The {BRAND.unit} connects computing students with partner companies through real tasks, interview practice,
            events and internships, from your first year to graduation.
          </p>
          <div className="mt-8 flex flex-wrap gap-3">
            <a href={BRAND.website} target="_blank" rel="noopener noreferrer"
              className="inline-flex h-12 items-center gap-2 rounded-control bg-white px-5 font-semibold text-primary hover:bg-white/90">
              <GraduationCap className="size-5" /> Study at NSBM <ExternalLink className="size-4" />
            </a>
            <a href="#partners" className="inline-flex h-12 items-center gap-2 rounded-control border border-white/60 px-5 font-semibold text-white hover:bg-white/10 hover:text-white">
              Partner with us <ArrowRight className="size-4" />
            </a>
          </div>
        </div>
      </section>

      {isLoading ? <PageSpinner label="Loading…" /> : isError || !data ? (
        <p className="mx-auto max-w-6xl px-4 py-16 text-muted">Live figures are unavailable right now. Please try again shortly.</p>
      ) : (
        <>
          <div className="px-4"><StatsBand stats={data.stats} /></div>

          <Section id="how" title="How students get industry-ready" intro="Everything happens on one platform, guided by lecturers and industry partners.">
            <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
              {FEATURES.map((f) => (
                <div key={f.title} className="rounded-card border border-line bg-surface p-5 shadow-card">
                  <f.icon className="size-6 text-accent" />
                  <h3 className="mt-3 font-semibold text-heading">{f.title}</h3>
                  <p className="mt-1 text-sm text-muted">{f.text}</p>
                </div>
              ))}
            </div>
          </Section>

          <Section id="career-paths" tinted title="Choose your career path" intro="Each path has a step-by-step roadmap, a skills baseline and matching industry tasks.">
            <CareerPathsGrid paths={data.careerPaths} />
          </Section>

          <Section id="internships" title="Internships open now" intro="Practice vacancies from partner companies, matched to students' skills.">
            <InternshipList internships={data.internships} />
          </Section>

          <Section id="events" tinted title="Coming up" intro="Public programmes from the Unit and events hosted by partner companies.">
            <EventList events={data.upcomingEvents} />
          </Section>

          {data.featuredProfiles.length > 0 && (
            <Section id="students" title="Meet our students" intro="Students who chose to share their profile cards, with badges verified by lecturers.">
              <ProfileList profiles={data.featuredProfiles} />
            </Section>
          )}

          <Section id="partners" tinted title="For companies" intro="Set real tasks, find interns early, host events and mentor the next generation of engineers.">
            <div className="grid gap-8 lg:grid-cols-[minmax(0,1fr)_minmax(0,1.3fr)]">
              <div className="space-y-4">
                {data.partners.length > 0 && <><p className="text-sm font-semibold text-heading">Already partnering with us</p><PartnerList partners={data.partners} /></>}
                <p className="flex gap-2 text-sm text-muted"><ShieldCheck className="mt-0.5 size-4 shrink-0 text-success" /> Every task, vacancy and event is reviewed by the Unit before students see it.</p>
              </div>
              <PartnerEnquiryForm />
            </div>
          </Section>
        </>
      )}

      <footer className="border-t border-line bg-surface">
        <div className="mx-auto flex max-w-6xl flex-wrap items-center justify-between gap-4 px-4 py-8 text-sm text-muted">
          <div>
            <p className="font-semibold text-heading">{BRAND.unit}</p>
            <p>{BRAND.unitHead.name}, {BRAND.unitHead.title}</p>
          </div>
          <div className="flex gap-4">
            <a href={BRAND.website} target="_blank" rel="noopener noreferrer">nsbm.ac.lk</a>
            <Link to="/login">Sign in</Link>
          </div>
          <p className="w-full text-xs">{COPYRIGHT}</p>
        </div>
      </footer>
    </div>
  )
}
