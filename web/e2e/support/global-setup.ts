import { resetDemoData } from './api'

/** Every run starts from the same seeded demo data. */
export default async function globalSetup() {
  await resetDemoData()
}
