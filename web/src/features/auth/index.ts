// Public surface of the auth feature. Code outside the feature imports from here only.
export { useCurrentUser, useSignOut } from './api/authApi'
export { ChangePasswordDialog } from './components/ChangePasswordDialog'
export { DemoRoleSwitcher } from './components/DemoRoleSwitcher'
export type { CurrentUser } from './types'
