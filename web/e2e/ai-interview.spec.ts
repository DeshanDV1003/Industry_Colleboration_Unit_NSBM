import { apiAs } from './support/api'
import { expect, test } from './support/fixtures'

/**
 * Never calls the real AI (that would spend API credit). Without a key the page must explain itself; with one,
 * only the start form is checked.
 */
test('AI interviewer page explains its state and needs consent', async ({ page, signInAs }) => {
  const { isAvailable } = await apiAs<{ isAvailable: boolean }>('student', 'GET', 'me/ai-interviews/availability')
  await signInAs('student')
  await page.goto('/ai-interview')

  if (!isAvailable) {
    await expect(page.getByText('The AI interviewer is not set up yet')).toBeVisible()
    await expect(page.getByRole('link', { name: /Soft-skills practice/ })).toBeVisible()
    return
  }
  const start = page.getByRole('button', { name: 'Start interview' })
  await expect(start).toBeDisabled()
  await page.getByRole('checkbox').check()
  await expect(start).toBeEnabled()
})
