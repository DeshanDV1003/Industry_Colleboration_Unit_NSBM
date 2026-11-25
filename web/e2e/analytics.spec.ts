import { expect, test } from './support/fixtures'

test.describe('Analytics', () => {
  test('admin sees the trend charts, each with a table view', async ({ page, signInAs }) => {
    await signInAs('admin')
    await page.goto('/analytics')
    await expect(page.getByRole('heading', { name: 'Analytics' })).toBeVisible()
    for (const title of ['Students practising each week', 'Practice by kind', 'Internship pipeline by company', 'Event attendance',
      'Hall use, next 4 weeks', 'Roadmap progress by career path'])
      await expect(page.getByRole('heading', { name: title })).toBeVisible()

    await expect(page.getByRole('img', { name: 'Students practising per week' })).toBeVisible()

    // The pipeline as a table: exact numbers, readable without colour.
    const pipeline = page.getByRole('region', { name: 'Internship pipeline by company' })
    await pipeline.getByRole('button', { name: 'Table' }).click()
    await expect(pipeline.getByRole('columnheader', { name: 'Interview invited' })).toBeVisible()
    await expect(pipeline.getByRole('cell', { name: 'Lanka CloudWorks (Pvt) Ltd' })).toBeVisible()

    await page.getByLabel('Period').selectOption('26')
    await expect(page.getByText('in the last 26 weeks').first()).toBeVisible()
  })

  test('small groups are hidden to protect privacy', async ({ page, signInAs }) => {
    await signInAs('lecturer')
    await page.goto('/analytics')
    await expect(page.getByText('hidden to protect privacy').first()).toBeVisible()
  })

  test('partners cannot open analytics', async ({ page, signInAs }) => {
    await signInAs('partner')
    await page.goto('/analytics')
    await expect(page).toHaveURL(/\/dashboard$/)
  })
})
