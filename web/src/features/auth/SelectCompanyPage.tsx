import { useState } from 'react'
import { Navigate, useNavigate } from 'react-router'
import { Building2 } from 'lucide-react'
import { Button, ErrorNotice, Spinner, TextField } from '@/shared/ui'
import { useCreateCompany } from '@/features/organization/api'
import { useAuth } from './AuthProvider'
import { AuthLayout } from './AuthLayout'

export function SelectCompanyPage() {
  const { status, companies, company, isAdministrator, selectCompany, refresh, logout } = useAuth()
  const navigate = useNavigate()
  const [error, setError] = useState<unknown>(null)
  const [pending, setPending] = useState<string | null>(null)

  if (status === 'loading') return <Spinner />
  if (status === 'anonymous') return <Navigate to="/login" replace />

  async function choose(id: string) {
    setPending(id)
    setError(null)
    try {
      await selectCompany(id)
      navigate('/', { replace: true })
    } catch (err) {
      setError(err)
      setPending(null)
    }
  }

  // First run: no company exists yet, so an administrator creates it here.
  if (companies.length === 0 && isAdministrator) {
    return (
      <AuthLayout title="Create your company" subtitle="There are no companies yet. Add the first one to get started.">
        <FirstCompanyForm onCreated={refresh} onSignOut={logout} />
      </AuthLayout>
    )
  }

  return (
    <AuthLayout title="Choose a company" subtitle="Everything you see and change belongs to this company">
      <div className="space-y-3">
        {error != null && <ErrorNotice error={error} />}
        <ul className="space-y-2">
          {companies.map((c) => (
            <li key={c.id}>
              <button
                type="button"
                disabled={pending !== null}
                onClick={() => choose(c.id)}
                className="flex min-h-14 w-full items-center gap-3 rounded-lg border border-line px-3 text-left transition-colors hover:bg-surface-muted disabled:opacity-60 aria-[current=true]:border-brand-500"
                aria-current={company?.id === c.id}
              >
                <Building2 className="size-5 text-brand-600" aria-hidden />
                <span className="flex-1">
                  <span className="block font-medium">{c.name}</span>
                  <span className="block text-xs text-ink-muted">Code {c.code}</span>
                </span>
                {pending === c.id && <Spinner label="" />}
              </button>
            </li>
          ))}
        </ul>
        <button type="button" onClick={logout} className="w-full text-center text-sm text-ink-muted hover:text-ink">
          Sign out
        </button>
      </div>
    </AuthLayout>
  )
}

function FirstCompanyForm({ onCreated, onSignOut }: { onCreated: () => Promise<void>; onSignOut: () => void }) {
  const create = useCreateCompany()
  const [values, setValues] = useState({ code: '', name: '', address: '', assetTag: '' })
  const set = (field: keyof typeof values) => (e: { target: { value: string } }) => setValues((v) => ({ ...v, [field]: e.target.value }))

  async function submit(e: React.FormEvent) {
    e.preventDefault()
    const ok = await create.mutateAsync(values).then(() => true, () => false)
    if (ok) await onCreated()
  }

  return (
    <form onSubmit={submit} className="space-y-4">
      {create.isError && <ErrorNotice error={create.error} />}
      <TextField label="Company name" required value={values.name} onChange={set('name')} />
      <TextField label="Code" required maxLength={20} value={values.code} onChange={set('code')} />
      <TextField label="Asset tag" required maxLength={10} hint="Prefix of item codes, e.g. CVC." value={values.assetTag} onChange={set('assetTag')} />
      <TextField label="Address" value={values.address} onChange={set('address')} />
      <Button type="submit" loading={create.isPending} className="w-full">
        Create company
      </Button>
      <button type="button" onClick={onSignOut} className="w-full text-center text-sm text-ink-muted hover:text-ink">
        Sign out
      </button>
    </form>
  )
}
