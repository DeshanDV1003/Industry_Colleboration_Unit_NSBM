import { useFieldArray, type Control, type FieldErrors, type UseFormRegister } from 'react-hook-form'
import { Plus, Trash2 } from 'lucide-react'
import { Button } from '@/components/ui/Button'
import { controlClass } from '@/components/ui/controlClass'
import type { TaskFormValues } from '../taskForm'

interface QuestionsEditorProps {
  control: Control<TaskFormValues>
  register: UseFormRegister<TaskFormValues>
  errors: FieldErrors<TaskFormValues>
}

export function QuestionsEditor({ control, register, errors }: QuestionsEditorProps) {
  const { fields, append, remove } = useFieldArray({ control, name: 'knowledgeQuestions' })

  return (
    <div className="space-y-2">
      {fields.map((field, index) => {
        const message = errors.knowledgeQuestions?.[index]?.prompt?.message
        return (
          <div key={field.id}>
            <div className="flex items-center gap-2">
              <span className="w-6 text-sm font-semibold text-muted">{index + 1}.</span>
              <input
                aria-label={`Question ${index + 1}`}
                className={`${controlClass(!!message)} h-10`}
                placeholder="In your own words, what caused the bug?"
                {...register(`knowledgeQuestions.${index}.prompt`)}
              />
              <Button variant="ghost" size="sm" disabled={fields.length === 1} onClick={() => remove(index)} aria-label={`Remove question ${index + 1}`}>
                <Trash2 className="size-4" />
              </Button>
            </div>
            {message && <p className="ml-8 mt-1 text-xs font-medium text-danger">{message}</p>}
          </div>
        )
      })}
      {errors.knowledgeQuestions?.root?.message && <p className="text-xs font-medium text-danger">{errors.knowledgeQuestions.root.message}</p>}
      <Button variant="secondary" size="sm" onClick={() => append({ prompt: '' })}>
        <Plus className="size-4" /> Add question
      </Button>
    </div>
  )
}
