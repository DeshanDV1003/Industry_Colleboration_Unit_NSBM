import { Navigate } from 'react-router-dom'
import { BRAND } from '@/config/brand'
import { useSession } from '@/stores/authStore'
import { AuthLayout } from '../components/AuthLayout'
import { DemoAccessPanel } from '../components/DemoAccessPanel'
import { LoginForm } from '../components/LoginForm'

const DEMO_MODE = import.meta.env.VITE_DEMO_MODE === 'true'

export default function LoginPage() {
  const session = useSession()
  if (session) return <Navigate to="/dashboard" replace />

  return (
    <AuthLayout title="Welcome back" description={`Sign in to the ${BRAND.unit} platform.`}>
      <LoginForm />
      {DEMO_MODE && (
        <div className="mt-6">
          <DemoAccessPanel />
        </div>
      )}
    </AuthLayout>
  )
}
