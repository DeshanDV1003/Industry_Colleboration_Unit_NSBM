/** Mirrors Features/Analytics/Dtos/AnalyticsDto. */
export interface WeekActivity {
  weekStart: string
  activeStudents: number
  taskAttempts: number
  interviewAnswers: number
  applications: number
}

export interface CompanyPipeline {
  company: string
  submitted: number
  underReview: number
  shortlisted: number
  interviewInvited: number
  notSelected: number
  total: number
}

export interface EventAttendance { event: string; date: string; registered: number; attended: number | null }
export interface VenueUsage { venue: string; bookedHours: number; availableHours: number }
export interface PathReadiness { careerPath: string; students: number; baselineTaken: number | null; averageRoadmapPercent: number | null }

export interface Analytics {
  weeks: number
  students: number
  activeStudentsInPeriod: number
  applicationsInPeriod: number
  upcomingHallWeeks: number
  minGroupSize: number
  activity: WeekActivity[]
  pipeline: CompanyPipeline[]
  events: EventAttendance[]
  halls: VenueUsage[]
  readiness: PathReadiness[]
}
