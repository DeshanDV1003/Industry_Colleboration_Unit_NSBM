import { apiAs } from './support/api'
import { expect, test } from './support/fixtures'

interface EmailPreferences { notificationsByEmail: boolean; weeklyDigestByEmail: boolean; emailEnabled: boolean }

test.describe('My settings and e-mail', () => {
  test('a student turns e-mail updates off from My settings, and it sticks', async ({ page, signInAs }) => {
    await signInAs('tharindu')
    await page.goto('/dashboard')
    await page.getByRole('button', { name: /Tharindu Silva/ }).click()
    await page.getByRole('menuitem', { name: 'My settings' }).click()
    await expect(page).toHaveURL(/\/my-settings$/)

    const updates = page.getByLabel('Updates by e-mail')
    await expect(updates).toBeChecked()
    await updates.uncheck()
    await expect(page.getByText('E-mail choices saved.')).toBeVisible()

    await page.reload()
    await expect(page.getByLabel('Updates by e-mail')).not.toBeChecked()
    await expect(page.getByLabel('Weekly summary')).toBeChecked()
    expect((await apiAs<EmailPreferences>('tharindu', 'GET', 'me/email-preferences')).notificationsByEmail).toBe(false)

    // Leave the demo account as it was.
    await apiAs('tharindu', 'PUT', 'me/email-preferences', { notificationsByEmail: true, weeklyDigestByEmail: true })
  })

  test('admin sends a test e-mail from Settings', async ({ page, signInAs }) => {
    await signInAs('admin')
    await page.goto('/settings')
    await page.getByRole('button', { name: 'Send test e-mail' }).click()
    await expect(page.getByText('Test e-mail sent to icu.head@nsbm.demo.')).toBeVisible()
  })
})
