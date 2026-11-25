import { resetDemoData } from './api'

/** Leaves the demo the way it was found, so manual demos after a test run look normal. */
export default async function globalTeardown() {
  await resetDemoData()
}
