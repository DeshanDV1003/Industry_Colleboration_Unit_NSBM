import { useState, type ChangeEvent } from 'react'
import { CheckCircle2, Download, FileUp } from 'lucide-react'
import { toast } from '@/components/feedback/toast'
import { Button } from '@/components/ui/Button'
import { Dialog } from '@/components/ui/Dialog'
import { FormField } from '@/components/ui/FormField'
import { controlClass } from '@/components/ui/controlClass'
import { PASSWORD_HINT, passwordSchema } from '@/config/passwordPolicy'
import { cn } from '@/lib/cn'
import { downloadText } from '@/lib/download'
import { getErrorMessage } from '@/lib/http'
import { useImportStudents } from '../api/usersApi'
import type { StudentImportResult } from '../types'

/** Mirrors StudentCsv.Columns on the API. */
const COLUMNS = ['StudentNo', 'FirstName', 'LastName', 'Email', 'Programme', 'AcademicYear', 'Batch']
const TEMPLATE = `${COLUMNS.join(',')}\nNSBM-2026-001,Ayesha,Fonseka,ayesha.f@students.nsbm.ac.lk,BSc (Hons) Software Engineering,1,2026.1\n`

/** Opens accounts for a whole cohort from a CSV file, all with one temporary password. */
export function ImportStudentsDialog({ onClose }: { onClose: () => void }) {
  const importStudents = useImportStudents()
  const [csv, setCsv] = useState('')
  const [fileName, setFileName] = useState<string | null>(null)
  const [password, setPassword] = useState('')
  const [passwordError, setPasswordError] = useState<string | null>(null)
  const [result, setResult] = useState<StudentImportResult | null>(null)

  const readFile = async (e: ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0]
    if (!file) return
    setCsv(await file.text())
    setFileName(file.name)
    setResult(null)
  }

  const submit = () => {
    const checked = passwordSchema.safeParse(password)
    setPasswordError(checked.success ? null : checked.error.issues[0].message)
    if (!checked.success) return
    if (!csv.trim()) { toast.error('Choose a CSV file or paste the rows first.'); return }

    importStudents.mutate({ csv, temporaryPassword: password }, {
      onSuccess: (r) => {
        setResult(r)
        if (r.errors.length === 0) toast.success(`${r.created} student account${r.created === 1 ? '' : 's'} created.`)
      },
      onError: (e) => toast.error(getErrorMessage(e)),
    })
  }

  return (
    <Dialog
      open
      onClose={onClose}
      title="Import students"
      description="One row per student. Valid rows are created; any problem rows are listed so you can fix and import them again."
      footer={<>
        <Button variant="secondary" onClick={onClose}>{result ? 'Done' : 'Cancel'}</Button>
        <Button loading={importStudents.isPending} onClick={submit}><FileUp className="size-4" /> Import</Button>
      </>}
    >
      <div className="space-y-4 text-sm">
        <div className="flex flex-wrap items-center justify-between gap-2 rounded-xl bg-surface-subtle px-4 py-3">
          <p className="text-body">Columns: <code className="text-xs">{COLUMNS.join(', ')}</code></p>
          <Button size="sm" variant="ghost" onClick={() => downloadText('students-template.csv', TEMPLATE)}>
            <Download className="size-4" /> Template
          </Button>
        </div>

        <FormField label="CSV file" htmlFor="im-file" hint={fileName ? `Loaded ${fileName}. You can still edit the rows below.` : 'Saved from Excel or Google Sheets as CSV.'}>
          <input id="im-file" type="file" accept=".csv,text/csv" onChange={readFile} className="block w-full text-sm file:mr-3 file:rounded-lg file:border-0 file:bg-primary-soft file:px-3 file:py-2 file:font-semibold file:text-primary" />
        </FormField>
        <FormField label="Rows" htmlFor="im-csv">
          <textarea id="im-csv" rows={6} value={csv} onChange={(e) => { setCsv(e.target.value); setResult(null) }}
            placeholder={TEMPLATE} className={cn(controlClass(false), 'py-2 font-mono text-xs')} />
        </FormField>
        <FormField label="Temporary password for everyone" htmlFor="im-password" hint={PASSWORD_HINT} error={passwordError ?? undefined}>
          <input id="im-password" autoComplete="new-password" value={password} onChange={(e) => setPassword(e.target.value)} className={cn(controlClass(!!passwordError), 'h-10')} />
        </FormField>

        {result && (
          <div className="space-y-2 rounded-xl border border-line p-4">
            <p className="flex items-center gap-2 font-semibold text-heading">
              <CheckCircle2 className="size-4 text-success" /> {result.created} created{result.errors.length > 0 && `, ${result.errors.length} not imported`}
            </p>
            {result.errors.length > 0 && (
              <ul className="max-h-48 space-y-1 overflow-y-auto text-xs">
                {result.errors.map((e) => (
                  <li key={e.row}><span className="font-semibold text-danger">Row {e.row}{e.studentNo ? ` (${e.studentNo})` : ''}:</span> {e.message}</li>
                ))}
              </ul>
            )}
          </div>
        )}
      </div>
    </Dialog>
  )
}
