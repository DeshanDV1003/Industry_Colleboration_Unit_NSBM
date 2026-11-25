// Captures every page and key dialog of every portal for the system document (docs/user-guide).
// Run with the API (5300) and web app (5301) up, against a DEMO database (it resets the demo data):
//   node scripts/capture-docs-screenshots.mjs
// Output: docs/user-guide/screenshots/<portal>/<nn>-<name>.png and manifest.json (what was captured, and any failures).

import { chromium, request } from '@playwright/test'
import { mkdirSync, writeFileSync, rmSync } from 'node:fs'
import path from 'node:path'

const WEB = process.env.DOCS_WEB_URL ?? 'http://localhost:5301'
const API = process.env.DOCS_API_URL ?? 'http://localhost:5300/api/v1'
const OUT = path.resolve(process.cwd(), '../docs/user-guide/screenshots')
const VIEWPORT = { width: 1440, height: 900 }
const MAX_HEIGHT = 2100

const ACCOUNTS = {
  admin: ['icu.head@nsbm.demo', 'Admin@123'],
  lecturer: ['lecturer@nsbm.demo', 'Lecturer@123'],
  student: ['student@nsbm.demo', 'Student@123'],
  partner: ['partner@nsbm.demo', 'Industry@123'],
  qaPartner: ['partner3@nsbm.demo', 'Industry@123'], // Ceylon QualityFirst: hosts the finished Bug Bash
  registrar: ['registrar@nsbm.demo', 'Registrar@123'],
}

const manifest = []
const failures = []

// ---------------------------------------------------------------- API helpers
async function api() { return request.newContext({ baseURL: `${API}/` }) }

async function session(account) {
  const ctx = await api()
  const [email, password] = ACCOUNTS[account]
  const res = await ctx.post('auth/login', { data: { email, password } })
  if (!res.ok()) throw new Error(`login ${account}: ${res.status()}`)
  const b = await res.json()
  await ctx.dispose()
  return { accessToken: b.accessToken, expiresAt: b.expiresAt, userId: b.userId, email: b.email, fullName: b.fullName, role: b.role }
}

async function apiAs(account, method, url, data) {
  const s = await session(account)
  const ctx = await api()
  const res = await ctx.fetch(url, { method, data, headers: { Authorization: `Bearer ${s.accessToken}` } })
  const text = await res.text()
  await ctx.dispose()
  if (!res.ok()) throw new Error(`${method} ${url}: ${res.status()} ${text.slice(0, 200)}`)
  return text ? JSON.parse(text) : null
}

// ---------------------------------------------------------------- browser helpers
let browser

async function portal(account) {
  const context = await browser.newContext({ viewport: VIEWPORT })
  if (account) {
    const s = await session(account)
    await context.addInitScript(([k, v]) => window.localStorage.setItem(k, v),
      ['nsbm-icu-auth', JSON.stringify({ state: { session: s }, version: 0 })])
  }
  await context.addInitScript(() => window.localStorage.setItem('nsbm-icu-theme', JSON.stringify({ state: { theme: 'light' }, version: 0 })))
  const page = await context.newPage()
  return { context, page }
}

