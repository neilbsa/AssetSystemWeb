import { useState } from 'react'
import { Pencil, Plus, UserX } from 'lucide-react'
import { Button, EmptyState, ErrorNotice, Modal, PageHeader, QueryState, ResponsiveTable, SearchInput, SelectField, TextField, useToast } from '@/shared/ui'
import { useDebounced } from '@/shared/hooks/useDebounced'
import { useAuth } from '@/features/auth/AuthProvider'
import {
  useBranches, useDeactivateEmployee, useDepartments, useEmployees, useRegisterEmployee, useUpdateEmployee, type EmployeeInput,
} from '@/features/organization/api'
import type { Employee } from '@/features/organization/types'

const blank: EmployeeInput = { employeeNumber: '', firstName: '', middleName: '', lastName: '', email: '', departmentId: '', branchId: '' }

export function EmployeesPage() {
  const { isAdministrator } = useAuth()
  const toast = useToast()
  const [search, setSearch] = useState('')
  const employees = useEmployees(useDebounced(search))
  const deactivate = useDeactivateEmployee()
  // `null` = closed, `'new'` = registering, otherwise the employee being edited.
  const [editing, setEditing] = useState<Employee | 'new' | null>(null)

  const remove = (e: Employee) => {
    if (window.confirm(`Deactivate ${e.fullName}? They will no longer appear when issuing equipment.`)) {
      deactivate.mutate(e.id, { onSuccess: () => toast.success(`${e.fullName} deactivated`) })
    }
  }

  return (
    <>
      <PageHeader
        title="Employees"
        subtitle="People who can be given equipment"
        actions={
          isAdministrator && (
            <Button onClick={() => setEditing('new')}>
              <Plus className="size-4" aria-hidden /> Register
            </Button>
          )
        }
      />
      <div className="mb-4">
        <SearchInput value={search} onChange={setSearch} placeholder="Search name or employee number…" />
      </div>

      <QueryState query={employees}>
        {employees.data && (
          <ResponsiveTable
            rows={employees.data}
            rowKey={(e) => e.id}
            empty={<EmptyState title="No employees found" />}
            columns={[
              { header: 'Name', cell: (e) => e.fullName, title: true },
              { header: 'Number', cell: (e) => e.employeeNumber },
              { header: 'Department', cell: (e) => e.departmentName ?? '—' },
              { header: 'Email', cell: (e) => e.email },
              ...(isAdministrator
                ? [
                    {
                      header: 'Actions',
                      className: 'text-right',
                      cell: (e: Employee) => (
                        <span className="inline-flex gap-1">
                          <Button size="sm" variant="ghost" aria-label={`Edit ${e.fullName}`} onClick={() => setEditing(e)}>
                            <Pencil className="size-4" aria-hidden />
                          </Button>
                          <Button size="sm" variant="ghost" aria-label={`Deactivate ${e.fullName}`} onClick={() => remove(e)}>
                            <UserX className="size-4" aria-hidden />
                          </Button>
                        </span>
                      ),
                    },
                  ]
                : []),
            ]}
          />
        )}
      </QueryState>

      <Modal open={editing !== null} title={editing === 'new' ? 'Register employee' : 'Edit employee'} onClose={() => setEditing(null)}>
        {editing !== null && <EmployeeForm key={editing === 'new' ? 'new' : editing.id} employee={editing === 'new' ? null : editing} onDone={() => setEditing(null)} />}
      </Modal>
    </>
  )
}

function EmployeeForm({ employee, onDone }: { employee: Employee | null; onDone: () => void }) {
  const toast = useToast()
  const departments = useDepartments()
  const branches = useBranches()
  const register = useRegisterEmployee()
  const update = useUpdateEmployee()
  const mutation = employee ? update : register

  const [values, setValues] = useState<EmployeeInput>(
    employee
      ? {
          employeeNumber: employee.employeeNumber,
          firstName: employee.firstName,
          middleName: employee.middleName ?? '',
          lastName: employee.lastName,
          email: employee.email,
          departmentId: employee.departmentId,
          branchId: employee.branchId,
        }
      : blank,
  )
  const set = (field: keyof EmployeeInput) => (e: { target: { value: string } }) => setValues((v) => ({ ...v, [field]: e.target.value }))

  async function submit(e: React.FormEvent) {
    e.preventDefault()
    const { employeeNumber, ...rest } = values
    const ok = await (employee ? update.mutateAsync({ id: employee.id, body: rest }) : register.mutateAsync({ employeeNumber, ...rest })).then(() => true, () => false)
    if (!ok) return
    toast.success(employee ? 'Employee updated' : 'Employee registered')
    onDone()
  }

  return (
    <form onSubmit={submit} className="space-y-4">
      {mutation.isError && <ErrorNotice error={mutation.error} />}
      <TextField label="Employee number" required disabled={employee !== null} value={values.employeeNumber} onChange={set('employeeNumber')} />
      <div className="grid gap-4 sm:grid-cols-3">
        <TextField label="First name" required value={values.firstName} onChange={set('firstName')} />
        <TextField label="Middle name" value={values.middleName} onChange={set('middleName')} />
        <TextField label="Last name" required value={values.lastName} onChange={set('lastName')} />
      </div>
      <TextField label="Email" type="email" required value={values.email} onChange={set('email')} />
      <SelectField label="Department" required value={values.departmentId} onChange={set('departmentId')}>
        <option value="">Select…</option>
        {departments.data?.map((d) => (
          <option key={d.id} value={d.id}>
            {d.code} · {d.name}
          </option>
        ))}
      </SelectField>
      <SelectField label="Branch" required value={values.branchId} onChange={set('branchId')}>
        <option value="">Select…</option>
        {branches.data?.map((b) => (
          <option key={b.id} value={b.id}>
            {b.name}
          </option>
        ))}
      </SelectField>
      <Button type="submit" loading={mutation.isPending} className="w-full">
        {employee ? 'Save' : 'Register'}
      </Button>
    </form>
  )
}
