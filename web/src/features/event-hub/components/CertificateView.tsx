import { Award } from 'lucide-react'
import { BRAND } from '@/config/brand'
import { EVENT_FORMAT_META } from '@/config/events'
import { formatRange } from '@/features/calendar'
import { formatDate } from '@/lib/format'
import type { Certificate } from '../types'
import { verifyUrl } from '../verifyUrl'

/** The printable certificate. The same design is used in the student's list and on the public verify page. */
export function CertificateView({ certificate }: { certificate: Certificate }) {
  const format = EVENT_FORMAT_META[certificate.format].label.toLowerCase()
  return (
    <article className="certificate relative overflow-hidden rounded-card border-2 border-primary bg-surface p-8 text-center shadow-card">
      <div aria-hidden className="absolute inset-x-0 top-0 h-2 bg-primary" />
      <img src={BRAND.logo.color} alt={BRAND.university} className="mx-auto h-12" />
      <p className="mt-2 text-xs font-semibold uppercase tracking-[0.2em] text-muted">{BRAND.unit}</p>
      <h2 className="mt-6 font-display text-2xl font-bold text-heading">Certificate of participation</h2>
      <p className="mt-4 text-sm text-muted">This certifies that</p>
      <p className="mt-1 font-display text-3xl font-bold text-primary">{certificate.studentName}</p>
      <p className="mt-3 text-sm">took part in the {format}</p>
      <p className="mt-1 text-lg font-semibold text-heading">{certificate.eventTitle}</p>
      <p className="mt-1 text-sm">
        hosted by <strong>{certificate.companyName}</strong>, {formatRange(certificate.startDate, certificate.endDate)}
        {certificate.teamName && <> · team <strong>{certificate.teamName}</strong></>}
      </p>
      <div className="mt-8 flex flex-wrap items-end justify-between gap-4 border-t border-line pt-4 text-left text-xs text-muted">
        <div>
          <p className="font-semibold text-heading">{BRAND.unitHead.name}</p>
          <p>{BRAND.unitHead.title}</p>
        </div>
        <div className="flex items-center gap-1.5"><Award className="size-4 text-accent" /> Issued {formatDate(certificate.issuedAt)}</div>
        <div className="text-right">
          <p className="font-mono font-semibold text-heading">{certificate.code}</p>
          <p className="break-all">Verify: {verifyUrl(certificate.code)}</p>
        </div>
      </div>
    </article>
  )
}
