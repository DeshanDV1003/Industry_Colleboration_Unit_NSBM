import { apiAs } from './support/api'
import { expect, openAs, test } from './support/fixtures'

interface Notifications { items: { kind: string; title: string }[] }

/** The Assistant Registrar allocates halls; the admin notifies everyone. Each result is checked in another portal. */
test.describe('Halls and announcements', () => {
  test('registrar allocates a lecture hall and the cohort is notified', async ({ page, signInAs }) => {
    await signInAs('registrar')
    await page.goto('/hall-bookings')
    await expect(page.getByRole('heading', { name: 'Hall allocations' })).toBeVisible()

    await page.getByRole('button', { name: 'Allocate hall' }).first().click()
    const dialog = page.getByRole('dialog')
    await dialog.getByLabel('Module or lecture').fill('E2E Networking Lab')
    await dialog.getByLabel('Cohort').selectOption({ label: 'BSc (Hons) Computer Networks · Year 4 (1 student)' })
    await dialog.getByLabel('Venue').selectOption({ label: 'LH-101 · Lecture Hall 101 — Lecture hall, seats 120' })
    await dialog.getByLabel('From').fill('17:00')
    await dialog.getByLabel('To').fill('18:00')
    await dialog.getByRole('button', { name: 'Allocate hall' }).click()
    await expect(page.getByText('Hall allocated. Everyone affected was notified.')).toBeVisible()

    const amaya = await apiAs<Notifications>('amaya', 'GET', 'me/notifications')
    expect(amaya.items.some((n) => n.kind === 'HallAssigned' && n.title.includes('E2E Networking Lab'))).toBe(true)
  })

  test('a clash is explained instead of saved', async ({ page, signInAs }) => {
    await signInAs('registrar')
    await page.goto('/hall-bookings')
    await page.getByRole('button', { name: 'Allocate hall' }).first().click()
    const dialog = page.getByRole('dialog')
    await dialog.getByLabel('Module or lecture').fill('E2E clash')
    await dialog.getByLabel('Cohort').selectOption({ label: 'BSc (Hons) Computer Networks · Year 4 (1 student)' })
    await dialog.getByLabel('Venue').selectOption({ label: 'AUD-01 · Main Auditorium — Auditorium, seats 500' })
    await dialog.getByLabel('Repeat weekly for').fill('2')
    await dialog.getByRole('button', { name: 'Allocate hall' }).click()
    await expect(page.getByText('Hall allocated for 2 weeks.', { exact: false })).toBeVisible()

    await page.getByRole('button', { name: 'Allocate hall' }).first().click()
    await dialog.getByLabel('Module or lecture').fill('E2E clash again')
    await dialog.getByLabel('Cohort').selectOption({ label: 'BSc (Hons) Computer Networks · Year 4 (1 student)' })
    await dialog.getByLabel('Venue').selectOption({ label: 'AUD-01 · Main Auditorium — Auditorium, seats 500' })
    await dialog.getByRole('button', { name: 'Allocate hall' }).click()
    await expect(page.getByText(/already booked|cohort already has/)).toBeVisible()
  })

  test('admin follows allocations read-only', async ({ browser }) => {
    const admin = await openAs(browser, 'admin')
    await admin.goto('/hall-bookings')
    await expect(admin.getByRole('heading', { name: 'Hall allocations' })).toBeVisible()
    await expect(admin.getByRole('button', { name: 'Allocate hall' })).toHaveCount(0)
    await admin.context().close()
  })

  test('admin announcement reaches every portal', async ({ page, signInAs }) => {
    await signInAs('admin')
    await page.goto('/announcements')
    await page.getByLabel('Title').fill('E2E campus notice')
    await page.getByLabel('Message').fill('The library opens late on Monday.')
    await page.getByRole('button', { name: 'Send announcement' }).click()
    await expect(page.getByText('Announcement sent to everyone.')).toBeVisible()
    await expect(page.getByText('E2E campus notice')).toBeVisible()

    for (const account of ['student', 'lecturer', 'partner', 'registrar'] as const) {
      const notes = await apiAs<Notifications>(account, 'GET', 'me/notifications')
      expect(notes.items.some((n) => n.kind === 'Announcement' && n.title === 'E2E campus notice'), account).toBe(true)
    }
  })
})
