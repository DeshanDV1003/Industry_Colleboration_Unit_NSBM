import { defineConfig } from '@playwright/test'
import { API_URL, WEB_URL } from './e2e/support/env'

/**
 * End-to-end tests for every portal, run in the installed Microsoft Edge (no browser download).
 * The suite resets the demo data before it starts, so it must run against a demo database, never production.
 * One worker: the journeys share that database and some change it.
 */
export default defineConfig({
  testDir: './e2e',
  globalSetup: './e2e/support/global-setup.ts',
  globalTeardown: './e2e/support/global-teardown.ts',
  fullyParallel: false,
  workers: 1,
  retries: process.env.CI ? 1 : 0,
  timeout: 45_000,
  expect: { timeout: 10_000 },
  reporter: [['list'], ['html', { outputFolder: 'e2e-report', open: 'never' }]],
  use: {
    baseURL: WEB_URL,
    channel: 'msedge',
    viewport: { width: 1440, height: 900 },
    trace: 'retain-on-failure',
    screenshot: 'only-on-failure',
  },
  outputDir: 'e2e-results',
  // Reuses the dev servers when they are already running; otherwise starts them.
  webServer: [
    {
      command: 'dotnet run --project ../src/NSBM.ICU.API --no-launch-profile --urls http://localhost:5300 --environment Development',
      url: `${API_URL}/showcase`,
      reuseExistingServer: true,
      timeout: 180_000,
    },
    {
      command: 'npm run dev',
      url: WEB_URL,
      reuseExistingServer: true,
      timeout: 60_000,
    },
  ],
})
