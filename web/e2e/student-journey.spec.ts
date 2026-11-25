import { resetDemoData } from './support/api'
import { expect, test } from './support/fixtures'

// A first-week student: earlier specs may have given the demo student a career path or baseline.
test.beforeAll(resetDemoData)

/** A first-week student: choose a path, take the baseline, tick the roadmap, try a task, see the streak. */
test('student journey from career path to first industry task', async ({ page, signInAs }) => {
  test.setTimeout(120_000)
  await signInAs('student')

  await test.step('choose the Quality Assurance path', async () => {
    await page.goto('/pathways')
    const card = page.locator('article, div').filter({ has: page.getByRole('heading', { name: 'Quality Assurance' }) }).last()
    await card.getByRole('button', { name: 'Choose this path' }).click()
    // Wait for the save itself; the sidebar always has a baseline link, so that proves nothing.
    await expect(page.getByText('Career path saved.', { exact: false })).toBeVisible()
  })

  await test.step('answer every baseline question', async () => {
    await page.goto('/baseline')
    // Each question is a <fieldset> of radios (role "group").
    const groups = page.getByRole('main').getByRole('group').filter({ has: page.getByRole('radio') })
    await expect(groups.first()).toBeVisible()
    const count = await groups.count()
    expect(count).toBeGreaterThan(0)
    for (let i = 0; i < count; i++) await groups.nth(i).getByRole('radio').first().check()
    await page.getByRole('button', { name: 'Submit answers' }).click()
    await expect(page.getByText(/%/).first()).toBeVisible()
  })

  await test.step('tick the first roadmap step', async () => {
    await page.goto('/roadmap')
    const first = page.getByRole('checkbox', { name: /^Mark ".+" as done$/ }).first()
    await first.click()
    await expect(page.getByRole('checkbox', { name: /as not done$/ }).first()).toBeVisible()
  })

  await test.step('submit an attempt on an industry task', async () => {
    await page.goto('/tasks')
    await page.getByRole('link', { name: /Write test cases|test cases/i }).first().click()
    await expect(page.getByRole('button', { name: 'Submit attempt' })).toBeVisible()
    const editor = page.getByRole('textbox').first()
    await editor.fill('Test case 1: valid login. Steps: enter a valid email and password. Expected result: the dashboard opens.')
    await page.getByRole('button', { name: 'Submit attempt' }).click()
    await expect(page.getByText(/checks passed/).first()).toBeVisible()
  })

  await test.step('the week now counts towards a practice streak', async () => {
    await page.goto('/achievements')
    await expect(page.getByText(/1 week/).first()).toBeVisible()
    await expect(page.getByText('You have practised this week. Nice!')).toBeVisible()
  })
})
