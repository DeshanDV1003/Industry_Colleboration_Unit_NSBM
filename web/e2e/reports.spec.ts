import { expect, test } from './support/fixtures'

test.describe('Reports', () => {
  test('registrar previews the hall timetable and downloads it as Excel', async ({ page, signInAs }) => {
    await signInAs('registrar')
    await page.goto('/reports')
    await expect(page.getByRole('button', { name: /Hall timetable/ })).toHaveAttribute('aria-pressed', 'true')
    await expect(page.getByRole('button', { name: /Event attendance/ })).toHaveCount(0)

    await page.getByRole('button', { name: 'Show report' }).click()
    const report = page.getByRole('region', { name: 'Hall timetable' })
    await expect(report.getByRole('columnheader', { name: 'Venue' })).toBeVisible()
    await expect(report.getByRole('cell', { name: 'Software Architecture' }).first()).toBeVisible()
    await expect(page.getByRole('button', { name: 'Print', exact: true })).toBeEnabled()

    const download = page.waitForEvent('download')
    await page.getByRole('button', { name: 'Download Excel' }).click()
    expect((await download).suggestedFilename()).toMatch(/^hall-timetable-\d{4}-\d{2}-\d{2}\.xlsx$/)
  })

  test('changing a filter asks for a fresh preview before printing', async ({ page, signInAs }) => {
    await signInAs('registrar')
    await page.goto('/reports')
    await page.getByRole('button', { name: 'Show report' }).click()
    await expect(page.getByRole('button', { name: 'Print', exact: true })).toBeEnabled()
    await page.getByLabel('Cohort').selectOption({ label: 'BSc (Hons) Software Engineering · Year 3' })
    await expect(page.getByRole('button', { name: 'Print', exact: true })).toBeDisabled()
  })

  test('lecturer gets student participation, as counts', async ({ page, signInAs }) => {
    await signInAs('lecturer')
    await page.goto('/reports')
    await expect(page.getByRole('button', { name: /Student participation/ })).toBeVisible()
    await expect(page.getByRole('button', { name: /Hall timetable/ })).toHaveCount(0)
    await page.getByRole('button', { name: 'Show report' }).click()
    const report = page.getByRole('region', { name: 'Student participation' })
    await expect(report.getByText('not a ranking', { exact: false })).toBeVisible()
    await expect(report.getByRole('cell', { name: 'DEMO-001' })).toBeVisible()
  })

  test('admin can run every report', async ({ page, signInAs }) => {
    await signInAs('admin')
    await page.goto('/reports')
    for (const title of ['Hall timetable', 'Event attendance', 'Internship applications', 'Student participation'])
      await expect(page.getByRole('button', { name: new RegExp(title) })).toBeVisible()
    await page.getByRole('button', { name: /Event attendance/ }).click()
    await page.getByRole('button', { name: 'Show report' }).click()
    await expect(page.getByRole('region', { name: 'Event attendance' }).getByRole('columnheader', { name: 'Certificate code' })).toBeVisible()
  })
})