/** Waits for data to load, hides demo-only chrome and toasts, then screenshots (tall pages up to MAX_HEIGHT). */
async function shot(page, group, name, title, { full = true, keepToasts = false } = {}) {
  try {
    await page.waitForLoadState('networkidle', { timeout: 15_000 }).catch(() => {})
    await page.waitForFunction(() => !document.querySelector('[aria-busy="true"], .animate-spin'), null, { timeout: 15_000 }).catch(() => {})
    await page.waitForTimeout(600)
    await page.evaluate((keep) => {
      for (const label of document.querySelectorAll('label')) {
        if (label.textContent?.includes('Demo role switcher')) (label.parentElement ?? label).style.display = 'none'
      }
      if (!keep) for (const t of document.querySelectorAll('[role="status"]')) if (t.querySelector('button[aria-label="Dismiss notification"]')) t.style.display = 'none'
    }, keepToasts)
    await page.evaluate(() => window.scrollTo(0, 0))
    await page.waitForTimeout(150)
    const dir = path.join(OUT, group)
    mkdirSync(dir, { recursive: true })
    const index = String(manifest.filter((m) => m.group === group).length + 1).padStart(2, '0')
    const file = path.join(dir, `${index}-${name}.png`)
    const height = full ? Math.min(MAX_HEIGHT, await page.evaluate(() => document.documentElement.scrollHeight)) : VIEWPORT.height
    await page.screenshot({ path: file, fullPage: full, clip: full ? { x: 0, y: 0, width: VIEWPORT.width, height } : undefined })
    manifest.push({ group, name, title, file: path.relative(path.resolve(OUT, '..'), file).replaceAll('\\', '/'), url: page.url().replace(WEB, '') })
    console.log(`  ✓ ${group}/${index}-${name}`)
  } catch (e) {
    failures.push({ group, name, error: String(e).slice(0, 300) })
    console.log(`  ✗ ${group}/${name}: ${String(e).slice(0, 160)}`)
  }
}

async function visit(page, url, group, name, title, options) {
  try {
    await page.goto(WEB + url)
    await page.getByRole('main').first().waitFor({ timeout: 15_000 }).catch(() => {})
    await shot(page, group, name, title, options)
  } catch (e) {
    failures.push({ group, name, error: String(e).slice(0, 300) })
    console.log(`  ✗ ${group}/${name}: ${String(e).slice(0, 160)}`)
  }
}

/** Runs a UI step; a failure is recorded but never stops the run. */
async function step(label, fn) {
  try { await fn() } catch (e) { failures.push({ group: 'setup', name: label, error: String(e).slice(0, 300) }); console.log(`  ✗ ${label}: ${String(e).slice(0, 160)}`) }
}

/**
 * Gives the documented student realistic activity so every page shows what it does, not an empty state:
 * interview answers, a CV, planner items, a mentor question with a partner's reply, applications that a partner
 * moves forward, and a skill badge request waiting for a lecturer.
 */
async function prepareStudentActivity() {
  const soft = await apiAs('student', 'GET', 'me/interviews/SoftSkills')
  for (const q of soft.questions.slice(0, 3))
    await apiAs('student', 'POST', 'me/interviews/answers', { questionId: q.id, inputMode: 'Typed',
      text: 'In my final-year project I led a team of four building a booking system. I planned the sprint, split the work by skills and held short daily check-ins. When we fell behind I re-prioritised the backlog with the team, and we delivered the core features on time. I learned to communicate early and to ask for help.' })
  const tech = await apiAs('student', 'GET', 'me/interviews/Technical')
  for (const q of tech.questions.slice(0, 2))
    await apiAs('student', 'POST', 'me/interviews/answers', { questionId: q.id, inputMode: 'Typed',
      text: 'An interface defines a contract with no implementation, so a class can implement several. An abstract class can hold shared state and partial implementation but a class inherits only one. I use interfaces for capabilities such as IRepository and abstract classes for a common base with shared logic.' })

  await apiAs('student', 'PUT', 'me/cv', {
    headline: 'Software engineering undergraduate · aspiring backend developer',
    summary: 'Third-year BSc (Hons) Software Engineering student who enjoys building reliable web APIs. Practised industry tasks on debugging and input validation, and working towards a backend internship.',
    education: 'BSc (Hons) Software Engineering, NSBM Green University (2024 – present)',
    projects: 'Campus event booking system (C#, ASP.NET Core, SQL Server): REST API with role-based access and automated tests.',
    gitHubUrl: 'https://github.com/example-student', linkedInUrl: null })

  const iso = (days) => { const d = new Date(); d.setDate(d.getDate() + days); return d.toISOString().slice(0, 10) }
  await apiAs('student', 'POST', 'me/planner', { kind: 'Goal', title: 'Close the REST API skill gap', notes: 'Finish the API roadmap step and one industry task.', dueDate: iso(14) })
  await apiAs('student', 'POST', 'me/planner', { kind: 'Task', title: 'Practise SQL joins for 30 minutes', notes: null, dueDate: iso(2) })
  await apiAs('student', 'POST', 'me/planner', { kind: 'Deadline', title: 'Submit internship application to Lanka CloudWorks', notes: null, dueDate: iso(5) })

  await apiAs('student', 'POST', 'mentoring/questions', { title: 'How do I prepare for a backend internship interview?',
    body: 'I am comfortable with C# and SQL. Which topics should I revise first, and is it worth building a small project before applying?' })
  const queue = await apiAs('partner', 'GET', 'mentoring/queue')
  const asked = (queue.items ?? queue).find?.((q) => q.title?.startsWith('How do I prepare for a backend'))
  if (asked) await apiAs('partner', 'POST', `mentoring/questions/${asked.id}/reply`, {
    reply: 'Revise HTTP, REST design, SQL joins and indexing first. A small API project you can explain end to end is worth more than many tutorials. Bring it to the interview.' })

  const vacancies = await apiAs('student', 'GET', 'career-center/vacancies')
  for (const v of vacancies.filter((v) => /Lanka CloudWorks|Serendib/.test(v.companyName)).slice(0, 2))
    await apiAs('student', 'POST', `career-center/vacancies/${v.id}/applications`, {
      coverNote: 'I am a third-year software engineering student who has practised your company tasks on this platform. I would like to grow as a backend developer with your team.' })
  const applications = await apiAs('partner', 'GET', 'candidates/applications')
  const mine = (applications.items ?? applications).filter?.((a) => /Sahan/.test(a.studentName ?? '')) ?? []
  for (const a of mine) await apiAs('partner', 'PUT', `candidates/applications/${a.id}/status`, { status: 'InterviewInvited' })

  const achievements = await apiAs('student', 'GET', 'me/achievements')
  const skill = achievements.eligibleSkills?.[0]
  if (skill) await apiAs('student', 'POST', 'me/achievements/badges', { skillId: skill.skillId ?? skill.id, note: 'I fixed the discount bug task and passed the knowledge check.' })
}

