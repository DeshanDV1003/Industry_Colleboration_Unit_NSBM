import { useForm, useWatch } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Send } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { Button } from '@/components/ui/Button'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { FormField } from '@/components/ui/FormField'
import { controlClass } from '@/components/ui/controlClass'
import { cn } from '@/lib/cn'
import { applyServerErrors } from '@/lib/formErrors'
import { getErrorMessage } from '@/lib/http'
import { useSendAnnouncement } from '../api/announcementsApi'
import { AUDIENCE_META, MAX_MESSAGE_LENGTH } from '../config'
import { ANNOUNCEMENT_AUDIENCES } from '../types'

const schema = z.object({
  audience: z.enum(ANNOUNCEMENT_AUDIENCES),
  title: z.string().trim().min(1, 'Enter a title.').max(150),
  message: z.string().trim().min(1, 'Enter the message.').max(MAX_MESSAGE_LENGTH),
})
type Values = z.infer<typeof schema>

const EMPTY: Values = { audience: 'Everyone', title: '', message: '' }

/** Sends one announcement; every recipient gets it in their notifications. */
export function AnnouncementComposer() {
  const send = useSendAnnouncement()
  const { register, control, handleSubmit, setError, reset, formState: { errors } } = useForm<Values>({ resolver: zodResolver(schema), defaultValues: EMPTY })
  const [audience, message] = useWatch({ control, name: ['audience', 'message'] })

  const submit = handleSubmit((values) => send.mutate(values, {
    onSuccess: () => { toast.success(`Announcement sent to ${AUDIENCE_META[values.audience].label.toLowerCase()}.`); reset(EMPTY) },
    onError: (e) => { if (!applyServerErrors(e, setError)) toast.error(getErrorMessage(e)) },
  }))

  return (
    <Card>
      <CardHeader title="New announcement" description="Delivered at once to everyone in the audience, in their notifications." />
      <CardBody>
        <form noValidate onSubmit={submit} className="space-y-4">
          <FormField label="Send to" htmlFor="an-audience" hint={AUDIENCE_META[audience].description} error={errors.audience?.message}>
            <select id="an-audience" className={cn(controlClass(!!errors.audience), 'h-10')} {...register('audience')}>
              {ANNOUNCEMENT_AUDIENCES.map((a) => <option key={a} value={a}>{AUDIENCE_META[a].label}</option>)}
            </select>
          </FormField>
          <FormField label="Title" htmlFor="an-title" error={errors.title?.message}>
            <input id="an-title" className={cn(controlClass(!!errors.title), 'h-10')} {...register('title')} />
          </FormField>
          <FormField label="Message" htmlFor="an-message" hint={`${message.length}/${MAX_MESSAGE_LENGTH}`} error={errors.message?.message}>
            <textarea id="an-message" rows={4} maxLength={MAX_MESSAGE_LENGTH} className={cn(controlClass(!!errors.message), 'py-2')} {...register('message')} />
          </FormField>
          <div className="flex justify-end">
            <Button type="submit" loading={send.isPending}><Send className="size-4" /> Send announcement</Button>
          </div>
        </form>
      </CardBody>
    </Card>
  )
}
