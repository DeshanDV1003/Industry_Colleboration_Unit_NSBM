/** Mirrors Features/EmailPreferences/Dtos/EmailPreferencesDto. */
export interface EmailPreferences {
  notificationsByEmail: boolean
  weeklyDigestByEmail: boolean
  /** False when this server does not send e-mail at all. */
  emailEnabled: boolean
}

export type EmailPreferencesInput = Omit<EmailPreferences, 'emailEnabled'>
