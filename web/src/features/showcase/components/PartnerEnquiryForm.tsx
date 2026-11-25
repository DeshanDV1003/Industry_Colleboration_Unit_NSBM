import { useState } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { CheckCircle2, Send } from 'lucide-react'
import { Button } from '@/components/ui/Button'
import { FormField } from '@/components/ui/FormField'
import { controlClass } from '@/components/ui/controlClass'
import { cn } from '@/lib/cn'
import { applyServerErrors } from '@/lib/formErrors'
import { getErrorMessage } from '@/lib/http'
import { useSubmitPartnerEnquiry } from '../api/showcaseApi'
import { PARTNER_INTEREST_LABEL } from '../config'
import { PARTNER_INTERESTS } from '../types'

const schema = z.object({
  organisationName: z.string().trim().min(1, 'Enter your organisation.').max(150),
  contactName: z.string().trim().min(1, 'Enter your name.').max(120),
  email: z.string().trim().email('Enter a valid email address.').max(200),
  phone: z.string().max(30).regex(/^[0-9+()\-\s]*$/, 'Use digits, spaces and + ( ) - only.'),
  interest: z.enum(PARTNER_INTERESTS),
  message: z.string().trim().min(20, 'Tell us a little more (at least 20 characters).').max(2000),
  website: z.string(),
})
type Values = z.infer<typeof schema>

/** "Become a partner" form. The hidden website field is a honeypot that real visitors never fill in. */
export function PartnerEnquiryForm() {
  const submit = useSubmitPartnerEnquiry()
  const [sent, setSent] = useState(false)
  const [failure, setFailure] = useState<string | null>(null)
  const { register, handleSubmit, setError, reset, formState: { errors } } = useForm<Values>({
    resolver: zodResolver(schema),
    defaultValues: { organisationName: '', contactName: '', email: '', phone: '', interest: 'IndustryTasks', message: '', website: '' },
  })
  const input = (invalid: boolean) => cn(controlClass(invalid), 'h-10')

  if (sent) {
    return (
      <div className="rounded-card border border-success bg-success-soft p-6 text-sm">
        <p className="flex items-center gap-2 font-semibold text-success"><CheckCircle2 className="size-5" /> Thank you. Your enquiry has reached the Industry Collaboration Unit.</p>
        <p className="mt-1 text-heading">We will reply by email. <button type="button" className="font-semibold text-primary underline" onClick={() => { reset(); setSent(false) }}>Send another</button></p>
      </div>
    )
  }

  return (
    <form noValidate className="grid gap-4 rounded-card border border-line bg-surface p-6 shadow-card md:grid-cols-2"
      onSubmit={handleSubmit((v) => {
        setFailure(null)
        submit.mutate({ ...v, phone: v.phone.trim() || null }, {
          onSuccess: () => setSent(true),
          onError: (e) => { if (!applyServerErrors(e, setError)) setFailure(getErrorMessage(e)) },
        })
      })}>
      <FormField label="Organisation" htmlFor="pe-org" error={errors.organisationName?.message}>
        <input id="pe-org" autoComplete="organization" className={input(!!errors.organisationName)} {...register('organisationName')} />
      </FormField>
      <FormField label="Your name" htmlFor="pe-name" error={errors.contactName?.message}>
        <input id="pe-name" autoComplete="name" className={input(!!errors.contactName)} {...register('contactName')} />
      </FormField>
      <FormField label="Work email" htmlFor="pe-email" error={errors.email?.message}>
        <input id="pe-email" type="email" autoComplete="email" className={input(!!errors.email)} {...register('email')} />
      </FormField>
      <FormField label="Phone (optional)" htmlFor="pe-phone" error={errors.phone?.message}>
        <input id="pe-phone" type="tel" autoComplete="tel" className={input(!!errors.phone)} {...register('phone')} />
      </FormField>
      <FormField label="I'm interested in" htmlFor="pe-interest" className="md:col-span-2">
        <select id="pe-interest" className={input(false)} {...register('interest')}>
          {PARTNER_INTERESTS.map((i) => <option key={i} value={i}>{PARTNER_INTEREST_LABEL[i]}</option>)}
        </select>
      </FormField>
      <FormField label="Message" htmlFor="pe-message" error={errors.message?.message} className="md:col-span-2">
        <textarea id="pe-message" rows={4} className={cn(controlClass(!!errors.message), 'py-2')} {...register('message')} />
      </FormField>
      <div aria-hidden className="absolute -left-[9999px] h-0 w-0 overflow-hidden">
        <label htmlFor="pe-website">Website</label>
        <input id="pe-website" tabIndex={-1} autoComplete="off" {...register('website')} />
      </div>
      <div className="flex flex-wrap items-center justify-between gap-3 md:col-span-2">
        <p className="text-xs text-muted">We use these details only to reply to your enquiry.</p>
        <Button type="submit" loading={submit.isPending}><Send className="size-4" /> Send enquiry</Button>
      </div>
      {failure && <p role="alert" className="text-sm text-danger md:col-span-2">{failure}</p>}
    </form>
  )
}
