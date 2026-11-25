/** Where the suite finds the app. Override with E2E_WEB_URL / E2E_API_URL to test another demo deployment. */
export const WEB_URL = process.env.E2E_WEB_URL ?? 'http://localhost:5301'
export const API_URL = process.env.E2E_API_URL ?? 'http://localhost:5300/api/v1'
