import { Badge, TextField } from '@/shared/ui'
import { useCompanies, useCreateCompany, useUpdateCompany } from '@/features/organization/api'
import type { Company } from '@/features/organization/types'
import { EditableList } from './EditableList'

type Form = { code: string; name: string; address: string; assetTag: string }

export function CompaniesTab() {
  const create = useCreateCompany()
  const update = useUpdateCompany()
  return (
    <EditableList<Company, Form>
      noun="company"
      query={useCompanies()}
      rowKey={(c) => c.id}
      blank={{ code: '', name: '', address: '', assetTag: '' }}
      toForm={(c) => ({ code: c.code, name: c.name, address: c.address, assetTag: c.assetTag })}
      save={(v, existing) =>
        existing ? update.mutateAsync({ id: existing.id, body: { name: v.name, address: v.address, assetTag: v.assetTag } }) : create.mutateAsync(v)
      }
      columns={[
        { header: 'Name', cell: (c) => c.name, title: true },
        { header: 'Code', cell: (c) => c.code },
        { header: 'Asset tag', cell: (c) => c.assetTag },
        { header: 'Status', cell: (c) => <Badge tone={c.isActive ? 'green' : 'neutral'}>{c.isActive ? 'Active' : 'Inactive'}</Badge> },
      ]}
      fields={(v, set, existing) => (
        <>
          <TextField label="Code" required disabled={existing !== null} maxLength={20} value={v.code} onChange={(e) => set('code', e.target.value)} />
          <TextField label="Name" required value={v.name} onChange={(e) => set('name', e.target.value)} />
          <TextField label="Address" value={v.address} onChange={(e) => set('address', e.target.value)} />
          <TextField label="Asset tag" required maxLength={10} hint="Prefix of every item code in this company, e.g. CVC." value={v.assetTag} onChange={(e) => set('assetTag', e.target.value)} />
        </>
      )}
    />
  )
}
