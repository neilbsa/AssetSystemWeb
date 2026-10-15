import { useState, type FormEvent } from 'react'
import { useMutation } from '@tanstack/react-query'
import { api } from '@/shared/api/http'
import { Button, ErrorNotice, PageHeader, TextField, useToast } from '@/shared/ui'
import { Card } from '@/shared/ui'

export function ChangePasswordPage() {
  const toast = useToast()
  const [current, setCurrent] = useState('')
  const [next, setNext] = useState('')
  const [confirm, setConfirm] = useState('')

  const change = useMutation({
    mutationFn: () => api('/auth/change-password', { method: 'POST', body: { currentPassword: current, newPassword: next } }),
    onSuccess: () => {
      toast.success('Password changed')
      setCurrent('')
      setNext('')
      setConfirm('')
    },
  })

  const mismatch = confirm !== '' && confirm !== next

  function submit(e: FormEvent) {
    e.preventDefault()
    if (!mismatch) change.mutate()
  }

  return (
    <>
      <PageHeader title="Account" subtitle="Change your password" />
      <Card className="max-w-md p-4">
        <form onSubmit={submit} className="space-y-4">
          {change.isError && <ErrorNotice error={change.error} />}
          <TextField label="Current password" type="password" autoComplete="current-password" required value={current} onChange={(e) => setCurrent(e.target.value)} />
          <TextField label="New password" type="password" autoComplete="new-password" minLength={8} required hint="At least 8 characters, with upper and lower case letters and a digit." value={next} onChange={(e) => setNext(e.target.value)} />
          <TextField label="Confirm new password" type="password" autoComplete="new-password" required error={mismatch ? 'Passwords do not match' : undefined} value={confirm} onChange={(e) => setConfirm(e.target.value)} />
          <Button type="submit" loading={change.isPending} disabled={mismatch}>
            Change password
          </Button>
        </form>
      </Card>
    </>
  )
}
