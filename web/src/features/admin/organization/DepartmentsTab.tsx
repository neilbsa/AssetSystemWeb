import { TextField } from '@/shared/ui'
import { useCreateDepartment, useDepartments } from '@/features/organization/api'
import type { Department } from '@/features/organization/types'
import { EditableList } from './EditableList'

type Form = { code: string; name: string }

export function DepartmentsTab() {
  const create = useCreateDepartment()
  return (
    <EditableList<Department, Form>
      noun="department"
      query={useDepartments()}
      rowKey={(d) => d.id}
      blank={{ code: '', name: '' }}
      save={(v) => create.mutateAsync(v)}
      columns={[
        { header: 'Code', cell: (d) => d.code, title: true },
        { header: 'Name', cell: (d) => d.name },
      ]}
      fields={(v, set) => (
        <>
          <TextField label="Code" required maxLength={20} value={v.code} onChange={(e) => set('code', e.target.value)} />
          <TextField label="Name" required value={v.name} onChange={(e) => set('name', e.target.value)} />
        </>
      )}
    />
  )
}
