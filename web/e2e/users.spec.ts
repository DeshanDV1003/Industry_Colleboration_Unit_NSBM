import { apiAs, apiContext } from './support/api'
import { expect, test } from './support/fixtures'
import type { Page } from '@playwright/test'

/** Accounts survive between runs only until the next demo reset, so each run uses fresh addresses. */
const run = Date.now().toString().slice(-6)

async function canSignIn(email: string, password: string): Promise<boolean> {
  const api = await apiContext()
  const response = await api.post('auth/login', { data: { email, password } })
  await api.dispose()
  return response.ok()
}

async function signInWithForm(page: Page, email: string, password: string) {
  await page.goto('/login')
  await page.getByLabel('Email').fill(email)
  await page.getByLabel('Password', { exact: true }).fill(password)
  await page.getByRole('button', { name: 'Sign in', exact: true }).click()
  await expect(page).toHaveURL(/\/dashboard$/)
}

test.describe('User management', () => {
  test('admin creates a lecturer who can then sign in, and deactivates them', async ({ page, signInAs }) => {
    const email = `e2e.lecturer${run}@nsbm.demo`
    await signInAs('admin')
    await page.goto('/users')
    await page.getByRole('button', { name: 'Add account' }).click()

    const dialog = page.getByRole('dialog')
    await dialog.getByLabel('Role').selectOption('Lecturer')
    await dialog.getByLabel('E-mail').fill(email)
    await dialog.getByLabel('Temporary password').fill('Welcome123')
    await dialog.getByLabel('First name').fill('Ruwan')
    await dialog.getByLabel('Last name').fill('Dias')
    await dialog.getByLabel('Staff number').fill(`E2E-${run}`)
    await dialog.getByLabel('Designation').fill('Senior Lecturer')
    await dialog.getByLabel('Department').fill('Computing')
    await dialog.getByRole('button', { name: 'Create account' }).click()

    await expect(page.getByText('Account created.', { exact: false })).toBeVisible()
    expect(await canSignIn(email, 'Welcome123')).toBe(true)

    await page.getByLabel('Search accounts').fill(email)
    await page.getByRole('button', { name: 'Deactivate Ruwan Dias' }).click()
    await expect(page.getByText('Deactivated')).toBeVisible()
    expect(await canSignIn(email, 'Welcome123')).toBe(false)
  })

  test('a missing role field is shown on that field', async ({ page, signInAs }) => {
    await signInAs('admin')
    await page.goto('/users')
    await page.getByRole('button', { name: 'Add account' }).click()
    const dialog = page.getByRole('dialog')
    await dialog.getByLabel('E-mail').fill(`e2e.blank${run}@nsbm.demo`)
    await dialog.getByLabel('Temporary password').fill('Welcome123')
    await dialog.getByLabel('First name').fill('Blank')
    await dialog.getByLabel('Last name').fill('Number')
    await dialog.getByRole('button', { name: 'Create account' }).click()
    await expect(dialog.getByText('Enter the student number.')).toBeVisible()
  })

  test('a new person changes their temporary password from the user menu', async ({ page }) => {
    const email = `e2e.student${run}@nsbm.demo`
    await apiAs('admin', 'POST', 'users', {
      role: 'Student', email, password: 'Welcome123',
      profile: { firstName: 'Nimal', lastName: 'Perera', studentNo: `E2E-S-${run}`, programme: 'BSc (Hons) Software Engineering', academicYear: 1, batch: '2026.1' },
    })

    await signInWithForm(page, email, 'Welcome123')
    await page.getByRole('button', { name: /Nimal Perera/ }).click()
    await page.getByRole('menuitem', { name: 'Change password' }).click()
    const dialog = page.getByRole('dialog')
    await dialog.getByLabel('Current password').fill('Welcome123')
    await dialog.getByLabel('New password', { exact: true }).fill('MyOwnPass7')
    await dialog.getByLabel('Confirm new password').fill('MyOwnPass7')
    await dialog.getByRole('button', { name: 'Change password' }).click()

    await expect(page.getByText('Password changed.', { exact: false })).toBeVisible()
    expect(await canSignIn(email, 'Welcome123')).toBe(false)
    expect(await canSignIn(email, 'MyOwnPass7')).toBe(true)
  })
})
