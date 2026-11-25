import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { Link } from 'react-router-dom'
import { z } from 'zod'
import { AlertCircle, ArrowLeft, Mail, MailCheck } from 'lucide-react'
import { Button } from '@/components/ui/Button'
import { Input } from '@/components/ui/Input'
import { Spinner } from '@/components/ui/Spinner'
import { getErrorMessage } from '@/lib/http'
import { useForgotPassword, usePasswordResetAvailability } from '../api/authApi'
import { AuthLayout } from '../components/AuthLayout'

const schema = z.object({ email: z.email('Enter a valid email address') })
type Values = z.infer<typeof schema>

const BackToSignIn = () => (
  <Link to="/login" className="mt-6 inline-flex items-center gap-1.5 text-sm font-medium"><ArrowLeft className="size-4" /> Back to sign in</Link>
)

/** Asks for a reset link. The answer is the same whether or not the address has an account. */
export default function ForgotPasswordPage() {
  const availability = usePasswordResetAvailability()
  const forgot = useForgotPassword()
  const { register, handleSubmit, getValues, formState: { errors } } = useForm<Values>({ resolver: zodResolver(schema), defaultValues: { email: '' } })

  if (availability.isLoading) return <AuthLayout title="Forgot your password?" description=""><Spinner /></AuthLayout>

  if (!availability.data?.available) {
    return (
      <AuthLayout title="Forgot your password?" description="Password reset by e-mail is not set up on this server.">
        <p className="text-sm text-body">Ask the Industry Collaboration Unit to reset it. They will give you a temporary password, which you can then change from your user menu.</p>
        <BackToSignIn />
      </AuthLayout>
    )
  }

  if (forgot.isSuccess) {
    return (
      <AuthLayout title="Check your e-mail" description={`If an account uses ${getValues('email')}, we have sent it a link to choose a new password.`}>
        <p className="flex items-start gap-2.5 rounded-control bg-success-soft px-3.5 py-3 text-sm text-success">
          <MailCheck className="mt-0.5 size-4 shrink-0" /> The link works once, for {availability.data.linkValidMinutes} minutes. Check your spam folder if it does not arrive.
        </p>
        <BackToSignIn />
      </AuthLayout>
    )
  }

  return (
    <AuthLayout title="Forgot your password?" description="Enter your e-mail and we will send you a link to choose a new one.">
      <form noValidate onSubmit={handleSubmit((values) => forgot.mutate(values.email))} className="space-y-4">
        {forgot.isError && (
          <div role="alert" className="flex items-start gap-2.5 rounded-control bg-danger-soft px-3.5 py-3 text-sm font-medium text-danger">
            <AlertCircle className="mt-0.5 size-4 shrink-0" /> {getErrorMessage(forgot.error)}
          </div>
        )}
        <Input label="Email" type="email" autoComplete="username" placeholder="you@nsbm.ac.lk" leadingIcon={<Mail className="size-4" />}
          error={errors.email?.message} {...register('email')} />
        <Button type="submit" size="lg" className="w-full" loading={forgot.isPending}>Send reset link</Button>
      </form>
      <BackToSignIn />
    </AuthLayout>
  )
}
