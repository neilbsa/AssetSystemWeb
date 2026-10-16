import { useState, type FormEvent } from 'react'
import { Link, useNavigate } from 'react-router'
import { ClipboardList, Plus, Search } from 'lucide-react'
import { Button, Card, PageHeader, QueryState, Section } from '@/shared/ui'
import { cn } from '@/shared/lib/cn'
import { useAuth } from '@/features/auth/AuthProvider'
import { useDashboard } from '@/features/assets/api'
import { itemStatusLabel, type ItemStatus } from '@/features/assets/types'

const barColor: Record<ItemStatus, string> = {
  Available: 'bg-green-500',
  InUse: 'bg-blue-500',
  Defective: 'bg-red-500',
}

export function DashboardPage() {
  const { user, company } = useAuth()
  const dashboard = useDashboard()
  const navigate = useNavigate()
  const [code, setCode] = useState('')

  function lookup(e: FormEvent) {
    e.preventDefault()
    if (code.trim()) navigate(`/items?search=${encodeURIComponent(code.trim())}`)
  }

  return (
    <>
      <PageHeader title={`Hello, ${user?.fullName.split(' ')[0] ?? ''}`} subtitle={company?.name} />

      <div className="space-y-4">
        <Card className="p-4">
          <form onSubmit={lookup} className="flex gap-2">
            <div className="relative flex-1">
              <Search className="pointer-events-none absolute top-1/2 left-3 size-4 -translate-y-1/2 text-ink-muted" aria-hidden />
              <input
                value={code}
                onChange={(e) => setCode(e.target.value)}
                placeholder="Find an item by code or serial…"
                aria-label="Find an item by code or serial"
                className="block min-h-11 w-full rounded-lg border border-line bg-surface py-2 pr-3 pl-9 focus:outline-2 focus:outline-brand-500"
              />
            </div>
            <Button type="submit">Find</Button>
          </form>
        </Card>

        <div className="grid grid-cols-2 gap-3">
          <Link to="/assets/new">
            <Button className="w-full">
              <Plus className="size-4" aria-hidden /> New asset
            </Button>
          </Link>
          <Link to="/consignments/new">
            <Button variant="secondary" className="w-full">
              <ClipboardList className="size-4" aria-hidden /> New consignment
            </Button>
          </Link>
        </div>

        <QueryState query={dashboard}>
          {dashboard.data && (
            <>
              <div className="grid grid-cols-2 gap-3">
                <Stat label="Assets" value={dashboard.data.assets} to="/assets" />
                <Stat label="Items" value={dashboard.data.items} to="/items" />
              </div>

              <Section title="Items by status">
                <ul className="space-y-3">
                  {dashboard.data.itemsByStatus.map(({ status, count }) => {
                    const share = dashboard.data!.items === 0 ? 0 : (count / dashboard.data!.items) * 100
                    return (
                      <li key={status}>
                        <Link to="/items" className="block">
                          <div className="mb-1 flex justify-between text-sm">
                            <span>{itemStatusLabel[status]}</span>
                            <span className="font-medium">{count}</span>
                          </div>
                          <div className="h-2 overflow-hidden rounded-full bg-surface-muted" role="img" aria-label={`${share.toFixed(0)}% ${itemStatusLabel[status]}`}>
                            <div className={cn('h-full rounded-full', barColor[status])} style={{ width: `${share}%` }} />
                          </div>
                        </Link>
                      </li>
                    )
                  })}
                </ul>
              </Section>
            </>
          )}
        </QueryState>
      </div>
    </>
  )
}

function Stat({ label, value, to }: { label: string; value: number; to: string }) {
  return (
    <Link to={to}>
      <Card className="p-4 transition-colors hover:bg-surface-muted">
        <p className="text-xs text-ink-muted">{label}</p>
        <p className="text-3xl font-semibold">{value}</p>
      </Card>
    </Link>
  )
}
