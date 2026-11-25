import { useState } from 'react'
import { Link } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { AlertCircle, Eye, EyeOff, Lock, Mail } from 'lucide-react'
import { Button } from '@/components/ui/Button'
import { Input } from '@/components/ui/Input'
import { getErrorMessage } from '@/lib/http'
import { useLogin } from '../api/authApi'

const schema = z.object({
  email: z.email('Enter a valid email address'),
  password: z.string().min(1, 'Enter your password'),
})

type LoginValues = z.infer<typeof schema>

export function LoginForm() {
  const [showPassword, setShowPassword] = useState(false)
  const login = useLogin()
  const { register, handleSubmit, formState: { errors } } = useForm<LoginValues>({
    resolver: zodResolver(schema),
    defaultValues: { email: '', password: '' },
  })

  return (
    <form noValidate onSubmit={handleSubmit((values) => login.mutate(values))} className="space-y-4">
      {login.isError && (
        <div role="alert" className="flex items-start gap-2.5 rounded-control bg-danger-soft px-3.5 py-3 text-sm font-medium text-danger">
          <AlertCircle className="mt-0.5 size-4 shrink-0" />
          {getErrorMessage(login.error)}
        </div>
      )}

      <Input
        label="Email"
        type="email"
        autoComplete="username"
        placeholder="you@nsbm.ac.lk"
        leadingIcon={<Mail className="size-4" />}
        error={errors.email?.message}
        {...register('email')}
      />

      <Input
        label="Password"
        type={showPassword ? 'text' : 'password'}
        autoComplete="current-password"
        placeholder="Enter your password"
        leadingIcon={<Lock className="size-4" />}
        error={errors.password?.message}
        trailing={
          <button
            type="button"
            onClick={() => setShowPassword((v) => !v)}
            className="grid size-9 place-items-center rounded-lg text-muted hover:bg-surface-subtle hover:text-heading"
            aria-label={showPassword ? 'Hide password' : 'Show password'}
          >
            {showPassword ? <EyeOff className="size-4" /> : <Eye className="size-4" />}
          </button>
        }
        {...register('password')}
      />

      <div className="-mt-1 text-right">
        <Link to="/forgot-password" className="text-sm font-medium">Forgot password?</Link>
      </div>

      <Button type="submit" size="lg" className="w-full" loading={login.isPending}>
        Sign in
      </Button>
    </form>
  )
}
