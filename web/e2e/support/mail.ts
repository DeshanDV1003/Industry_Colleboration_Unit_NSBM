import { readdirSync, readFileSync, statSync } from 'node:fs'
import path from 'node:path'

/** The development inbox: the API (Email:Mode=File) writes each e-mail here as an .eml file. Tests run from web/. */
const MAIL_DIR = path.resolve(process.cwd(), '../src/NSBM.ICU.API/App_Data/mail')

/** Decodes quoted-printable text (soft line breaks and =XX bytes) when the message says it uses it; plain 8-bit text is left alone. */
const decode = (source: string) => !/Content-Transfer-Encoding:\s*quoted-printable/i.test(source) ? source : source
  .replace(/=\r?\n/g, '').replace(/=([0-9A-F]{2})/gi, (_, hex: string) => String.fromCharCode(parseInt(hex, 16)))

/** Waits for the newest e-mail to an address that arrived after `since` (ms), and returns its decoded source. */
export async function waitForEmail(to: string, since: number, timeoutMs = 30_000): Promise<string> {
  const deadline = Date.now() + timeoutMs
  while (Date.now() < deadline) {
    const files = safeList().filter((f) => statSync(f).mtimeMs >= since).sort((a, b) => statSync(b).mtimeMs - statSync(a).mtimeMs)
    for (const file of files) {
      const source = decode(readFileSync(file, 'utf8'))
      if (new RegExp(`^To:.*${to.replace(/[.@]/g, '\\$&')}`, 'im').test(source)) return source
    }
    await new Promise((resolve) => setTimeout(resolve, 1000))
  }
  throw new Error(`No e-mail to ${to} arrived in ${timeoutMs / 1000}s (folder: ${MAIL_DIR}).`)
}

function safeList(): string[] {
  try {
    return readdirSync(MAIL_DIR).filter((f) => f.endsWith('.eml')).map((f) => path.join(MAIL_DIR, f))
  } catch {
    return []
  }
}
