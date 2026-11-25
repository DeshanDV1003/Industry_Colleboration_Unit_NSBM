import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { toast } from '@/components/feedback/toast'
import { Button } from '@/components/ui/Button'
import { Dialog } from '@/components/ui/Dialog'
import { FormField } from '@/components/ui/FormField'
import { controlClass } from '@/components/ui/controlClass'
import { PASSWORD_HINT, passwordSchema } from '@/config/passwordPolicy'
import { cn } from '@/lib/cn'
import { applyServerErrors } from '@/lib/formErrors'
import { getErrorMessage } from '@/lib/http'
import { useChangePassword } from '../api/authApi'

const schema = z.object({
  currentPassword: z.string().min(1, 'Enter your current password.'),
  newPassword: passwordSchema,
  confirmPassword: z.string(),
})
  .refine((v) => v.newPassword === v.confirmPassword, { path: ['confirmPassword'], message: 'The passwords do not match.' })
  .refine((v) => v.newPassword !== v.currentPassword, { path: ['newPassword'], message: 'Choose a password different from the current one.' })

type Values = z.infer<typeof schema>

/** Lets anyone replace their password, e.g. the temporary one the Unit admin gave them. */
export function ChangePasswordDialog({ onClose }: { onClose: () => void }) {
  const change = useChangePassword()
  const { register, handleSubmit, setError, formState: { errors } } = useForm<Values>({
    resolver: zodResolver(schema),
    defaultValues: { currentPassword: '', newPassword: '', confirmPassword: '' },
  })
  const input = (invalid: boolean) => cn(controlClass(invalid), 'h-10')

  const submit = handleSubmit(({ currentPassword, newPassword }) => change.mutate({ currentPassword, newPassword }, {
    onSuccess: () => { toast.success('Password changed. Use it next time you sign in.'); onClose() },
    onError: (e) => { if (!applyServerErrors(e, setError)) toast.error(getErrorMessage(e)) },
  }))

  return (
    <Dialog
      open
      onClose={onClose}
      title="Change password"
      className="max-w-md"
      footer={<>
        <Button variant="secondary" onClick={onClose}>Cancel</Button>
        <Button loading={change.isPending} onClick={submit}>Change password</Button>
      </>}
    >
      <form noValidate onSubmit={submit} className="space-y-4">
        <FormField label="Current password" htmlFor="cp-current" error={errors.currentPassword?.message}>
          <input id="cp-current" type="password" autoComplete="current-password" className={input(!!errors.currentPassword)} {...register('currentPassword')} />
        </FormField>
        <FormField label="New password" htmlFor="cp-new" hint={PASSWORD_HINT} error={errors.newPassword?.message}>
          <input id="cp-new" type="password" autoComplete="new-password" className={input(!!errors.newPassword)} {...register('newPassword')} />
        </FormField>
        <FormField label="Confirm new password" htmlFor="cp-confirm" error={errors.confirmPassword?.message}>
          <input id="cp-confirm" type="password" autoComplete="new-password" className={input(!!errors.confirmPassword)} {...register('confirmPassword')} />
        </FormField>
      </form>
    </Dialog>
  )
}
