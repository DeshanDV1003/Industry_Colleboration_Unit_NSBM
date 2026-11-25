import { StreakSummary } from '@/features/achievements'
import { useCurrentUser } from '@/features/auth'
import { WeeklyDigestCard } from '@/features/notifications'
import { useSession } from '@/stores/authStore'
import { PortalWorkspace } from '../components/PortalWorkspace'
import { ResponsibleUseNote } from '../components/ResponsibleUseNote'
import { StudentJourney } from '../components/StudentJourney'
import { StudentOverviewCards } from '../components/StudentOverviewCards'
import { WelcomeHero } from '../components/WelcomeHero'

export default function DashboardPage() {
  const session = useSession()
  const { data: profile } = useCurrentUser()

  if (!session) return null
  const { role, fullName } = session

  return (
    <div className="space-y-6">
      <WelcomeHero role={role} fullName={fullName} profile={profile} />
      <WeeklyDigestCard />
      {role === 'Student' && (
        <>
          <StreakSummary />
          <StudentOverviewCards />
          <StudentJourney />
        </>
      )}
      <PortalWorkspace role={role} />
      <ResponsibleUseNote role={role} />
    </div>
  )
}
