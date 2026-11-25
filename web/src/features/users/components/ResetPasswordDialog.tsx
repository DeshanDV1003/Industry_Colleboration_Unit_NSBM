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
import { useResetPassword } from '../api/usersApi'
import type { UserSummary } from '../types'

const schema = z.object({ newPassword: passwordSchema })
type Values = z.infer<typeof schema>

/** The admin sets a temporary password for someone who cannot sign in. */
export function ResetPasswordDialog({ user, onClose }: { user: UserSummary; onClose: () => void }) {
  const reset = useResetPassword()
  const { register, handleSubmit, setError, formState: { errors } } = useForm<Values>({ resolver: zodResolver(schema), defaultValues: { newPassword: '' } })

  const submit = handleSubmit(({ newPassword }) => reset.mutate({ userId: user.userId, newPassword }, {
    onSuccess: () => { toast.success(`Password reset. Give ${user.fullName} the temporary password.`); onClose() },
    onError: (e) => { if (!applyServerErrors(e, setError)) toast.error(getErrorMessage(e)) },
  }))

  return (
    <Dialog
      open
      onClose={onClose}
      title={`Reset password for ${user.fullName}`}
      description="Their current password stops working. They can change the temporary one after signing in."
      className="max-w-md"
      footer={<>
        <Button variant="secondary" onClick={onClose}>Cancel</Button>
        <Button loading={reset.isPending} onClick={submit}>Reset password</Button>
      </>}
    >
      <form noValidate onSubmit={submit}>
        <FormField label="Temporary password" htmlFor="rp-new" hint={PASSWORD_HINT} error={errors.newPassword?.message}>
          <input id="rp-new" autoComplete="new-password" className={cn(controlClass(!!errors.newPassword), 'h-10')} {...register('newPassword')} />
        </FormField>
      </form>
    </Dialog>
  )
}
