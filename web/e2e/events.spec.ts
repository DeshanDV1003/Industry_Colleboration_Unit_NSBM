import { resetDemoData } from './support/api'
import { expect, test } from './support/fixtures'

// Starts from the seed: earlier specs may have registered the student or formed teams.
test.beforeAll(resetDemoData)

test.describe('Events, teams and certificates', () => {
  test('student registers for a hackathon and starts a team', async ({ page, signInAs }) => {
    await signInAs('student')
    await page.goto('/events')
    await page.getByRole('link', { name: /Green Hack 48/ }).click()
    await page.getByRole('button', { name: 'Register', exact: true }).click()
    await expect(page.getByText('You are registered').first()).toBeVisible()
    // Registering refreshes the event and can re-render the team form while typing, so retry until the team shows.
    await expect(async () => {
      const name = page.getByLabel('New team name')
      if (await name.isVisible()) {
        await name.fill('E2E Builders')
        await page.getByRole('button', { name: 'Create' }).click()
      }
      await expect(page.getByText('E2E Builders').first()).toBeVisible({ timeout: 3_000 })
    }).toPass({ timeout: 20_000 })
    await expect(page.getByText('(you)')).toBeVisible()
    await expect(page.locator('code').filter({ hasText: /^[A-Z2-9]{6}$/ })).toBeVisible()
  })

  test('certificate holder can view it and its public verification page', async ({ page, signInAs }) => {
    await signInAs('kavindi')
    await page.goto('/certificates')
    await expect(page.getByRole('heading', { name: 'Certificate of participation' })).toBeVisible()
    const href = await page.getByRole('link', { name: /printable view/i }).getAttribute('href')
    expect(href).toMatch(/^\/verify\/ICU-/)
    await page.goto(href!)
    await expect(page.getByText('Genuine certificate issued by the Industry Collaboration Unit')).toBeVisible()
  })

  test('partner sees the attendance sheet for a finished event', async ({ page, signInAs }) => {
    await signInAs('qaPartner')
    await page.goto('/industry-events')
    await page.locator('tr').filter({ hasText: 'Bug Bash Challenge' }).getByRole('link', { name: /Attendance/ }).click()
    await expect(page.getByRole('heading', { name: /Attendance: Bug Bash/ })).toBeVisible()
    await expect(page.getByRole('checkbox', { name: /attended/ }).first()).toBeChecked()
  })
})
