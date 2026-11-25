import { z } from 'zod'
import type { IndustryTask, IndustryTaskInput } from './types'

/** Client-side mirror of IndustryTaskInputValidator. Regex syntax is validated by the API (.NET regex flavour). */
export const taskFormSchema = z.object({
  title: z.string().trim().min(1, 'Enter a title').max(150),
  summary: z.string().trim().min(1, 'Enter a short summary').max(500),
  instructions: z.string().trim().min(1, 'Explain what the student has to do').max(8000),
  careerPathId: z.number({ message: 'Choose a job role' }).int().positive('Choose a job role'),
  level: z.enum(['Beginner', 'Intermediate', 'Advanced']),
  situation: z.enum(['BugFix', 'Feature', 'Testing', 'CodeReview', 'Deployment', 'Troubleshooting']),
  estimatedMinutes: z.number({ message: 'Enter minutes' }).int().min(5, 'At least 5 minutes').max(480, 'At most 8 hours'),
  starterLanguage: z.string(),
  starterCode: z.string().max(20000),
  skillIds: z.array(z.number()).min(1, 'Pick at least one skill or tool'),
  checks: z.array(z.object({
    description: z.string().trim().min(1, 'Describe what the check looks for').max(300),
    pattern: z.string().min(1, 'Enter a pattern').max(500),
    shouldMatch: z.boolean(),
    hint: z.string().max(500),
  })).min(1, 'Add at least one pattern check'),
  knowledgeQuestions: z.array(z.object({ prompt: z.string().trim().min(1, 'Enter the question').max(500) }))
    .min(1, 'Add at least one follow-up question'),
})

export type TaskFormValues = z.infer<typeof taskFormSchema>

export const emptyTaskForm: TaskFormValues = {
  title: '',
  summary: '',
  instructions: '',
  careerPathId: 0,
  level: 'Beginner',
  situation: 'BugFix',
  estimatedMinutes: 30,
  starterLanguage: 'javascript',
  starterCode: '',
  skillIds: [],
  checks: [{ description: '', pattern: '', shouldMatch: true, hint: '' }],
  knowledgeQuestions: [{ prompt: '' }],
}

export const toFormValues = (task: IndustryTask): TaskFormValues => ({
  title: task.title,
  summary: task.summary,
  instructions: task.instructions,
  careerPathId: task.careerPathId,
  level: task.level,
  situation: task.situation,
  estimatedMinutes: task.estimatedMinutes,
  starterLanguage: task.starterLanguage ?? 'text',
  starterCode: task.starterCode ?? '',
  skillIds: task.skills.map((s) => s.id),
  checks: task.checks.map((c) => ({ description: c.description, pattern: c.pattern, shouldMatch: c.shouldMatch, hint: c.hint ?? '' })),
  knowledgeQuestions: task.knowledgeQuestions.map((q) => ({ prompt: q.prompt })),
})

export const toInput = (values: TaskFormValues): IndustryTaskInput => ({
  ...values,
  starterLanguage: values.starterCode.trim() ? values.starterLanguage : null,
  starterCode: values.starterCode.trim() ? values.starterCode : null,
  checks: values.checks.map((c) => ({ ...c, hint: c.hint.trim() || null })),
  knowledgeQuestions: values.knowledgeQuestions.map((q) => q.prompt),
})
