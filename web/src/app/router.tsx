import { lazy, Suspense } from 'react'
import { createBrowserRouter, Navigate } from 'react-router-dom'
import { PageSpinner } from '@/components/ui/Spinner'
import { RequireAuth, RequireRole } from './guards'
import { AppShell } from './layout/AppShell'

const LoginPage = lazy(() => import('@/features/auth/pages/LoginPage'))
const DashboardPage = lazy(() => import('@/features/dashboard/pages/DashboardPage'))
const CalendarPage = lazy(() => import('@/features/calendar/pages/CalendarPage'))
const NotificationsPage = lazy(() => import('@/features/notifications/pages/NotificationsPage'))
const EventHubPage = lazy(() => import('@/features/event-hub/pages/EventHubPage'))
const AiInterviewHomePage = lazy(() => import('@/features/ai-interview/pages/AiInterviewHomePage'))
const AchievementsPage = lazy(() => import('@/features/achievements/pages/AchievementsPage'))
const ShowcasePage = lazy(() => import('@/features/showcase/pages/ShowcasePage'))
const PartnerEnquiriesPage = lazy(() => import('@/features/showcase/pages/PartnerEnquiriesPage'))
const BadgeReviewsPage = lazy(() => import('@/features/achievements/pages/BadgeReviewsPage'))
const PublicProfileCardPage = lazy(() => import('@/features/achievements/pages/PublicProfileCardPage'))
const AiInterviewSessionPage = lazy(() => import('@/features/ai-interview/pages/AiInterviewSessionPage'))
const EventDetailPage = lazy(() => import('@/features/event-hub/pages/EventDetailPage'))
const CertificatesPage = lazy(() => import('@/features/event-hub/pages/CertificatesPage'))
const VerifyCertificatePage = lazy(() => import('@/features/event-hub/pages/VerifyCertificatePage'))
const IndustryEventsPage = lazy(() => import('@/features/industry-events/pages/IndustryEventsPage'))
const EventEditorPage = lazy(() => import('@/features/industry-events/pages/EventEditorPage'))
const EventAttendancePage = lazy(() => import('@/features/industry-events/pages/EventAttendancePage'))
const EventApprovalsPage = lazy(() => import('@/features/moderation/pages/EventApprovalsPage'))
const CareerPathsPage = lazy(() => import('@/features/careers/pages/CareerPathsPage'))
const BaselinePage = lazy(() => import('@/features/baseline/pages/BaselinePage'))
const RoadmapPage = lazy(() => import('@/features/roadmap/pages/RoadmapPage'))
const TaskLibraryPage = lazy(() => import('@/features/practice/pages/TaskLibraryPage'))
const TaskWorkbenchPage = lazy(() => import('@/features/practice/pages/TaskWorkbenchPage'))
const KnowledgeCheckPage = lazy(() => import('@/features/practice/pages/KnowledgeCheckPage'))
const IndustryChallengesPage = lazy(() => import('@/features/industry-tasks/pages/IndustryChallengesPage'))
const TaskEditorPage = lazy(() => import('@/features/industry-tasks/pages/TaskEditorPage'))
const TaskApprovalsPage = lazy(() => import('@/features/moderation/pages/TaskApprovalsPage'))
const VacancyApprovalsPage = lazy(() => import('@/features/moderation/pages/VacancyApprovalsPage'))
const ReadinessPage = lazy(() => import('@/features/readiness/pages/ReadinessPage'))
const CareerCenterPage = lazy(() => import('@/features/career-center/pages/CareerCenterPage'))
const ApplicationsPage = lazy(() => import('@/features/career-center/pages/ApplicationsPage'))
const VacancyHubPage = lazy(() => import('@/features/vacancies/pages/VacancyHubPage'))
const VacancyEditorPage = lazy(() => import('@/features/vacancies/pages/VacancyEditorPage'))
const CandidateReviewPage = lazy(() => import('@/features/candidates/pages/CandidateReviewPage'))
const InterviewPrepPage = lazy(() => import('@/features/interviews/pages/InterviewPrepPage'))
const TechnicalInterviewPage = lazy(() => import('@/features/interviews/pages/TechnicalInterviewPage'))
const InterviewReportPage = lazy(() => import('@/features/interviews/pages/InterviewReportPage'))
const CvStudioPage = lazy(() => import('@/features/cv-studio/pages/CvStudioPage'))
const PlannerPage = lazy(() => import('@/features/planner/pages/PlannerPage'))
const MentorCenterPage = lazy(() => import('@/features/mentoring/pages/MentorCenterPage'))
const TeamSimulationPage = lazy(() => import('@/features/team-simulation/pages/TeamSimulationPage'))
const PortfolioPage = lazy(() => import('@/features/portfolio/pages/PortfolioPage'))
const IndustryFeedbackPage = lazy(() => import('@/features/industry-feedback/pages/IndustryFeedbackPage'))
const StudentDirectoryPage = lazy(() => import('@/features/lecturer/pages/StudentDirectoryPage'))
const StudentInsightsPage = lazy(() => import('@/features/lecturer/pages/StudentInsightsPage'))
const ResearchPage = lazy(() => import('@/features/research/pages/ResearchPage'))
const SettingsPage = lazy(() => import('@/features/settings/pages/SettingsPage'))
const HallBookingsPage = lazy(() => import('@/features/venues/pages/HallBookingsPage'))
const VenuesPage = lazy(() => import('@/features/venues/pages/VenuesPage'))
const AnnouncementsPage = lazy(() => import('@/features/announcements/pages/AnnouncementsPage'))

