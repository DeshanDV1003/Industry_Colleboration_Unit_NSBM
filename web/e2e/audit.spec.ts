import { apiAs } from './support/api'
import { expect, test } from './support/fixtures'

test.describe('Audit log', () => {
  test('an announcement shows up in the audit log, filtered by area, and exports to Excel', async ({ page, signInAs }) => {
    const title = `E2E audit notice ${Date.now().toString().slice(-6)}`
    await apiAs('admin', 'POST', 'announcements', { audience: 'Staff', title, message: 'Checking the audit trail.' })

    await signInAs('admin')
    await page.goto('/audit')
    await expect(page.getByRole('heading', { name: 'Audit log' })).toBeVisible()

    await page.getByLabel('Area').selectOption('Communication')
    await page.getByLabel('Search').fill(title)
    const row = page.getByRole('row').filter({ hasText: title })
    await expect(row).toHaveCount(1)
    await expect(row).toContainText('announcement.sent')
    await expect(row).toContainText('Pavithra Subhashini')

    await page.getByLabel('Area').selectOption('Halls')
    await expect(page.getByText('No entries match these filters')).toBeVisible()

    const download = page.waitForEvent('download')
    await page.getByRole('button', { name: 'Export to Excel' }).click()
    expect((await download).suggestedFilename()).toMatch(/^audit-log-\d{4}-\d{2}-\d{2}\.xlsx$/)
  })

  test('only the Unit admin can open it', async ({ page, signInAs }) => {
    await signInAs('registrar')
    await page.goto('/audit')
    await expect(page).toHaveURL(/\/dashboard$/)
  })
})
