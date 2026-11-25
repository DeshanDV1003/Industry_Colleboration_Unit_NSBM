import { expect, test } from './support/fixtures'

test.describe('Public pages (signed out)', () => {
  test('homepage shows live figures and sections', async ({ page, expectNoCrash }) => {
    await page.goto('/')
    await expect(page.getByRole('heading', { level: 1 })).toContainText('first tech role')
    await expect(page.getByText('Open internships', { exact: true })).toBeVisible()
    await expect(page.getByRole('heading', { name: 'Choose your career path' })).toBeVisible()
    await expect(page.getByRole('heading', { name: 'Internships open now' })).toBeVisible()
    await expect(page.getByRole('link', { name: 'Sign in' }).first()).toBeVisible()
    await expect(page.getByRole('link', { name: /Study at NSBM/ })).toHaveAttribute('href', /nsbm\.ac\.lk/)
    await expectNoCrash()
  })

  test('partner enquiry form validates, then sends', async ({ page }) => {
    await page.goto('/#partners')
    await page.getByRole('button', { name: 'Send enquiry' }).click()
    await expect(page.getByText('Enter your organisation.')).toBeVisible()
    await expect(page.getByText('Enter a valid email address.')).toBeVisible()

    await page.getByLabel('Organisation').fill('E2E Test Systems (Pvt) Ltd')
    await page.getByLabel('Your name').fill('E2E Contact')
    await page.getByLabel('Work email').fill('e2e.contact@example.com')
    await page.getByLabel("I'm interested in").selectOption('Internships')
    await page.getByLabel('Message').fill('We would like to offer internships to second-year students.')
    await page.getByRole('button', { name: 'Send enquiry' }).click()
    await expect(page.getByText('Your enquiry has reached the Industry Collaboration Unit')).toBeVisible()
  })

  test('unknown certificate and profile card links explain themselves', async ({ page }) => {
    await page.goto('/verify/ICU-NOTREAL00')
    await expect(page.getByRole('heading', { name: 'Certificate not found' })).toBeVisible()
    await page.goto('/card/doesnotexist')
    await expect(page.getByRole('heading', { name: 'Profile card not available' })).toBeVisible()
  })

  test('protected pages send signed-out visitors to sign in', async ({ page }) => {
    await page.goto('/dashboard')
    await expect(page).toHaveURL(/\/login$/)
  })
})
