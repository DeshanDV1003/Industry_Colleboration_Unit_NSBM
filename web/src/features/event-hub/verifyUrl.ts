/** The public page an employer opens to check a certificate. */
export const verifyUrl = (code: string) => `${window.location.origin}/verify/${encodeURIComponent(code)}`
