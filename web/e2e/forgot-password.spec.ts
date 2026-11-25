import { apiContext, resetDemoData } from './support/api'
import { expect, test } from './support/fixtures'
import { waitForEmail } from './support/mail'

const EMAIL = 'demo-004@nsbm.demo' // Amaya; demo reset restores her password afterwards

async function canSignIn(email: string, password: string) {
  const api = await apiContext()
  const ok = (await api.post('auth/login', { data: { email, password } })).ok()
  await api.dispose()
  return ok
}

test.describe('Forgot password', () => {
  test.afterAll(resetDemoData)

  test('a student resets a forgotten password with the e-mailed link, which then stops working', async ({ page }) => {
    const since = Date.now()
    await page.goto('/login')
    await page.getByRole('link', { name: 'Forgot password?' }).click()
    await page.getByLabel('Email').fill(EMAIL)
    await page.getByRole('button', { name: 'Send reset link' }).click()
    await expect(page.getByRole('heading', { name: 'Check your e-mail' })).toBeVisible()

    const mail = await waitForEmail(EMAIL, since)
    const link = mail.match(/http:\/\/localhost:5301\/reset-password\?email=[^\s"<]+&token=[A-Za-z0-9_-]+/)?.[0]
    expect(link, 'reset link in the e-mail').toBeTruthy()

    await page.goto(link!.replace('http://localhost:5301', ''))
    await expect(page.getByRole('heading', { name: 'Choose a new password' })).toBeVisible()
    await page.getByLabel('New password', { exact: true }).fill('Forgotten9')
    await page.getByLabel('Confirm new password').fill('Forgotten9')
    await page.getByRole('button', { name: 'Change password' }).click()
    await expect(page.getByRole('heading', { name: 'Password changed' })).toBeVisible()

    expect(await canSignIn(EMAIL, 'Forgotten9')).toBe(true)
    expect(await canSignIn(EMAIL, 'Student@123')).toBe(false)

    // Opening the same link again is refused.
    await page.goto(link!.replace('http://localhost:5301', ''))
    await page.getByLabel('New password', { exact: true }).fill('Another9pass')
    await page.getByLabel('Confirm new password').fill('Another9pass')
    await page.getByRole('button', { name: 'Change password' }).click()
    await expect(page.getByRole('alert')).toContainText('expired or was already used')
  })

  test('an unknown address gets the same answer', async ({ page }) => {
    await page.goto('/forgot-password')
    await page.getByLabel('Email').fill('nobody-here@nsbm.demo')
    await page.getByRole('button', { name: 'Send reset link' }).click()
    await expect(page.getByRole('heading', { name: 'Check your e-mail' })).toBeVisible()
  })
})
