import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { Link, useSearchParams } from 'react-router-dom'
import { z } from 'zod'
import { AlertCircle, CheckCircle2, Lock } from 'lucide-react'
import { Button } from '@/components/ui/Button'
import { Input } from '@/components/ui/Input'
import { LinkButton } from '@/components/ui/LinkButton'
import { PASSWORD_HINT, passwordSchema } from '@/config/passwordPolicy'
import { applyServerErrors } from '@/lib/formErrors'
import { getErrorMessage } from '@/lib/http'
import { useResetForgottenPassword } from '../api/authApi'
import { AuthLayout } from '../components/AuthLayout'

const schema = z.object({ newPassword: passwordSchema, confirmPassword: z.string() })
  .refine((v) => v.newPassword === v.confirmPassword, { path: ['confirmPassword'], message: 'The passwords do not match.' })
type Values = z.infer<typeof schema> & { token?: string }

/** Opened from the e-mailed link (?email=…&token=…): choose a new password once. */
export default function ResetPasswordPage() {
  const [params] = useSearchParams()
  const email = params.get('email') ?? ''
  const token = params.get('token') ?? ''
  const reset = useResetForgottenPassword()
  const { register, handleSubmit, setError, formState: { errors } } = useForm<Values>({
    resolver: zodResolver(schema),
    defaultValues: { newPassword: '', confirmPassword: '' },
  })

  if (!email || !token) {
    return (
      <AuthLayout title="This link is incomplete" description="Open the link from your e-mail again, or ask for a new one.">
        <LinkButton to="/forgot-password" variant="secondary">Ask for a new link</LinkButton>
      </AuthLayout>
    )
  }

  if (reset.isSuccess) {
    return (
      <AuthLayout title="Password changed" description="You can now sign in with your new password.">
        <p className="mb-6 flex items-center gap-2 text-sm text-success"><CheckCircle2 className="size-4" /> The reset link has now stopped working.</p>
        <LinkButton to="/login">Sign in</LinkButton>
      </AuthLayout>
    )
  }

  const submit = handleSubmit(({ newPassword }) => reset.mutate({ email, token, newPassword }, {
    onError: (e) => { applyServerErrors(e, setError) },
  }))
  const linkProblem = errors.token?.message

  return (
    <AuthLayout title="Choose a new password" description={`For ${email}.`}>
      <form noValidate onSubmit={submit} className="space-y-4">
        {(linkProblem || (reset.isError && !errors.newPassword)) && (
          <div role="alert" className="flex items-start gap-2.5 rounded-control bg-danger-soft px-3.5 py-3 text-sm font-medium text-danger">
            <AlertCircle className="mt-0.5 size-4 shrink-0" />
            <span>{linkProblem ?? getErrorMessage(reset.error)} {linkProblem && <Link to="/forgot-password" className="underline">Ask for a new link</Link>}</span>
          </div>
        )}
        <Input label="New password" type="password" autoComplete="new-password" leadingIcon={<Lock className="size-4" />}
          error={errors.newPassword?.message} {...register('newPassword')} />
        <p className="-mt-2 text-xs text-muted">{PASSWORD_HINT}</p>
        <Input label="Confirm new password" type="password" autoComplete="new-password" leadingIcon={<Lock className="size-4" />}
          error={errors.confirmPassword?.message} {...register('confirmPassword')} />
        <Button type="submit" size="lg" className="w-full" loading={reset.isPending}>Change password</Button>
      </form>
    </AuthLayout>
  )
}
