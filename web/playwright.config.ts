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
  timeout: 60_000,
  // Generous: the Vite dev server compiles each route on first visit.
  expect: { timeout: 15_000 },
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
      // The "http" launch profile sets the Development environment and port 5300.
      command: 'dotnet run --project ../src/NSBM.ICU.API --launch-profile http',
      url: `${API_URL}/showcase`,
      reuseExistingServer: true,
      timeout: 180_000,
    },
    {
      // A production build: the dev server re-bundles dependencies on first visits and reloads mid-test.
      // Preview reuses the dev proxy settings, so /api still reaches the API.
      command: 'npm run build && npx vite preview --port 5301 --strictPort',
      url: WEB_URL,
      reuseExistingServer: true,
      timeout: 180_000,
    },
  ],
})
