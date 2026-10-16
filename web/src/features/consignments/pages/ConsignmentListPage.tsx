import { useState } from 'react'
import { Link, useSearchParams } from 'react-router'
import { Plus, Users } from 'lucide-react'
import { Badge, Button, EmptyState, PageHeader, Pagination, QueryState, ResponsiveTable, SearchInput } from '@/shared/ui'
import { useDebounced } from '@/shared/hooks/useDebounced'
import { formatDate } from '@/shared/lib/format'
import { useConsignments } from '../api'

export function ConsignmentListPage() {
  const [params, setParams] = useSearchParams()
  const consigneeId = params.get('consigneeId') ?? undefined
  const [search, setSearch] = useState('')
  const [page, setPage] = useState(1)
  const consignments = useConsignments({ consigneeId, search: useDebounced(search), page })

  return (
    <>
      <PageHeader
        title="Consignments"
        subtitle="Equipment handed to employees"
        actions={
          <>
            <Link to="/consignees">
              <Button variant="secondary">
                <Users className="size-4" aria-hidden /> Who holds what
              </Button>
            </Link>
            <Link to="/consignments/new">
              <Button>
                <Plus className="size-4" aria-hidden /> New
              </Button>
            </Link>
          </>
        }
      />

      <div className="mb-4 space-y-3">
        {consigneeId && (
          <div className="flex items-center justify-between rounded-lg bg-brand-50 px-3 py-2 text-sm dark:bg-brand-600/20">
            Showing one employee's consignments
            <button type="button" className="font-medium text-brand-600 dark:text-brand-200" onClick={() => setParams({})}>
              Show all
            </button>
          </div>
        )}
        <SearchInput
          value={search}
          onChange={(v) => {
            setSearch(v)
            setPage(1)
          }}
          placeholder="Search number, employee, item code…"
        />
      </div>

      <QueryState query={consignments}>
        {consignments.data && (
          <>
            <ResponsiveTable
              rows={consignments.data.items}
              rowKey={(c) => c.id}
              href={(c) => `/consignments/${c.id}`}
              empty={<EmptyState title="No consignments yet" description="Issue equipment to an employee to create one." />}
              columns={[
                { header: 'Number', cell: (c) => `#${c.number}`, title: true },
                { header: 'Consignee', cell: (c) => `${c.consigneeName} (${c.consigneeNumber})` },
                { header: 'Issued', cell: (c) => formatDate(c.issuedOn) },
                { header: 'Items', cell: (c) => c.itemCount },
                { header: 'Signed copy', cell: (c) => (c.hasReceivingCopy ? <Badge tone="green">Attached</Badge> : <Badge>Missing</Badge>) },
                { header: 'Status', cell: (c) => <Badge tone={c.status === 'Open' ? 'blue' : 'red'}>{c.status}</Badge> },
              ]}
            />
            <Pagination page={consignments.data.page} pageSize={consignments.data.pageSize} total={consignments.data.total} onChange={setPage} />
          </>
        )}
      </QueryState>
    </>
  )
}
