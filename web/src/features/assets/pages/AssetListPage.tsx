import { useState } from 'react'
import { Link } from 'react-router'
import { Plus } from 'lucide-react'
import { Badge, Button, EmptyState, Pagination, QueryState, ResponsiveTable, SearchInput, Segmented, PageHeader } from '@/shared/ui'
import { useDebounced } from '@/shared/hooks/useDebounced'
import { formatDate, formatMoney } from '@/shared/lib/format'
import { useAssets } from '../api'
import type { AssetCategory } from '../types'

type Filter = AssetCategory | 'All'

export function AssetListPage() {
  const [filter, setFilter] = useState<Filter>('All')
  const [search, setSearch] = useState('')
  const [page, setPage] = useState(1)
  const assets = useAssets({ category: filter === 'All' ? undefined : filter, search: useDebounced(search), page })

  return (
    <>
      <PageHeader
        title="Assets"
        subtitle="Purchases tracked item by item"
        actions={
          <Link to={`/assets/new${filter === 'Salesman' ? '?category=Salesman' : ''}`}>
            <Button>
              <Plus className="size-4" aria-hidden /> New asset
            </Button>
          </Link>
        }
      />

      <div className="mb-4 flex flex-col gap-3 sm:flex-row">
        <Segmented<Filter>
          label="Inventory"
          value={filter}
          onChange={(v) => {
            setFilter(v)
            setPage(1)
          }}
          options={[
            { value: 'All', label: 'All' },
            { value: 'Company', label: 'Company' },
            { value: 'Salesman', label: 'Salesman' },
          ]}
        />
        <div className="sm:flex-1">
          <SearchInput
            value={search}
            onChange={(v) => {
              setSearch(v)
              setPage(1)
            }}
            placeholder="Search number, description, vendor, serial…"
          />
        </div>
      </div>

      <QueryState query={assets}>
        {assets.data && (
          <>
            <ResponsiveTable
              rows={assets.data.items}
              rowKey={(a) => a.id}
              href={(a) => `/assets/${a.id}`}
              empty={<EmptyState title="No assets found" description="Register a purchase to start tracking its items." />}
              columns={[
                { header: 'Number', cell: (a) => a.assetNumber, title: true },
                { header: 'Description', cell: (a) => a.description },
                { header: 'Vendor', cell: (a) => a.vendor },
                { header: 'Inventory', cell: (a) => <Badge tone={a.category === 'Company' ? 'blue' : 'amber'}>{a.category}</Badge> },
                { header: 'Items', cell: (a) => `${a.availableCount} of ${a.itemCount} available` },
                { header: 'Purchased', cell: (a) => formatDate(a.purchaseDate) },
                { header: 'Price', cell: (a) => formatMoney(a.price), className: 'text-right' },
              ]}
            />
            <Pagination page={assets.data.page} pageSize={assets.data.pageSize} total={assets.data.total} onChange={setPage} />
          </>
        )}
      </QueryState>
    </>
  )
}
