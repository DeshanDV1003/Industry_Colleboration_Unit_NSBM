import { Controller, useForm, type UseFormSetError } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { Button } from '@/components/ui/Button'
import { Card, CardBody, CardHeader } from '@/components/ui/Card'
import { CodeEditor } from '@/components/ui/CodeEditor'
import { FormField } from '@/components/ui/FormField'
import { controlClass } from '@/components/ui/controlClass'
import { LEARNING_STAGES, LEARNING_STAGE_META } from '@/config/learning'
import { STARTER_LANGUAGES, WORK_SITUATIONS, WORK_SITUATION_LABEL } from '@/config/tasks'
import { SkillPicker, useCareerPaths } from '@/features/careers'
import { cn } from '@/lib/cn'
import { taskFormSchema, type TaskFormValues } from '../taskForm'
import { ChecksEditor } from './ChecksEditor'
import { QuestionsEditor } from './QuestionsEditor'

interface TaskEditorFormProps {
  defaultValues: TaskFormValues
  submitLabel: string
  submitting: boolean
  onSubmit: (values: TaskFormValues, setError: UseFormSetError<TaskFormValues>) => void
  onCancel: () => void
}

export function TaskEditorForm({ defaultValues, submitLabel, submitting, onSubmit, onCancel }: TaskEditorFormProps) {
  const { data: paths } = useCareerPaths()
  const { register, control, handleSubmit, setError, formState: { errors } } = useForm<TaskFormValues>({
    resolver: zodResolver(taskFormSchema),
    defaultValues,
  })

  const input = (invalid: boolean) => cn(controlClass(invalid), 'h-10')

  return (
    <form noValidate onSubmit={handleSubmit((values) => onSubmit(values, setError))} className="space-y-6">
      <Card>
        <CardHeader title="The challenge" description="What should the student work on, and in what kind of situation?" />
        <CardBody className="grid gap-4 md:grid-cols-2">
          <FormField label="Title" htmlFor="title" error={errors.title?.message} className="md:col-span-2">
            <input id="title" className={input(!!errors.title)} placeholder="Fix the order total discount bug" {...register('title')} />
          </FormField>
          <FormField label="Summary" htmlFor="summary" error={errors.summary?.message} hint="One or two sentences shown in the task library." className="md:col-span-2">
            <input id="summary" className={input(!!errors.summary)} {...register('summary')} />
          </FormField>
          <FormField label="Job role" htmlFor="careerPathId" error={errors.careerPathId?.message}>
            <select id="careerPathId" className={input(!!errors.careerPathId)} {...register('careerPathId', { valueAsNumber: true })}>
              <option value={0}>Choose a role…</option>
              {paths?.map((p) => <option key={p.id} value={p.id}>{p.name}</option>)}
            </select>
          </FormField>
          <FormField label="Work situation" htmlFor="situation" error={errors.situation?.message}>
            <select id="situation" className={input(!!errors.situation)} {...register('situation')}>
              {WORK_SITUATIONS.map((s) => <option key={s} value={s}>{WORK_SITUATION_LABEL[s]}</option>)}
            </select>
          </FormField>
          <FormField label="Level" htmlFor="level" error={errors.level?.message}>
            <select id="level" className={input(!!errors.level)} {...register('level')}>
              {LEARNING_STAGES.map((s) => <option key={s} value={s}>{LEARNING_STAGE_META[s].label}</option>)}
            </select>
          </FormField>
          <FormField label="Estimated minutes" htmlFor="estimatedMinutes" error={errors.estimatedMinutes?.message}>
            <input id="estimatedMinutes" type="number" min={5} max={480} className={input(!!errors.estimatedMinutes)} {...register('estimatedMinutes', { valueAsNumber: true })} />
          </FormField>
          <FormField label="Instructions" htmlFor="instructions" error={errors.instructions?.message} hint="Describe the scenario, the steps and what 'done' looks like." className="md:col-span-2">
            <textarea id="instructions" rows={8} className={cn(controlClass(!!errors.instructions), 'py-2.5')} {...register('instructions')} />
          </FormField>
        </CardBody>
      </Card>

      <Card>
        <CardHeader title="Skills and tools" description="Link the task to the skills it practises. Students see these on the task." />
        <CardBody>
          <Controller control={control} name="skillIds" render={({ field }) => <SkillPicker value={field.value} onChange={field.onChange} />} />
          {errors.skillIds?.message && <p className="mt-2 text-xs font-medium text-danger">{errors.skillIds.message}</p>}
        </CardBody>
      </Card>

      <Card>
        <CardHeader title="Starter code" description="Optional code or template the student starts from." />
        <CardBody className="space-y-3">
          <FormField label="Language" htmlFor="starterLanguage" className="max-w-xs">
            <select id="starterLanguage" className={input(false)} {...register('starterLanguage')}>
              {STARTER_LANGUAGES.map((l) => <option key={l} value={l}>{l}</option>)}
            </select>
          </FormField>
          <Controller
            control={control}
            name="starterCode"
            render={({ field }) => <CodeEditor value={field.value} onChange={field.onChange} minRows={10} ariaLabel="Starter code" />}
          />
        </CardBody>
      </Card>

      <Card>
        <CardHeader title="Pattern checks" description="Simple text checks run on each submission. They look at the text only — code is never executed." />
        <CardBody><ChecksEditor control={control} register={register} errors={errors} /></CardBody>
      </Card>

      <Card>
        <CardHeader title="Knowledge check" description="Follow-up questions the student answers in their own words after submitting." />
        <CardBody><QuestionsEditor control={control} register={register} errors={errors} /></CardBody>
      </Card>

      <div className="flex justify-end gap-2">
        <Button variant="secondary" onClick={onCancel}>Cancel</Button>
        <Button type="submit" loading={submitting}>{submitLabel}</Button>
      </div>
    </form>
  )
}
