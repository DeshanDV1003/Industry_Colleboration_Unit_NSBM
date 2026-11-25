import { useNavigate, useParams } from 'react-router-dom'
import type { UseFormSetError } from 'react-hook-form'
import { Briefcase, Lock } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { PageHeader } from '@/components/layout/PageHeader'
import { EmptyState } from '@/components/ui/EmptyState'
import { LinkButton } from '@/components/ui/LinkButton'
import { PageSpinner } from '@/components/ui/Spinner'
import { applyServerErrors } from '@/lib/formErrors'
import { getErrorMessage } from '@/lib/http'
import { useCreateVacancy, useUpdateVacancy, useVacancy } from '../api/vacanciesApi'
import { VacancyEditorForm } from '../components/VacancyEditorForm'
import { emptyVacancyForm, toVacancyFormValues, toVacancyInput, type VacancyFormValues } from '../vacancyForm'

const LIST_PATH = '/industry-hub'

/** Posts a new vacancy (/industry-hub/vacancies/new) or edits a pending or rejected one (/industry-hub/vacancies/:id/edit). */
export default function VacancyEditorPage() {
  const { id } = useParams()
  const vacancyId = id ? Number(id) : null
  const navigate = useNavigate()
  const { data: vacancy, isLoading, isError, error } = useVacancy(vacancyId)
  const create = useCreateVacancy()
  const update = useUpdateVacancy()

  if (vacancyId !== null && isLoading) return <PageSpinner label="Loading vacancy…" />
  if (vacancyId !== null && (isError || !vacancy)) return <EmptyState icon={Briefcase} title="Vacancy could not be loaded" description={getErrorMessage(error)} />

  if (vacancy && !vacancy.canBeEdited) {
    return (
      <EmptyState
        icon={Lock}
        title="This vacancy is live"
        description="Approved vacancies are visible to students and can no longer be edited."
        action={<LinkButton to={LIST_PATH} variant="secondary">Back to vacancies</LinkButton>}
      />
    )
  }

  const handleSubmit = (values: VacancyFormValues, setError: UseFormSetError<VacancyFormValues>) => {
    const input = toVacancyInput(values)
    const onError = (e: unknown) => {
      if (!applyServerErrors(e, setError, 'Vacancy.')) toast.error(getErrorMessage(e))
    }
    const onSuccess = () => {
      toast.success('Submitted for review. Students can search the vacancy once the Unit approves it.')
      navigate(LIST_PATH)
    }

    if (vacancyId === null) create.mutate(input, { onSuccess, onError })
    else update.mutate({ id: vacancyId, input }, { onSuccess, onError })
  }

  return (
    <>
      <PageHeader
        title={vacancy ? 'Edit vacancy' : 'Post an internship vacancy'}
        description="Vacancies are fictional practice opportunities. They stay Pending until the Unit reviews them."
      />
      <VacancyEditorForm
        key={vacancy?.id ?? 'new'}
        defaultValues={vacancy ? toVacancyFormValues(vacancy) : emptyVacancyForm}
        submitLabel={vacancy ? 'Save and resubmit' : 'Submit for review'}
        submitting={create.isPending || update.isPending}
        onSubmit={handleSubmit}
        onCancel={() => navigate(LIST_PATH)}
      />
    </>
  )
}
