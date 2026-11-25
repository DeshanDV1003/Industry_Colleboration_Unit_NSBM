import { test as base, expect, type Browser, type Page } from '@playwright/test'
import { apiContext, signIn, type AccountKey } from './api'

const AUTH_STORAGE_KEY = 'nsbm-icu-auth' // web/src/stores/authStore persist name

/**
 * Signs the page in as a demo account without going through the login form (that form has its own test).
 * The session is written to storage before any app script runs, exactly as the app would store it.
 */
export async function signInAs(page: Page, account: AccountKey) {
  const api = await apiContext()
  const session = await signIn(api, account).finally(() => api.dispose())
  await page.addInitScript(([key, value]) => window.localStorage.setItem(key, value),
    [AUTH_STORAGE_KEY, JSON.stringify({ state: { session }, version: 0 })] as const)
}

/** A second, separate browser session for "the other person" in a cross-portal journey. Close its context when done. */
export async function openAs(browser: Browser, account: AccountKey): Promise<Page> {
  const context = await browser.newContext()
  const page = await context.newPage()
  await signInAs(page, account)
  return page
}

/** Fails the test if the page crashed into React Router's error screen. */
async function expectNoCrash(page: Page) {
  await expect(page.getByText('Unexpected Application Error')).toHaveCount(0)
}

export const test = base.extend<{ signInAs: (account: AccountKey) => Promise<void>; expectNoCrash: () => Promise<void> }>({
  signInAs: async ({ page }, use) => use((account) => signInAs(page, account)),
  expectNoCrash: async ({ page }, use) => use(() => expectNoCrash(page)),
})

export { expect }
