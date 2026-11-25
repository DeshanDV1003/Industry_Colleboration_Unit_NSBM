import { Controller, useForm, type UseFormSetError } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { Button } from '@/components/ui/Button'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { FormField } from '@/components/ui/FormField'
import { controlClass } from '@/components/ui/controlClass'
import { WORK_MODES, WORK_MODE_LABEL } from '@/config/opportunities'
import { SkillPicker, useCareerPaths } from '@/features/careers'
import { cn } from '@/lib/cn'
import { vacancyFormSchema, type VacancyFormValues } from '../vacancyForm'

interface VacancyEditorFormProps {
  defaultValues: VacancyFormValues
  submitLabel: string
  submitting: boolean
  onSubmit: (values: VacancyFormValues, setError: UseFormSetError<VacancyFormValues>) => void
  onCancel: () => void
}

export function VacancyEditorForm({ defaultValues, submitLabel, submitting, onSubmit, onCancel }: VacancyEditorFormProps) {
  const { data: paths } = useCareerPaths()
  const { register, control, handleSubmit, setError, setValue, getValues, formState: { errors } } = useForm<VacancyFormValues>({
    resolver: zodResolver(vacancyFormSchema),
    defaultValues,
  })
  const input = (invalid: boolean) => cn(controlClass(invalid), 'h-10')

  // A skill is either required or preferred — choosing it in one list removes it from the other.
  const exclusive = (field: 'requiredSkillIds' | 'preferredSkillIds', other: 'requiredSkillIds' | 'preferredSkillIds') => (ids: number[]) => {
    setValue(field, ids, { shouldValidate: true })
    setValue(other, getValues(other).filter((id) => !ids.includes(id)))
  }

  return (
    <form noValidate onSubmit={handleSubmit((values) => onSubmit(values, setError))} className="space-y-6">
      <Card>
        <CardHeader title="The internship" description="What will the intern do, where and for how long?" />
        <CardBody className="grid gap-4 md:grid-cols-2">
          <FormField label="Title" htmlFor="title" error={errors.title?.message} className="md:col-span-2">
            <input id="title" className={input(!!errors.title)} placeholder="Associate Software Engineer Intern" {...register('title')} />
          </FormField>
          <FormField label="Job role" htmlFor="careerPathId" error={errors.careerPathId?.message}>
            <select id="careerPathId" className={input(!!errors.careerPathId)} {...register('careerPathId', { valueAsNumber: true })}>
              <option value={0}>Choose a role…</option>
              {paths?.map((p) => <option key={p.id} value={p.id}>{p.name}</option>)}
            </select>
          </FormField>
          <FormField label="Work mode" htmlFor="workMode" error={errors.workMode?.message}>
            <select id="workMode" className={input(!!errors.workMode)} {...register('workMode')}>
              {WORK_MODES.map((m) => <option key={m} value={m}>{WORK_MODE_LABEL[m]}</option>)}
            </select>
          </FormField>
          <FormField label="Location" htmlFor="location" error={errors.location?.message}>
            <input id="location" className={input(!!errors.location)} placeholder="Colombo 03" {...register('location')} />
          </FormField>
          <div className="grid grid-cols-2 gap-4">
            <FormField label="Duration (months)" htmlFor="durationMonths" error={errors.durationMonths?.message}>
              <input id="durationMonths" type="number" min={1} max={12} className={input(!!errors.durationMonths)} {...register('durationMonths', { valueAsNumber: true })} />
            </FormField>
            <FormField label="Closing date" htmlFor="closingDate" error={errors.closingDate?.message}>
              <input id="closingDate" type="date" className={input(!!errors.closingDate)} {...register('closingDate')} />
            </FormField>
          </div>
          <FormField label="Description" htmlFor="description" error={errors.description?.message} className="md:col-span-2">
            <textarea id="description" rows={6} className={cn(controlClass(!!errors.description), 'py-2.5')} {...register('description')} />
          </FormField>
        </CardBody>
      </Card>

      <Card>
        <CardHeader title="Required skills" description="Students see these first; the match percentage counts only required skills." />
        <CardBody>
          <Controller control={control} name="requiredSkillIds" render={({ field }) =>
            <SkillPicker value={field.value} onChange={exclusive('requiredSkillIds', 'preferredSkillIds')} />} />
          {errors.requiredSkillIds?.message && <p className="mt-2 text-xs font-medium text-danger">{errors.requiredSkillIds.message}</p>}
        </CardBody>
      </Card>

      <Card>
        <CardHeader title="Preferred skills" description="Nice to have. Optional." />
        <CardBody>
          <Controller control={control} name="preferredSkillIds" render={({ field }) =>
            <SkillPicker value={field.value} onChange={exclusive('preferredSkillIds', 'requiredSkillIds')} />} />
          {errors.preferredSkillIds?.message && <p className="mt-2 text-xs font-medium text-danger">{errors.preferredSkillIds.message}</p>}
        </CardBody>
      </Card>

      <div className="flex justify-end gap-2">
        <Button variant="secondary" onClick={onCancel}>Cancel</Button>
        <Button type="submit" loading={submitting}>{submitLabel}</Button>
      </div>
    </form>
  )
}
