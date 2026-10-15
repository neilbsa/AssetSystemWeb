import { useState } from 'react'
import { Lock, Pencil, Plus } from 'lucide-react'
import { Badge, Button, EmptyState, ErrorNotice, Modal, PageHeader, QueryState, ResponsiveTable, TextField, useToast } from '@/shared/ui'
import { useCompanies } from '@/features/organization/api'
import type { Company } from '@/features/organization/types'
import { ROLES, useRegisterUser, useUpdateUserAccess, useUsers, type SystemUser } from './api'

export function UsersPage() {
  const users = useUsers()
  const companies = useCompanies()
  const [editing, setEditing] = useState<SystemUser | 'new' | null>(null)

  return (
    <>
      <PageHeader
        title="System users"
        subtitle="People who can sign in"
        actions={
          <Button onClick={() => setEditing('new')}>
            <Plus className="size-4" aria-hidden /> Add user
          </Button>
        }
      />

      <QueryState query={users}>
        {users.data && (
          <ResponsiveTable
            rows={users.data}
            rowKey={(u) => u.id}
            empty={<EmptyState title="No users" />}
            columns={[
              { header: 'User name', cell: (u) => u.userName, title: true },
              { header: 'Name', cell: (u) => u.fullName },
              { header: 'Email', cell: (u) => u.email },
              {
                header: 'Roles',
                cell: (u) => (
                  <span className="inline-flex flex-wrap gap-1">
                    {u.roles.map((r) => (
                      <Badge key={r} tone={r === 'Administrator' ? 'amber' : 'neutral'}>
                        {r}
                      </Badge>
                    ))}
                    {u.isLocked && (
                      <Badge tone="red">
                        <Lock className="mr-1 size-3" aria-hidden /> Locked
                      </Badge>
                    )}
                  </span>
                ),
              },
              {
                header: 'Edit',
                className: 'text-right',
                cell: (u) => (
                  <Button size="sm" variant="ghost" aria-label={`Edit ${u.userName}`} onClick={() => setEditing(u)}>
                    <Pencil className="size-4" aria-hidden />
                  </Button>
                ),
              },
            ]}
          />
        )}
      </QueryState>

      <Modal open={editing !== null} title={editing === 'new' ? 'Add user' : 'Edit access'} onClose={() => setEditing(null)}>
        {editing === 'new' && <RegisterForm companies={companies.data ?? []} onDone={() => setEditing(null)} />}
        {editing && editing !== 'new' && <AccessForm key={editing.id} user={editing} companies={companies.data ?? []} onDone={() => setEditing(null)} />}
      </Modal>
    </>
  )
}

function toggle<T>(list: T[], value: T): T[] {
  return list.includes(value) ? list.filter((x) => x !== value) : [...list, value]
}

/** Checkbox groups for roles and companies, shared by both forms. */
function AccessFields({
  roles, companyIds, companies, onRoles, onCompanies,
}: { roles: string[]; companyIds: string[]; companies: Company[]; onRoles: (r: string[]) => void; onCompanies: (c: string[]) => void }) {
  const isAdmin = roles.includes('Administrator')
  return (
    <>
      <fieldset className="space-y-2">
        <legend className="text-sm font-medium">Roles</legend>
        {ROLES.map((role) => (
          <label key={role} className="flex min-h-11 items-center gap-3">
            <input type="checkbox" className="size-5" checked={roles.includes(role)} onChange={() => onRoles(toggle(roles, role))} />
            {role}
          </label>
        ))}
      </fieldset>
      <fieldset className="space-y-2">
        <legend className="text-sm font-medium">Companies</legend>
        {isAdmin && <p className="text-xs text-ink-muted">Administrators can work in every company.</p>}
        {companies.map((company) => (
          <label key={company.id} className="flex min-h-11 items-center gap-3">
            <input type="checkbox" className="size-5" disabled={isAdmin} checked={isAdmin || companyIds.includes(company.id)} onChange={() => onCompanies(toggle(companyIds, company.id))} />
            {company.name}
          </label>
        ))}
      </fieldset>
    </>
  )
}

function RegisterForm({ companies, onDone }: { companies: Company[]; onDone: () => void }) {
  const toast = useToast()
  const register = useRegisterUser()
  const [v, setV] = useState({ userName: '', email: '', firstName: '', middleName: '', lastName: '', password: '' })
  const [roles, setRoles] = useState<string[]>(['Common'])
  const [companyIds, setCompanyIds] = useState<string[]>([])
  const set = (field: keyof typeof v) => (e: { target: { value: string } }) => setV((x) => ({ ...x, [field]: e.target.value }))

  async function submit(e: React.FormEvent) {
    e.preventDefault()
    const ok = await register.mutateAsync({ ...v, roles, companyIds }).then(() => true, () => false)
    if (!ok) return
    toast.success(`${v.userName} added`)
    onDone()
  }

  return (
    <form onSubmit={submit} className="space-y-4">
      {register.isError && <ErrorNotice error={register.error} />}
      <TextField label="User name" required autoCapitalize="none" value={v.userName} onChange={set('userName')} />
      <TextField label="Email" type="email" required value={v.email} onChange={set('email')} />
      <div className="grid gap-4 sm:grid-cols-3">
        <TextField label="First name" required value={v.firstName} onChange={set('firstName')} />
        <TextField label="Middle name" value={v.middleName} onChange={set('middleName')} />
        <TextField label="Last name" required value={v.lastName} onChange={set('lastName')} />
      </div>
      <TextField label="Temporary password" type="password" autoComplete="new-password" required minLength={8} hint="At least 8 characters with upper and lower case letters and a digit." value={v.password} onChange={set('password')} />
      <AccessFields roles={roles} companyIds={companyIds} companies={companies} onRoles={setRoles} onCompanies={setCompanyIds} />
      <Button type="submit" loading={register.isPending} className="w-full">
        Add user
      </Button>
    </form>
  )
}

function AccessForm({ user, companies, onDone }: { user: SystemUser; companies: Company[]; onDone: () => void }) {
  const toast = useToast()
  const update = useUpdateUserAccess()
  const [roles, setRoles] = useState(user.roles)
  const [companyIds, setCompanyIds] = useState(user.companyIds)
  const [isLocked, setIsLocked] = useState(user.isLocked)
  const [newPassword, setNewPassword] = useState('')

  async function submit(e: React.FormEvent) {
    e.preventDefault()
    const ok = await update.mutateAsync({ id: user.id, body: { roles, companyIds, isLocked, newPassword: newPassword || null } }).then(() => true, () => false)
    if (!ok) return
    toast.success(`${user.userName} updated`)
    onDone()
  }

  return (
    <form onSubmit={submit} className="space-y-4">
      <p className="text-sm text-ink-muted">
        {user.fullName} · {user.email}
      </p>
      {update.isError && <ErrorNotice error={update.error} />}
      <AccessFields roles={roles} companyIds={companyIds} companies={companies} onRoles={setRoles} onCompanies={setCompanyIds} />
      <label className="flex min-h-11 items-center gap-3">
        <input type="checkbox" className="size-5" checked={isLocked} onChange={(e) => setIsLocked(e.target.checked)} />
        Lock this account (cannot sign in)
      </label>
      <TextField label="Reset password" type="password" autoComplete="new-password" hint="Leave blank to keep the current password." value={newPassword} onChange={(e) => setNewPassword(e.target.value)} />
      <Button type="submit" loading={update.isPending} className="w-full">
        Save
      </Button>
    </form>
  )
}