// ---------------------------------------------------------------- the run
async function main() {
  rmSync(OUT, { recursive: true, force: true })
  mkdirSync(OUT, { recursive: true })
  browser = await chromium.launch({ channel: 'msedge' })

  console.log('Resetting demo data…')
  await apiAs('admin', 'POST', 'settings/demo-reset')

  // ------------------------------------------------ public pages
  console.log('Public pages')
  {
    const { context, page } = await portal(null)
    await visit(page, '/', 'public', 'showcase', 'Public showcase homepage')
    await visit(page, '/login', 'public', 'sign-in', 'Sign in', { full: false })
    await visit(page, '/forgot-password', 'public', 'forgot-password', 'Forgot password', { full: false })
    await visit(page, '/reset-password?email=student%40nsbm.demo&token=example', 'public', 'reset-password', 'Choose a new password (opened from the e-mailed link)', { full: false })
    const certs = await apiAs('student', 'GET', 'events/certificates').catch(() => [])
    if (certs[0]) await visit(page, `/verify/${certs[0].code}`, 'public', 'verify-certificate', 'Public certificate verification')
    await context.close()
  }

  // ------------------------------------------------ student
  console.log('Student portal')
  {
    const { context, page } = await portal('student')
    await visit(page, '/dashboard', 'student', 'dashboard-new', 'Dashboard for a new student (no career path yet)')
    await visit(page, '/pathways', 'student', 'career-pathways', 'Career pathways')

    await step('choose career path', async () => {
      const card = page.locator('article, div').filter({ has: page.getByRole('heading', { name: 'Software Engineering' }) }).last()
      await card.getByRole('button', { name: 'Choose this path' }).click()
      await page.getByText('Career path saved.', { exact: false }).waitFor()
    })
    await visit(page, '/baseline', 'student', 'skills-baseline-quiz', 'Skills baseline: the questions')
    await step('answer baseline', async () => {
      const groups = page.getByRole('main').getByRole('group').filter({ has: page.getByRole('radio') })
      await groups.first().waitFor()
      const n = await groups.count()
      for (let i = 0; i < n; i++) await groups.nth(i).getByRole('radio').nth(i % 3 === 0 ? 1 : 0).check()
      await page.getByRole('button', { name: 'Submit answers' }).click()
      await page.getByText(/%/).first().waitFor()
    })
    await shot(page, 'student', 'skills-baseline-result', 'Skills baseline: the result')

    await page.goto(WEB + '/roadmap')
    await step('tick roadmap steps', async () => {
      for (let i = 0; i < 2; i++) {
        await page.getByRole('checkbox', { name: /^Mark ".+" as done$/ }).first().click()
        await page.waitForTimeout(800)
      }
    })
    await shot(page, 'student', 'learning-roadmap', 'Learning roadmap')
    await visit(page, '/readiness', 'student', 'readiness-and-gaps', 'Readiness and skill gaps')
    await visit(page, '/tasks', 'student', 'industry-task-library', 'Industry task library')

    const tasks = await apiAs('student', 'GET', 'practice/tasks').catch(() => [])
    const task = tasks.find((t) => /bug|discount|validate/i.test(t.title)) ?? tasks[0]
    if (task) {
      await visit(page, `/tasks/${task.id}`, 'student', 'task-workbench', 'Task workbench (brief, code editor and automatic checks)')
      await step('submit task attempt', async () => {
        await page.getByRole('button', { name: 'Submit attempt' }).click()
        await page.getByText(/checks passed/).first().waitFor({ timeout: 15_000 })
      })
      await shot(page, 'student', 'task-attempt-feedback', 'Task attempt with check results')
      await visit(page, `/assessment/${task.id}`, 'student', 'knowledge-check', 'Knowledge check for a task')
    }

    await step('prepare student activity', prepareStudentActivity)
    await visit(page, '/team-simulation', 'student', 'team-simulation', 'Team simulation (practice sprint board)')
    await visit(page, '/mentor-center', 'student', 'mentor-center', 'Mentor center')
    await visit(page, '/events', 'student', 'events-and-hackathons', 'Events and hackathons')
    const events = await apiAs('student', 'GET', 'events').catch(() => [])
    const hack = events.find((e) => /hack/i.test(e.title)) ?? events[0]
    if (hack) await visit(page, `/events/${hack.id}`, 'student', 'event-detail', 'Event details, registration and teams')
    await visit(page, '/certificates', 'student', 'certificates', 'My certificates')
    await visit(page, '/portfolio', 'student', 'portfolio', 'Portfolio')
    await visit(page, '/achievements', 'student', 'achievements', 'Achievements: streak, level, badges and profile card')
    await visit(page, '/ai-interview', 'student', 'ai-interviewer', 'AI interviewer')
    await visit(page, '/interview-prep', 'student', 'soft-skills-practice', 'Soft-skills interview practice')
    await visit(page, '/interview', 'student', 'technical-interview', 'Technical interview practice')
    await visit(page, '/interview-report', 'student', 'interview-report', 'Interview report')
    await visit(page, '/cv-studio', 'student', 'cv-studio', 'CV studio')
    await visit(page, '/career-center', 'student', 'career-center', 'Career center (internship vacancies)')
    await visit(page, '/applications', 'student', 'my-applications', 'My applications')
    await visit(page, '/planner', 'student', 'planner', 'Planner')
    await visit(page, '/calendar', 'student', 'year-calendar', 'Year calendar (with lecture halls)')
    await visit(page, '/notifications', 'student', 'notifications', 'Notifications and weekly summary')
    await visit(page, '/my-settings', 'student', 'my-settings', 'My settings (e-mail choices and password)')
    await step('open change-password dialog', async () => { await page.getByRole('button', { name: 'Change password' }).click() })
    await shot(page, 'student', 'change-password', 'Change password', { full: false })
    await visit(page, '/dashboard', 'student', 'dashboard', 'Dashboard with progress')
    await step('open user menu', async () => { await page.getByRole('button', { name: /Sahan Fernando/ }).click() })
    await shot(page, 'student', 'user-menu', 'User menu', { full: false })
    await context.close()

    // Public profile card: share it, then open it signed out.
    await step('share profile card', async () => { await apiAs('student', 'PUT', 'me/achievements/profile-card', { share: true }) })
    const ach = await apiAs('student', 'GET', 'me/achievements').catch(() => null)
    const slug = ach?.profileCard?.slug
    if (slug) {
      const pub = await portal(null)
      await visit(pub.page, `/card/${slug}`, 'public', 'profile-card', "A student's public profile card")
      await pub.context.close()
    }
  }

  // ------------------------------------------------ industry partner
  console.log('Industry partner portal')
  {
    const { context, page } = await portal('partner')
    await visit(page, '/dashboard', 'partner', 'dashboard', 'Dashboard')
    await visit(page, '/industry', 'partner', 'industry-challenges', 'Industry challenges (tasks)')
    await visit(page, '/industry/tasks/new', 'partner', 'task-editor-new', 'Create an industry task')
    const myTasks = await apiAs('partner', 'GET', 'industry-tasks/mine').catch(() => [])
    const editable = myTasks.find((t) => t.status !== 'Approved') ?? myTasks[0]
    if (editable) await visit(page, `/industry/tasks/${editable.id}/edit`, 'partner', 'task-editor-edit', 'Edit an industry task (pending review)')
    await visit(page, '/industry-hub', 'partner', 'vacancies', 'Vacancies (industry hub)')
    await visit(page, '/industry-hub/vacancies/new', 'partner', 'vacancy-editor-new', 'Create a vacancy')
    const myVacancies = await apiAs('partner', 'GET', 'vacancies/mine').catch(() => [])
    if (myVacancies[0]) await visit(page, `/industry-hub/vacancies/${myVacancies[0].id}/edit`, 'partner', 'vacancy-locked', 'An approved vacancy is locked for editing')
    await visit(page, '/candidate-review', 'partner', 'candidate-review', 'Candidate review')
    await step('select an applicant', async () => {
      await page.getByRole('main').getByRole('button').filter({ hasText: /DEMO-0/ }).first().click()
      await page.waitForTimeout(800)
    })
    await shot(page, 'partner', 'candidate-evidence', "Comparing an applicant's skills and practice evidence with the vacancy")
    await visit(page, '/industry-feedback', 'partner', 'mentoring-and-reviews', 'Mentoring and task reviews')
    await visit(page, '/industry-events', 'partner', 'events', 'Events and hackathons (hosting)')
    await visit(page, '/industry-events/new', 'partner', 'event-editor-new', 'Create an event')
    const myEvents = await apiAs('partner', 'GET', 'industry-events/mine').catch(() => [])
    if (myEvents[0]) await visit(page, `/industry-events/${myEvents[0].id}/edit`, 'partner', 'event-editor-edit', 'Edit an event')
    {
      const qa = await portal('qaPartner')
      const qaEvents = await apiAs('qaPartner', 'GET', 'industry-events/mine').catch(() => [])
      const finished = qaEvents.find((e) => e.registeredCount > 0 && /bug bash/i.test(e.title)) ?? qaEvents.find((e) => e.registeredCount > 0)
      if (finished) await visit(qa.page, `/industry-events/${finished.id}/attendance`, 'partner', 'event-attendance', 'Attendance and certificates (a finished event)')
      await qa.context.close()
    }
    await visit(page, '/calendar', 'partner', 'year-calendar', 'Year calendar')
    await visit(page, '/notifications', 'partner', 'notifications', 'Notifications')
    await context.close()
  }

  // ------------------------------------------------ lecturer
  console.log('Lecturer portal')
  {
    const { context, page } = await portal('lecturer')
    await visit(page, '/dashboard', 'lecturer', 'dashboard', 'Dashboard')
    await visit(page, '/lecturer', 'lecturer', 'student-directory', 'Student directory')
    await visit(page, '/lecturer-insights', 'lecturer', 'student-insights', 'Student insights')
    await step('pick a student for insights', async () => {
      const select = page.getByRole('combobox').first()
      const options = await select.locator('option').allTextContents()
      const sahan = options.find((o) => o.includes('Sahan'))
      if (sahan) await select.selectOption({ label: sahan })
    })
    await shot(page, 'lecturer', 'student-insights-detail', "Student insights for one student (support signals, notes, assigned tasks)")
    await visit(page, '/badge-reviews', 'lecturer', 'badge-reviews', 'Badge reviews')
    await visit(page, '/analytics', 'lecturer', 'analytics', 'Analytics')
    await visit(page, '/research', 'lecturer', 'research-analytics', 'Research analytics')
    await visit(page, '/reports', 'lecturer', 'reports', 'Reports (student participation)')
    await step('show participation report', async () => { await page.getByRole('button', { name: 'Show report' }).click(); await page.getByRole('region').first().waitFor() })
    await shot(page, 'lecturer', 'reports-preview', 'Student participation report preview')
    await visit(page, '/calendar', 'lecturer', 'year-calendar', 'Year calendar')
    await context.close()
  }

  // ------------------------------------------------ assistant registrar
  console.log('Assistant Registrar portal')
  {
    const { context, page } = await portal('registrar')
    await visit(page, '/dashboard', 'registrar', 'dashboard', 'Dashboard')
    await visit(page, '/hall-bookings', 'registrar', 'hall-allocations', 'Hall allocations (this week)')
    await step('next week', async () => { await page.getByRole('button', { name: 'Next week' }).click() })
    await shot(page, 'registrar', 'hall-allocations-next-week', 'Hall allocations (next week, with weekly lectures)')
    await step('open allocate dialog', async () => { await page.getByRole('button', { name: 'Allocate hall' }).first().click(); await page.getByRole('dialog').waitFor() })
    await shot(page, 'registrar', 'allocate-hall-lecture', 'Allocate a hall to a cohort lecture', { full: false })
    await step('partner event tab', async () => { await page.getByRole('dialog').getByRole('tab', { name: 'Partner event' }).click() })
    await shot(page, 'registrar', 'allocate-hall-event', 'Allocate a hall to a partner event', { full: false })
    await visit(page, '/venues', 'registrar', 'venues', 'Venues')
    await step('open venue dialog', async () => { await page.getByRole('button', { name: 'Add venue' }).click(); await page.getByRole('dialog').waitFor() })
    await shot(page, 'registrar', 'add-venue', 'Add a venue', { full: false })
    await visit(page, '/reports', 'registrar', 'reports', 'Reports (hall timetable)')
    await step('show timetable', async () => { await page.getByRole('button', { name: 'Show report' }).click(); await page.getByRole('region').first().waitFor() })
    await shot(page, 'registrar', 'hall-timetable-report', 'Hall timetable report preview')
    await visit(page, '/calendar', 'registrar', 'year-calendar', 'Year calendar')
    await visit(page, '/notifications', 'registrar', 'notifications', 'Notifications')
    await context.close()
  }

  // ------------------------------------------------ unit admin
  console.log('Admin portal')
  {
    const { context, page } = await portal('admin')
    await visit(page, '/dashboard', 'admin', 'dashboard', 'Dashboard')
    await visit(page, '/calendar', 'admin', 'year-calendar', 'Year calendar (planning)')
    await step('open add-event dialog', async () => { await page.getByRole('button', { name: /Add event/ }).first().click(); await page.getByRole('dialog').waitFor() })
    await shot(page, 'admin', 'add-calendar-event', 'Add a calendar event', { full: false })
    await visit(page, '/admin', 'admin', 'task-approvals', 'Task approvals')
    await step('open task review', async () => { await page.getByRole('button', { name: 'Review' }).first().click(); await page.getByRole('dialog').waitFor() })
    await shot(page, 'admin', 'task-review-dialog', 'Reviewing a partner task', { full: false })
    await visit(page, '/admin-vacancies', 'admin', 'vacancy-approvals', 'Vacancy approvals')
    await visit(page, '/admin-events', 'admin', 'event-approvals', 'Event approvals')
    await visit(page, '/users', 'admin', 'users', 'Users')
    await step('open add account', async () => { await page.getByRole('button', { name: 'Add account' }).click(); await page.getByRole('dialog').waitFor() })
    await shot(page, 'admin', 'add-account', 'Add an account', { full: false })
    await step('close dialog', async () => { await page.keyboard.press('Escape') })
    await step('open import', async () => { await page.getByRole('button', { name: 'Import students' }).click(); await page.getByRole('dialog').waitFor() })
    await shot(page, 'admin', 'import-students', 'Import students from CSV', { full: false })
    await step('close dialog', async () => { await page.keyboard.press('Escape') })
    await step('open reset password', async () => { await page.getByRole('button', { name: /Reset password for Tharindu/ }).click(); await page.getByRole('dialog').waitFor() })
    await shot(page, 'admin', 'reset-password', "Reset a user's password", { full: false })
    await step('send a sample announcement', async () => {
      await apiAs('admin', 'POST', 'announcements', { audience: 'Students', title: 'Industry Week registrations are open',
        message: 'Register for the partner talks and the Green Hack 48 hackathon under Events and hackathons before 10 December.' })
    })
    await visit(page, '/announcements', 'admin', 'announcements', 'Announcements')
    await visit(page, '/hall-bookings', 'admin', 'hall-allocations-week', 'Hall allocations (read-only)')
    await step('admin next week', async () => { await page.getByRole('button', { name: 'Next week' }).click() })
    await shot(page, 'admin', 'hall-allocations', 'Hall allocations next week (read-only for the admin)')
    await step('sample partner enquiry', async () => {
      const ctx = await api()
      await ctx.post('showcase/partner-enquiries', { data: { organisationName: 'Colombo Data Systems', contactName: 'Nuwan Fernando',
        email: 'careers@colombodata.example', phone: '+94 11 234 5678', interest: 'Internships',
        message: 'We would like to offer four data engineering internships next year and run a workshop on SQL performance.', website: '' } })
      await ctx.dispose()
    })
    await visit(page, '/partner-enquiries', 'admin', 'partner-enquiries', 'Partner enquiries')
    await visit(page, '/analytics', 'admin', 'analytics', 'Analytics dashboard')
    await visit(page, '/reports', 'admin', 'reports', 'Reports')
    await step('applications report', async () => {
      await page.getByRole('button', { name: /Internship applications/ }).click()
      await page.getByRole('button', { name: 'Show report' }).click()
      await page.getByRole('region').first().waitFor()
    })
    await shot(page, 'admin', 'applications-report', 'Internship applications report preview')
    await visit(page, '/audit', 'admin', 'audit-log', 'Audit log')
    await visit(page, '/research', 'admin', 'research-overview', 'Research overview')
    await visit(page, '/settings', 'admin', 'settings', 'Settings (profile, e-mail test, demo reset)')
    await visit(page, '/notifications', 'admin', 'notifications', 'Notifications')
    await context.close()
  }

  // Demo mode, once, for the appendix: the sign-in page panel and the role switcher.
  console.log('Demo mode (appendix)')
  {
    const { context, page } = await portal('admin')
    await page.goto(WEB + '/dashboard')
    await page.waitForLoadState('networkidle').catch(() => {})
    const dir = path.join(OUT, 'demo')
    mkdirSync(dir, { recursive: true })
    await page.screenshot({ path: path.join(dir, '01-role-switcher.png') })
    manifest.push({ group: 'demo', name: 'role-switcher', title: 'Demo role switcher (demo mode only)', file: 'screenshots/demo/01-role-switcher.png', url: '/dashboard' })
    await context.close()
  }

  await browser.close()
  console.log('Restoring demo data…')
  await apiAs('admin', 'POST', 'settings/demo-reset')
  writeFileSync(path.join(OUT, 'manifest.json'), JSON.stringify({ captured: manifest, failures }, null, 2))
  console.log(`\n${manifest.length} screenshots, ${failures.length} failures.`)
  if (failures.length) console.log(JSON.stringify(failures, null, 2))
}

main().catch(async (e) => { console.error(e); await browser?.close(); process.exit(1) })
