import { useState } from 'react'
import { useSearchParams } from 'react-router'
import { EmptyState, PageHeader, Pagination, QueryState, ResponsiveTable, SearchInput, Segmented } from '@/shared/ui'
import { useDebounced } from '@/shared/hooks/useDebounced'
import { useItems } from '../api'
import { StatusBadge } from '../components/StatusBadge'
import { itemStatusLabel, type ItemStatus } from '../types'

type Filter = ItemStatus | 'All'

export function ItemListPage() {
  const [status, setStatus] = useState<Filter>('All')
  const [params] = useSearchParams()
  const [search, setSearch] = useState(params.get('search') ?? '')
  const [page, setPage] = useState(1)
  const items = useItems({ status: status === 'All' ? undefined : status, search: useDebounced(search), page })

  return (
    <>
      <PageHeader title="Items" subtitle="Every component across all assets" />

      <div className="mb-4 flex flex-col gap-3 sm:flex-row">
        <Segmented<Filter>
          label="Status"
          value={status}
          onChange={(v) => {
            setStatus(v)
            setPage(1)
          }}
          options={[
            { value: 'All', label: 'All' },
            { value: 'Available', label: itemStatusLabel.Available },
            { value: 'InUse', label: itemStatusLabel.InUse },
            { value: 'Defective', label: itemStatusLabel.Defective },
          ]}
        />
        <div className="sm:flex-1">
          <SearchInput
            value={search}
            onChange={(v) => {
              setSearch(v)
              setPage(1)
            }}
            placeholder="Search item code, description, serial…"
          />
        </div>
      </div>

      <QueryState query={items}>
        {items.data && (
          <>
            <ResponsiveTable
              rows={items.data.items}
              rowKey={(i) => i.id}
              href={(i) => `/items/${i.id}`}
              empty={<EmptyState title="No items match" />}
              columns={[
                { header: 'Code', cell: (i) => i.itemCode, title: true },
                { header: 'Type', cell: (i) => i.typeName },
                { header: 'Description', cell: (i) => i.description },
                { header: 'Serial', cell: (i) => i.serialNumber },
                { header: 'Asset', cell: (i) => i.assetNumber },
                { header: 'Status', cell: (i) => <StatusBadge status={i.status} /> },
              ]}
            />
            <Pagination page={items.data.page} pageSize={items.data.pageSize} total={items.data.total} onChange={setPage} />
          </>
        )}
      </QueryState>
    </>
  )
}
