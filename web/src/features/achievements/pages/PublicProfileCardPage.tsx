import { useParams } from 'react-router-dom'
import { UserX } from 'lucide-react'
import { Spinner } from '@/components/ui/Spinner'
import { COPYRIGHT } from '@/config/brand'
import { usePublicProfileCard } from '../api/achievementsApi'
import { ProfileCardView } from '../components/ProfileCardView'

/** Public page (no sign-in) for a profile card a student chose to share. */
export default function PublicProfileCardPage() {
  const slug = useParams().slug ?? ''
  const { data: card, isLoading, isError } = usePublicProfileCard(slug)

  return (
    <div className="min-h-screen bg-surface-subtle px-4 py-10">
      <div className="mx-auto max-w-2xl space-y-4">
        {isLoading ? <Spinner className="py-20" /> : isError || !card ? (
          <div className="rounded-card border border-line bg-surface p-8 text-center shadow-card">
            <UserX className="mx-auto size-10 text-muted" />
            <h1 className="mt-3 text-lg font-semibold">Profile card not available</h1>
            <p className="mt-1 text-sm text-muted">The link may be wrong, or the student has made their card private.</p>
          </div>
        ) : <ProfileCardView card={card} />}
        <p className="text-center text-xs text-muted">{COPYRIGHT}</p>
      </div>
    </div>
  )
}
