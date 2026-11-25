import { useForm, useWatch } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Copy, Download, NotebookPen } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { PageHeader } from '@/components/layout/PageHeader'
import { Button } from '@/components/ui/Button'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { FormField } from '@/components/ui/FormField'
import { PageSpinner } from '@/components/ui/Spinner'
import { controlClass } from '@/components/ui/controlClass'
import { downloadText } from '@/lib/download'
import { MySkillsCard } from '@/features/skills-profile'
import { cn } from '@/lib/cn'
import { applyServerErrors } from '@/lib/formErrors'
import { getErrorMessage } from '@/lib/http'
import { useMyCv, useSaveCv, type Cv } from '../api/cvApi'
import { CvKeywordFinder } from '../components/CvKeywordFinder'
import { buildCvText } from '../cvText'

const optionalUrl = (host: string) => z.string().trim().refine(
  (v) => v === '' || (v.startsWith('https://') && new URL(v).hostname.endsWith(host)), `Enter a full https link on ${host}`)

const schema = z.object({
  headline: z.string().max(150),
  summary: z.string().max(2000),
  education: z.string().max(2000),
  projects: z.string().max(4000),
  gitHubUrl: optionalUrl('github.com'),
  linkedInUrl: optionalUrl('linkedin.com'),
})
type CvValues = z.infer<typeof schema>

function CvForm({ cv }: { cv: Cv }) {
  const save = useSaveCv()
  const { register, handleSubmit, setError, control, formState: { errors, isDirty } } = useForm<CvValues>({
    resolver: zodResolver(schema),
    defaultValues: { ...cv, gitHubUrl: cv.gitHubUrl ?? '', linkedInUrl: cv.linkedInUrl ?? '' },
  })
  const values = useWatch({ control })
  const preview = buildCvText({ ...cv, ...values, gitHubUrl: values.gitHubUrl || null, linkedInUrl: values.linkedInUrl || null })
  const field = (invalid: boolean, area = false) => cn(controlClass(invalid), area ? 'py-2.5' : 'h-10')

  const onSubmit = (values: CvValues) =>
    save.mutate({ ...values, gitHubUrl: values.gitHubUrl || null, linkedInUrl: values.linkedInUrl || null }, {
      onSuccess: () => toast.success('CV saved.'),
      onError: (e) => { if (!applyServerErrors(e, setError)) toast.error(getErrorMessage(e)) },
    })

  return (
    <div className="grid gap-6 2xl:grid-cols-2">
      <Card>
        <CardHeader title="Edit your CV" description="Plain, honest and specific works best." />
        <CardBody>
          <form noValidate onSubmit={handleSubmit(onSubmit)} className="grid gap-4 md:grid-cols-2">
            <FormField label="Headline" htmlFor="headline" error={errors.headline?.message} className="md:col-span-2">
              <input id="headline" className={field(!!errors.headline)} placeholder="Aspiring QA engineer" {...register('headline')} />
            </FormField>
            <FormField label="GitHub" htmlFor="gitHubUrl" error={errors.gitHubUrl?.message}>
              <input id="gitHubUrl" className={field(!!errors.gitHubUrl)} placeholder="https://github.com/you" {...register('gitHubUrl')} />
            </FormField>
            <FormField label="LinkedIn" htmlFor="linkedInUrl" error={errors.linkedInUrl?.message}>
              <input id="linkedInUrl" className={field(!!errors.linkedInUrl)} placeholder="https://www.linkedin.com/in/you" {...register('linkedInUrl')} />
            </FormField>
            <FormField label="Summary" htmlFor="summary" error={errors.summary?.message} className="md:col-span-2">
              <textarea id="summary" rows={3} className={field(!!errors.summary, true)} {...register('summary')} />
            </FormField>
            <FormField label="Education" htmlFor="education" error={errors.education?.message} className="md:col-span-2">
              <textarea id="education" rows={3} className={field(!!errors.education, true)} {...register('education')} />
            </FormField>
            <FormField label="Projects" htmlFor="projects" error={errors.projects?.message} className="md:col-span-2">
              <textarea id="projects" rows={5} className={field(!!errors.projects, true)} {...register('projects')} />
            </FormField>
            <div className="md:col-span-2 flex justify-end">
              <Button type="submit" loading={save.isPending} disabled={!isDirty}>Save CV</Button>
            </div>
          </form>
        </CardBody>
      </Card>

      <Card>
        <CardHeader
          title="CV outline"
          description="Skills come from My skills. Copy or download it as text."
          action={<div className="flex gap-1">
            <Button size="sm" variant="ghost" onClick={() => navigator.clipboard.writeText(preview).then(() => toast.success('CV copied.'))}><Copy className="size-4" /> Copy</Button>
            <Button size="sm" variant="ghost" onClick={() => downloadText(`cv-${cv.studentNo}.txt`, preview)}><Download className="size-4" /> Download</Button>
          </div>}
        />
        <CardBody>
          <pre className="whitespace-pre-wrap rounded-xl bg-surface-subtle p-4 font-sans text-sm text-heading">{preview}</pre>
        </CardBody>
      </Card>
    </div>
  )
}

export default function CvStudioPage() {
  const { data: cv, isLoading, isError, error } = useMyCv()

  if (isLoading) return <PageSpinner label="Loading your CV…" />
  if (isError || !cv) return <EmptyState icon={NotebookPen} title="CV could not be loaded" description={getErrorMessage(error)} />

  return (
    <>
      <PageHeader title="CV and skills studio" description="Build an editable CV outline, list your skills and check an old CV for known skills." />
      <div className="grid gap-6 xl:grid-cols-[minmax(0,1fr)_340px]">
        <CvForm key={cv.headline + cv.summary} cv={cv} />
        <aside className="space-y-6">
          <MySkillsCard />
          <CvKeywordFinder />
        </aside>
      </div>
    </>
  )
}
