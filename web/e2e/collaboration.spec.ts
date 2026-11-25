import { apiAs } from './support/api'
import { expect, openAs, test } from './support/fixtures'

/** Journeys that cross portals: one person acts in the UI, another person sees the result in theirs. */
test.describe('Cross-portal workflows', () => {
  test('admin approves a partner task and a student can open it', async ({ page, signInAs, browser }) => {
    const queue = await apiAs<{ title: string; status: string }[]>('admin', 'GET', 'industry-tasks/review-queue')
    const pending = queue.find((t) => t.status === 'Pending')
    test.skip(!pending, 'no pending task in the seed')

    await signInAs('admin')
    await page.goto('/admin')
    const card = page.locator('div').filter({ has: page.getByText(pending!.title, { exact: true }) })
      .filter({ has: page.getByRole('button', { name: 'Review' }) }).last()
    await card.getByRole('button', { name: 'Review' }).click()
    await page.getByRole('dialog').getByRole('button', { name: 'Approve' }).click()
    await expect(page.getByRole('dialog')).toHaveCount(0)

    const student = await openAs(browser, 'student')
    await student.goto('/tasks')
    await expect(student.getByRole('link', { name: pending!.title })).toBeVisible()
    await student.context().close()
  })

  test('admin plans a student event and only students see it', async ({ page, signInAs, browser }) => {
    const day = new Date()
    day.setDate(day.getDate() + 3)
    const iso = `${day.getFullYear()}-${String(day.getMonth() + 1).padStart(2, '0')}-${String(day.getDate()).padStart(2, '0')}`
    const dayButton = new RegExp(`^${day.getDate()} ${day.toLocaleString('en-GB', { month: 'long' })}`)

    await signInAs('admin')
    await page.goto('/calendar')
    if (day.getFullYear() !== new Date().getFullYear()) await page.getByRole('button', { name: 'Next year' }).click()
    await page.getByRole('button', { name: dayButton }).click()
    await page.getByRole('button', { name: 'Add event' }).first().click()
    const dialog = page.getByRole('dialog')
    await dialog.getByLabel('Title').fill('E2E careers clinic')
    await dialog.getByLabel('Who it is for').selectOption('Students')
    await expect(dialog.getByLabel('Start date')).toHaveValue(iso)
    await dialog.getByRole('button', { name: 'Add event' }).click()
    await expect(page.getByRole('heading', { name: 'E2E careers clinic' })).toBeVisible()

    const student = await openAs(browser, 'student')
    await student.goto('/calendar')
    if (day.getFullYear() !== new Date().getFullYear()) await student.getByRole('button', { name: 'Next year' }).click()
    await student.getByRole('button', { name: dayButton }).click()
    await expect(student.getByRole('heading', { name: 'E2E careers clinic' })).toBeVisible()
    await student.context().close()

    const partnerView = await apiAs<{ title: string }[]>('partner', 'GET', `calendar/${day.getFullYear()}`)
    expect(partnerView.some((e) => e.title === 'E2E careers clinic')).toBe(false)
  })

  test('lecturer approves a badge and the student is notified', async ({ page, signInAs, browser }) => {
    await signInAs('lecturer')
    await page.goto('/badge-reviews')
    const card = page.locator('div').filter({ hasText: 'Networking basics' })
      .filter({ has: page.getByRole('button', { name: 'Approve badge' }) }).last()
    await card.getByRole('button', { name: 'Not yet' }).click()
    await expect(page.getByText('Add a short note so the student knows what to practise.')).toBeVisible()
    await card.getByRole('button', { name: 'Approve badge' }).click()
    await expect(page.getByText('Badge approved. The student has been told.')).toBeVisible()

    const student = await openAs(browser, 'amaya')
    await student.goto('/achievements')
    await expect(student.getByText('Earned').first()).toBeVisible()
    await expect(student.getByRole('button', { name: /Notifications, \d+ unread/ })).toBeVisible()
    await student.context().close()
  })
})
