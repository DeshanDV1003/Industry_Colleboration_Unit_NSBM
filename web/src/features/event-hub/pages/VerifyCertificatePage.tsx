import { useParams } from 'react-router-dom'
import { BadgeCheck, Printer, ShieldAlert } from 'lucide-react'
import { Button } from '@/components/ui/Button'
import { Spinner } from '@/components/ui/Spinner'
import { BRAND, COPYRIGHT } from '@/config/brand'
import { useVerifyCertificate } from '../api/eventHubApi'
import { CertificateView } from '../components/CertificateView'

/** Public page (no sign-in): confirms a certificate is genuine and shows it ready to print. */
export default function VerifyCertificatePage() {
  const code = useParams().code ?? ''
  const { data: certificate, isLoading, isError } = useVerifyCertificate(code)

  return (
    <div className="min-h-screen bg-surface-subtle px-4 py-10">
      <div className="mx-auto max-w-3xl space-y-4">
        {isLoading ? <Spinner className="py-20" /> : isError || !certificate ? (
          <div className="rounded-card border border-line bg-surface p-8 text-center shadow-card">
            <ShieldAlert className="mx-auto size-10 text-danger" />
            <h1 className="mt-3 text-lg font-semibold">Certificate not found</h1>
            <p className="mt-1 text-sm text-muted">No certificate with the code <code className="font-mono">{code}</code> was issued by the {BRAND.university} {BRAND.unit}.</p>
          </div>
        ) : (
          <>
            <div className="flex flex-wrap items-center justify-between gap-3 print:hidden">
              <p className="flex items-center gap-2 rounded-full bg-success-soft px-3 py-1.5 text-sm font-semibold text-success">
                <BadgeCheck className="size-4" /> Genuine certificate issued by the {BRAND.unit}
              </p>
              <Button variant="secondary" size="sm" onClick={() => window.print()}><Printer className="size-4" /> Print</Button>
            </div>
            <CertificateView certificate={certificate} />
          </>
        )}
        <p className="text-center text-xs text-muted print:hidden">{COPYRIGHT}</p>
      </div>
    </div>
  )
}
