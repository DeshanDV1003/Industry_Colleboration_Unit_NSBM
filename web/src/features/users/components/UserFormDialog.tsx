import { useForm, useWatch } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { toast } from '@/components/feedback/toast'
import { Button } from '@/components/ui/Button'
import { Dialog } from '@/components/ui/Dialog'
import { FormField } from '@/components/ui/FormField'
import { Spinner } from '@/components/ui/Spinner'
import { controlClass } from '@/components/ui/controlClass'
import { PASSWORD_HINT } from '@/config/passwordPolicy'
import { ROLES, ROLE_META, type Role } from '@/config/roles'
import { cn } from '@/lib/cn'
import { applyServerErrors } from '@/lib/formErrors'
import { getErrorMessage } from '@/lib/http'
import { useCreateUser, useUpdateUser, useUser, useUserOptions } from '../api/usersApi'
import type { UserOptions } from '../types'
import { FIELD_LABEL, FIELD_PLACEHOLDER, ROLE_FIELDS, editUserForm, newUserForm, toProfile, userFormSchema, type ProfileField, type UserFormValues } from '../userForm'

type UserFormDialogProps = { onClose: () => void } & ({ userId: string } | { userId: null; initialRole: Role })

/** Opens an account for any role, or edits one. Mounts the form once the account (when editing) and options are loaded. */
export function UserFormDialog(props: UserFormDialogProps) {
  const user = useUser(props.userId)
  const options = useUserOptions()
  const title = props.userId ? 'Edit account' : 'Add an account'

  const ready = options.data && (props.userId === null || user.data)
  if (!ready) {
    const error = options.error ?? user.error
    return (
      <Dialog open onClose={props.onClose} title={title}>
        {error ? <p className="text-sm text-danger">{getErrorMessage(error)}</p> : <Spinner />}
      </Dialog>
    )
  }

  const defaults = props.userId === null ? newUserForm(props.initialRole) : editUserForm(user.data!)
  return <UserForm userId={props.userId} defaults={defaults} options={options.data!} onClose={props.onClose} title={title} />
}

interface UserFormProps {
  userId: string | null
  defaults: UserFormValues
  options: UserOptions
  title: string
  onClose: () => void
}

function UserForm({ userId, defaults, options, title, onClose }: UserFormProps) {
  const create = useCreateUser()
  const update = useUpdateUser()
  const { register, control, handleSubmit, setError, formState: { errors } } = useForm<UserFormValues>({
    resolver: zodResolver(userFormSchema),
    defaultValues: defaults,
  })
  const role = useWatch({ control, name: 'role' })
  const input = (invalid: boolean) => cn(controlClass(invalid), 'h-10')
  const creating = userId === null

  const submit = handleSubmit((values) => {
    const callbacks = {
      onSuccess: () => {
        toast.success(creating ? `Account created. Give ${values.profile.firstName} the temporary password to sign in.` : 'Account updated.')
        onClose()
      },
      onError: (e: unknown) => { if (!applyServerErrors(e, setError)) toast.error(getErrorMessage(e)) },
    }
    if (creating) create.mutate({ role: values.role, email: values.email, password: values.password, profile: toProfile(values) }, callbacks)
    else update.mutate({ userId, role: values.role, profile: toProfile(values) }, callbacks)
  })

  const field = (name: ProfileField) => {
    const error = errors.profile?.[name]?.message
    const id = `uf-${name}`
    const control = name === 'companyId' ? (
      <select id={id} className={input(!!error)} {...register(`profile.${name}`)}>
        <option value="">Choose a company…</option>
        {options.companies.map((c) => <option key={c.id} value={c.id}>{c.name}</option>)}
      </select>
    ) : name === 'academicYear' ? (
      <select id={id} className={input(!!error)} {...register(`profile.${name}`)}>
        {[1, 2, 3, 4, 5].map((y) => <option key={y} value={y}>Year {y}</option>)}
      </select>
    ) : (
      <input id={id} className={input(!!error)} placeholder={FIELD_PLACEHOLDER[name]}
        list={name === 'programme' ? 'uf-programmes' : undefined} {...register(`profile.${name}`)} />
    )
    return <FormField key={name} label={FIELD_LABEL[name]} htmlFor={id} error={error}>{control}</FormField>
  }

  return (
    <Dialog
      open
      onClose={onClose}
      title={title}
      description={creating ? 'The person signs in with this e-mail and the temporary password, then changes it.' : defaults.email}
      footer={<>
        <Button variant="secondary" onClick={onClose}>Cancel</Button>
        <Button loading={create.isPending || update.isPending} onClick={submit}>{creating ? 'Create account' : 'Save changes'}</Button>
      </>}
    >
      <form noValidate onSubmit={submit} className="grid gap-4 md:grid-cols-2">
        {creating && (<>
          <FormField label="Role" htmlFor="uf-role" error={errors.role?.message} className="md:col-span-2">
            <select id="uf-role" className={input(!!errors.role)} {...register('role')}>
              {ROLES.map((r) => <option key={r} value={r}>{ROLE_META[r].label}</option>)}
            </select>
          </FormField>
          <FormField label="E-mail" htmlFor="uf-email" error={errors.email?.message}>
            <input id="uf-email" type="email" autoComplete="off" className={input(!!errors.email)} {...register('email')} />
          </FormField>
          <FormField label="Temporary password" htmlFor="uf-password" hint={PASSWORD_HINT} error={errors.password?.message}>
            <input id="uf-password" autoComplete="new-password" className={input(!!errors.password)} {...register('password')} />
          </FormField>
        </>)}
        <FormField label="First name" htmlFor="uf-first" error={errors.profile?.firstName?.message}>
          <input id="uf-first" className={input(!!errors.profile?.firstName)} {...register('profile.firstName')} />
        </FormField>
        <FormField label="Last name" htmlFor="uf-last" error={errors.profile?.lastName?.message}>
          <input id="uf-last" className={input(!!errors.profile?.lastName)} {...register('profile.lastName')} />
        </FormField>
        {ROLE_FIELDS[role].map(field)}
        <datalist id="uf-programmes">
          {options.programmes.map((p) => <option key={p} value={p} />)}
        </datalist>
      </form>
    </Dialog>
  )
}
