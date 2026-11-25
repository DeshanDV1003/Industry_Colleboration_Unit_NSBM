import { z } from 'zod'

/** Mirrors Application/Common/Validation/PasswordRules and the Identity options on the API. */
export const PASSWORD_MIN_LENGTH = 8

export const passwordSchema = z.string()
  .min(PASSWORD_MIN_LENGTH, `Use at least ${PASSWORD_MIN_LENGTH} characters.`)
  .regex(/[0-9]/, 'Include at least one number.')
  .max(100)

export const PASSWORD_HINT = `At least ${PASSWORD_MIN_LENGTH} characters, including a number.`
