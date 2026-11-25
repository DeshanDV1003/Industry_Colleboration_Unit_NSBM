import type { AccountKey } from './support/api'
import { expect, test } from './support/fixtures'

/**
 * Smoke test: every page in every portal's sidebar opens without crashing or showing a load error.
 * Catches broken routes, failing API calls and render errors across the whole app in one pass.
 */
const PORTALS: AccountKey[] = ['student', 'lecturer', 'partner', 'admin', 'registrar']

for (const account of PORTALS) {
  test(`every ${account} page opens cleanly`, async ({ page, signInAs, expectNoCrash }) => {
    test.setTimeout(120_000)
    const consoleErrors: string[] = []
    page.on('console', (m) => { if (m.type() === 'error' && !/camera|getUserMedia/i.test(m.text())) consoleErrors.push(m.text()) })

    await signInAs(account)
    await page.goto('/dashboard')
    const links = await page.getByRole('navigation').getByRole('link').evaluateAll((els) =>
      [...new Set(els.map((e) => e.getAttribute('href')).filter((h): h is string => !!h && h.startsWith('/')))])
    expect(links.length).toBeGreaterThan(3)

    for (const href of links) {
      await page.goto(href)
      await expect(page.locator('main h1, main [role="status"]').first(), `${href} rendered`).toBeVisible()
      await expectNoCrash()
      await expect(page.getByText(/could not be loaded/i), `${href} loaded its data`).toHaveCount(0)
    }
    expect(consoleErrors, 'no console errors').toEqual([])
  })
}
