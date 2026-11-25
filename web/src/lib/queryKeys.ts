/**
 * Query-key roots shared across features.
 * Every query about the signed-in user's own data starts with ME, so a change to the user's
 * learning context (e.g. choosing a career path) can refresh all of it with one invalidation.
 */
export const ME = ['me'] as const
export const CAREER_PATHS = ['career-paths'] as const
