import { ACCOUNTS } from './support/api'
import { expect, test } from './support/fixtures'

test.describe('Signing in and portal access', () => {
  test('wrong password shows an error', async ({ page }) => {
    await page.goto('/login')
    await page.getByLabel('Email').fill(ACCOUNTS.student.email)
    await page.getByLabel('Password', { exact: true }).fill('not-the-password')
    await page.getByRole('button', { name: 'Sign in', exact: true }).click()
    await expect(page.getByRole('alert').or(page.getByText(/incorrect|invalid/i)).first()).toBeVisible()
    await expect(page).toHaveURL(/\/login$/)
  })

  test('student signs in through the form and lands on their dashboard', async ({ page }) => {
    await page.goto('/login')
    await page.getByLabel('Email').fill(ACCOUNTS.student.email)
    await page.getByLabel('Password', { exact: true }).fill(ACCOUNTS.student.password)
    await page.getByRole('button', { name: 'Sign in', exact: true }).click()
    await expect(page).toHaveURL(/\/dashboard$/)
    await expect(page.getByRole('banner').getByText('Student portal')).toBeVisible()
    await expect(page.getByRole('heading', { name: /Sahan/ })).toBeVisible()
  })

  test('a student cannot open admin pages', async ({ page, signInAs }) => {
    await signInAs('student')
    await page.goto('/admin')
    await expect(page).toHaveURL(/\/dashboard$/)
  })

  for (const [account, portal] of [['admin', 'Admin portal'], ['lecturer', 'Lecturer portal'], ['partner', 'Industry portal'], ['registrar', 'Registrar portal']] as const) {
    test(`${account} reaches the ${portal}`, async ({ page, signInAs }) => {
      await signInAs(account)
      await page.goto('/dashboard')
      await expect(page.getByRole('banner').getByText(portal)).toBeVisible()
    })
  }
})
