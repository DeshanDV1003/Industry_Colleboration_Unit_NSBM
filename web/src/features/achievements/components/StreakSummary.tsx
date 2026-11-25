import { Link } from 'react-router-dom'
import { useMyAchievements } from '../api/achievementsApi'
import { StreakCard } from './StreakCard'

/** Dashboard widget: the student's weekly streak, linking to the full achievements page. */
export function StreakSummary() {
  const { data } = useMyAchievements()
  if (!data) return null
  return (
    <Link to="/achievements" className="block rounded-card transition-shadow hover:shadow-pop" aria-label="Open achievements">
      <StreakCard progress={data.progress} compact />
    </Link>
  )
}
