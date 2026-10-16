import { TextField } from '@/shared/ui'
import { useBranches, useCreateBranch, useUpdateBranch } from '@/features/organization/api'
import type { Branch } from '@/features/organization/types'
import { EditableList } from './EditableList'

type Form = { name: string; address: string; assetTag: string }

export function BranchesTab() {
  const create = useCreateBranch()
  const update = useUpdateBranch()
  return (
    <EditableList<Branch, Form>
      noun="branch"
      query={useBranches()}
      rowKey={(b) => b.id}
      blank={{ name: '', address: '', assetTag: '' }}
      toForm={(b) => ({ name: b.name, address: b.address, assetTag: b.assetTag })}
      save={(v, existing) => (existing ? update.mutateAsync({ id: existing.id, body: v }) : create.mutateAsync(v))}
      columns={[
        { header: 'Name', cell: (b) => b.name, title: true },
        { header: 'Address', cell: (b) => b.address },
        { header: 'Asset tag', cell: (b) => b.assetTag || '—' },
      ]}
      fields={(v, set) => (
        <>
          <TextField label="Name" required value={v.name} onChange={(e) => set('name', e.target.value)} />
          <TextField label="Address" value={v.address} onChange={(e) => set('address', e.target.value)} />
          <TextField label="Asset tag" hint="Prefix of item codes created here, e.g. DVO. Leave blank for head office." maxLength={10} value={v.assetTag} onChange={(e) => set('assetTag', e.target.value)} />
        </>
      )}
    />
  )
}
