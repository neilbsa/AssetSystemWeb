import { TextField } from '@/shared/ui'
import { useCreateItemType, useItemTypes } from '@/features/organization/api'
import type { ItemType } from '@/features/organization/types'
import { EditableList } from './EditableList'

type Form = { code: string; name: string }

export function ItemTypesTab() {
  const create = useCreateItemType()
  return (
    <EditableList<ItemType, Form>
      noun="item type"
      query={useItemTypes()}
      rowKey={(t) => t.code}
      blank={{ code: '', name: '' }}
      save={(v) => create.mutateAsync(v)}
      columns={[
        { header: 'Code', cell: (t) => t.code, title: true },
        { header: 'Name', cell: (t) => t.name },
      ]}
      fields={(v, set) => (
        <>
          <TextField label="Code" required maxLength={20} hint="Letters and digits only. Appears in item codes, e.g. M for Monitor." value={v.code} onChange={(e) => set('code', e.target.value)} />
          <TextField label="Name" required value={v.name} onChange={(e) => set('name', e.target.value)} />
        </>
      )}
    />
  )
}
