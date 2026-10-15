import { useState } from 'react'
import { EmptyState, PageHeader, QueryState, ResponsiveTable, SearchInput } from '@/shared/ui'
import { useDebounced } from '@/shared/hooks/useDebounced'
import { useConsignees } from '../api'

/** "Who holds what": one row per employee that has been issued equipment. */
export function ConsigneeListPage() {
  const [search, setSearch] = useState('')
  const consignees = useConsignees(useDebounced(search))

  return (
    <>
      <PageHeader title="Who holds what" subtitle="Employees with issued equipment" back="/consignments" />
      <div className="mb-4">
        <SearchInput value={search} onChange={setSearch} placeholder="Search name or employee number…" />
      </div>
      <QueryState query={consignees}>
        {consignees.data && (
          <ResponsiveTable
            rows={consignees.data}
            rowKey={(c) => c.consigneeId}
            href={(c) => `/consignments?consigneeId=${c.consigneeId}`}
            empty={<EmptyState title="Nobody holds equipment yet" />}
            columns={[
              { header: 'Employee', cell: (c) => c.consigneeName, title: true },
              { header: 'Number', cell: (c) => c.consigneeNumber },
              { header: 'Department', cell: (c) => c.departmentName ?? '—' },
              { header: 'Consignments', cell: (c) => c.consignmentCount },
              { header: 'Items held', cell: (c) => c.activeItemCount },
            ]}
          />
        )}
      </QueryState>
    </>
  )
}
