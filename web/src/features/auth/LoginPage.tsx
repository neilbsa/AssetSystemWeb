import { useState, type FormEvent } from 'react'
import { Navigate, useLocation } from 'react-router'
import { Button, ErrorNotice, TextField } from '@/shared/ui'
import { useAuth } from './AuthProvider'
import { AuthLayout } from './AuthLayout'

export function LoginPage() {
  const { status, company, login } = useAuth()
  const from = (useLocation().state as { from?: string } | null)?.from ?? '/'

  const [userName, setUserName] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<unknown>(null)
  const [busy, setBusy] = useState(false)

  // Signed in: go where the user was heading, or pick a company first when several are available.
  if (status === 'authenticated') return <Navigate to={company ? from : '/select-company'} replace />

  async function submit(e: FormEvent) {
    e.preventDefault()
    setBusy(true)
    setError(null)
    try {
      await login(userName.trim(), password)
    } catch (err) {
      setError(err)
      setBusy(false)
    }
  }

  return (
    <AuthLayout title="Asset System" subtitle="Sign in to manage company assets">
      <form onSubmit={submit} className="space-y-4">
        {error != null && <ErrorNotice error={error} />}
        <TextField label="User name" autoComplete="username" autoCapitalize="none" required value={userName} onChange={(e) => setUserName(e.target.value)} />
        <TextField label="Password" type="password" autoComplete="current-password" required value={password} onChange={(e) => setPassword(e.target.value)} />
        <Button type="submit" loading={busy} className="w-full">
          Sign in
        </Button>
      </form>
    </AuthLayout>
  )
}