export const router = createBrowserRouter([
  {
    path: '/login',
    element: (
      <Suspense fallback={<PageSpinner />}>
        <LoginPage />
      </Suspense>
    ),
  },
  {
    // Public: employers verify a certificate a student shared, without an account.
    path: '/verify/:code',
    element: (
      <Suspense fallback={<PageSpinner />}>
        <VerifyCertificatePage />
      </Suspense>
    ),
  },
  {
    // Public homepage for prospective students and companies. Signed-in users get a "My portal" button.
    path: '/',
    element: (
      <Suspense fallback={<PageSpinner />}>
        <ShowcasePage />
      </Suspense>
    ),
  },
  {
    // Public: a profile card the student chose to share.
    path: '/card/:slug',
    element: (
      <Suspense fallback={<PageSpinner />}>
        <PublicProfileCardPage />
      </Suspense>
    ),
  },
  {
    element: <RequireAuth />,
    children: [
      {
        element: <AppShell />,
        children: [
          { path: '/dashboard', element: <DashboardPage /> },
          { path: '/calendar', element: <CalendarPage /> },
          { path: '/notifications', element: <NotificationsPage /> },
          {
            element: <RequireRole roles={['Student']} />,
            children: [
              { path: '/pathways', element: <CareerPathsPage /> },
              { path: '/baseline', element: <BaselinePage /> },
              { path: '/roadmap', element: <RoadmapPage /> },
              { path: '/tasks', element: <TaskLibraryPage /> },
              { path: '/tasks/:id', element: <TaskWorkbenchPage /> },
              { path: '/assessment/:id', element: <KnowledgeCheckPage /> },
              { path: '/readiness', element: <ReadinessPage /> },
              { path: '/career-center', element: <CareerCenterPage /> },
              { path: '/applications', element: <ApplicationsPage /> },
              { path: '/interview-prep', element: <InterviewPrepPage /> },
              { path: '/interview', element: <TechnicalInterviewPage /> },
              { path: '/interview-report', element: <InterviewReportPage /> },
              { path: '/ai-interview', element: <AiInterviewHomePage /> },
              { path: '/ai-interview/:id', element: <AiInterviewSessionPage /> },
              { path: '/cv-studio', element: <CvStudioPage /> },
              { path: '/planner', element: <PlannerPage /> },
              { path: '/mentor-center', element: <MentorCenterPage /> },
              { path: '/team-simulation', element: <TeamSimulationPage /> },
              { path: '/portfolio', element: <PortfolioPage /> },
              { path: '/events', element: <EventHubPage /> },
              { path: '/events/:id', element: <EventDetailPage /> },
              { path: '/certificates', element: <CertificatesPage /> },
              { path: '/achievements', element: <AchievementsPage /> },
            ],
          },
          {
            element: <RequireRole roles={['Industry']} />,
            children: [
              { path: '/industry', element: <IndustryChallengesPage /> },
              { path: '/industry/tasks/new', element: <TaskEditorPage /> },
              { path: '/industry/tasks/:id/edit', element: <TaskEditorPage /> },
              { path: '/industry-hub', element: <VacancyHubPage /> },
              { path: '/industry-hub/vacancies/new', element: <VacancyEditorPage /> },
              { path: '/industry-hub/vacancies/:id/edit', element: <VacancyEditorPage /> },
              { path: '/candidate-review', element: <CandidateReviewPage /> },
              { path: '/industry-feedback', element: <IndustryFeedbackPage /> },
              { path: '/industry-events', element: <IndustryEventsPage /> },
              { path: '/industry-events/new', element: <EventEditorPage /> },
              { path: '/industry-events/:id/edit', element: <EventEditorPage /> },
              { path: '/industry-events/:id/attendance', element: <EventAttendancePage /> },
            ],
          },
          {
            element: <RequireRole roles={['Lecturer']} />,
            children: [
              { path: '/lecturer', element: <StudentDirectoryPage /> },
              { path: '/lecturer-insights', element: <StudentInsightsPage /> },
              { path: '/badge-reviews', element: <BadgeReviewsPage /> },
            ],
          },
          {
            element: <RequireRole roles={['Lecturer', 'Admin']} />,
            children: [{ path: '/research', element: <ResearchPage /> }],
          },
          {
            // The registrar allocates halls; Unit admins follow the same pages read-only.
            element: <RequireRole roles={['Registrar', 'Admin']} />,
            children: [
              { path: '/hall-bookings', element: <HallBookingsPage /> },
              { path: '/venues', element: <VenuesPage /> },
            ],
          },
          {
            element: <RequireRole roles={['Admin']} />,
            children: [
              { path: '/admin', element: <TaskApprovalsPage /> },
              { path: '/admin-vacancies', element: <VacancyApprovalsPage /> },
              { path: '/admin-events', element: <EventApprovalsPage /> },
              { path: '/partner-enquiries', element: <PartnerEnquiriesPage /> },
              { path: '/announcements', element: <AnnouncementsPage /> },
              { path: '/settings', element: <SettingsPage /> },
            ],
          },
        ],
      },
    ],
  },
  { path: '*', element: <Navigate to="/dashboard" replace /> },
])
