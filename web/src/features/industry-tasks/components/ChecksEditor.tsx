import { useFieldArray, type Control, type FieldErrors, type UseFormRegister } from 'react-hook-form'
import { Plus, Trash2 } from 'lucide-react'
import { Button } from '@/components/ui/Button'
import { FormField } from '@/components/ui/FormField'
import { controlClass } from '@/components/ui/controlClass'
import type { TaskFormValues } from '../taskForm'

interface ChecksEditorProps {
  control: Control<TaskFormValues>
  register: UseFormRegister<TaskFormValues>
  errors: FieldErrors<TaskFormValues>
}

export function ChecksEditor({ control, register, errors }: ChecksEditorProps) {
  const { fields, append, remove } = useFieldArray({ control, name: 'checks' })

  return (
    <div className="space-y-3">
      {fields.map((field, index) => {
        const e = errors.checks?.[index]
        return (
          <div key={field.id} className="rounded-xl border border-line p-4">
            <div className="mb-3 flex items-center justify-between">
              <span className="text-xs font-bold text-muted">CHECK {index + 1}</span>
              <Button variant="ghost" size="sm" disabled={fields.length === 1} onClick={() => remove(index)} aria-label={`Remove check ${index + 1}`}>
                <Trash2 className="size-4" />
              </Button>
            </div>
            <div className="grid gap-3 md:grid-cols-2">
              <FormField label="What it checks" htmlFor={`checks.${index}.description`} error={e?.description?.message}>
                <input id={`checks.${index}.description`} className={`${controlClass(!!e?.description)} h-10`} placeholder="Treats the discount as a percentage" {...register(`checks.${index}.description`)} />
              </FormField>
              <FormField label="Pattern (.NET regular expression)" htmlFor={`checks.${index}.pattern`} error={e?.pattern?.message}>
                <input id={`checks.${index}.pattern`} className={`${controlClass(!!e?.pattern)} h-10 font-mono`} placeholder={'/\\s*100'} {...register(`checks.${index}.pattern`)} />
              </FormField>
              <FormField label="Hint shown when it fails" htmlFor={`checks.${index}.hint`} error={e?.hint?.message} className="md:col-span-2">
                <input id={`checks.${index}.hint`} className={`${controlClass(!!e?.hint)} h-10`} placeholder="Optional" {...register(`checks.${index}.hint`)} />
              </FormField>
            </div>
            <label className="mt-3 flex items-center gap-2 text-sm">
              <input type="checkbox" className="size-4 accent-[var(--color-primary)]" {...register(`checks.${index}.shouldMatch`)} />
              The pattern <strong>must appear</strong> in the solution (untick if it must <strong>not</strong> appear)
            </label>
          </div>
        )
      })}
      {errors.checks?.root?.message && <p className="text-xs font-medium text-danger">{errors.checks.root.message}</p>}
      <Button variant="secondary" size="sm" onClick={() => append({ description: '', pattern: '', shouldMatch: true, hint: '' })}>
        <Plus className="size-4" /> Add check
      </Button>
    </div>
  )
}
