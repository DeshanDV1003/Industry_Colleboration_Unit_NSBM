import { Award, Link2, Printer } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { PageHeader } from '@/components/layout/PageHeader'
import { Button } from '@/components/ui/Button'
import { EmptyState } from '@/components/ui/EmptyState'
import { LinkButton } from '@/components/ui/LinkButton'
import { PageSpinner } from '@/components/ui/Spinner'
import { getErrorMessage } from '@/lib/http'
import { useMyCertificates } from '../api/eventHubApi'
import { CertificateView } from '../components/CertificateView'
import { verifyUrl } from '../verifyUrl'

/** The student's certificates, each with a public verification link to share with employers. */
export default function CertificatesPage() {
  const { data: certificates, isLoading, isError, error } = useMyCertificates()

  if (isLoading) return <PageSpinner label="Loading certificates…" />
  if (isError || !certificates) return <EmptyState icon={Award} title="Certificates could not be loaded" description={getErrorMessage(error)} />

  const copyLink = (code: string) =>
    navigator.clipboard.writeText(verifyUrl(code)).then(() => toast.success('Verification link copied. Add it to your CV or LinkedIn.'))

  return (
    <>
      <PageHeader
        title="My certificates"
        description="Certificates from partner events you took part in. Anyone with the link can check that a certificate is genuine."
      />
      {certificates.length === 0 ? (
        <EmptyState icon={Award} title="No certificates yet" description="Take part in a partner event. The host issues certificates after it ends."
          action={<LinkButton to="/events">Browse events</LinkButton>} />
      ) : (
        <div className="space-y-8">
          {certificates.map((c) => (
            <section key={c.code} className="max-w-3xl">
              <CertificateView certificate={c} />
              <div className="mt-3 flex gap-2 print:hidden">
                <Button variant="secondary" size="sm" onClick={() => copyLink(c.code)}><Link2 className="size-4" /> Copy verification link</Button>
                <LinkButton to={`/verify/${c.code}`} variant="secondary" target="_blank"><Printer className="size-4" /> Open printable view</LinkButton>
              </div>
            </section>
          ))}
        </div>
      )}
    </>
  )
}
