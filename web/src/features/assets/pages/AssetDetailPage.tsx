import { useState } from 'react'
import { Link, useParams } from 'react-router'
import { Pencil, Plus, Printer } from 'lucide-react'
import { Badge, Button, DescriptionList, EmptyState, ErrorNotice, PageHeader, QueryState, Section, useToast, errorMessage } from '@/shared/ui'
import { formatDate, formatMoney } from '@/shared/lib/format'
import { useAsset, useChangeItemStatus } from '../api'
import { DocumentsSection } from '../components/DocumentsSection'
import { ItemCard } from '../components/ItemCard'
import { AddItemsModal, EditItemModal } from '../components/ItemModals'
import type { AssetItem } from '../types'

export function AssetDetailPage() {
  const { id = '' } = useParams()
  const toast = useToast()
  const asset = useAsset(id)
  const changeStatus = useChangeItemStatus(id)
  const [editing, setEditing] = useState<AssetItem | null>(null)
  const [adding, setAdding] = useState(false)

  const setStatus = (item: AssetItem, status: 'Available' | 'Defective') =>
    changeStatus.mutate({ itemId: item.id, status }, { onSuccess: () => toast.success(`${item.itemCode} is now ${status.toLowerCase()}`), onError: (e) => toast.error(errorMessage(e)) })

  const a = asset.data
  const warrantyActive = a ? new Date(a.warrantyEndsOn) >= new Date() : false

  return (
    <>
      <PageHeader
        title={a?.assetNumber ?? 'Asset'}
        subtitle={a?.description}
        back="/assets"
        actions={
          a && (
            <>
              <Button variant="secondary" onClick={() => window.print()}>
                <Printer className="size-4" aria-hidden /> Print
              </Button>
              <Link to={`/assets/${a.id}/edit`}>
                <Button variant="secondary">
                  <Pencil className="size-4" aria-hidden /> Edit
                </Button>
              </Link>
            </>
          )
        }
      />

      <QueryState query={asset}>
        {a && (
          <div className="space-y-4">
            <Section title="Purchase">
              <DescriptionList
                items={[
                  { label: 'Inventory', value: <Badge tone={a.category === 'Company' ? 'blue' : 'amber'}>{a.category}</Badge> },
                  { label: 'Branch', value: a.branchName },
                  { label: 'Vendor', value: a.vendor },
                  { label: 'Supplier', value: a.supplierName },
                  { label: 'Invoice', value: `${a.invoiceNumber} · ${formatDate(a.invoiceDate)}` },
                  { label: 'Purchase order', value: a.purchaseOrderNumber },
                  { label: 'Control no.', value: a.controlNumber },
                  { label: 'Purchase price', value: formatMoney(a.price) },
                  { label: `Current value (${a.lifeSpanMonths} month life)`, value: formatMoney(a.depreciatedValue) },
                  {
                    label: 'Warranty until',
                    value: (
                      <span className="inline-flex items-center gap-2">
                        {formatDate(a.warrantyEndsOn)} <Badge tone={warrantyActive ? 'green' : 'neutral'}>{warrantyActive ? 'Active' : 'Expired'}</Badge>
                      </span>
                    ),
                  },
                  { label: 'Remarks', value: a.remarks },
                  { label: 'Registered', value: `${formatDate(a.createdAt)} by ${a.createdBy}` },
                ]}
              />
            </Section>

            <Section
              title={`Items (${a.items.length})`}
              action={
                <Button size="sm" variant="secondary" className="print:hidden" onClick={() => setAdding(true)}>
                  <Plus className="size-4" aria-hidden /> Add items
                </Button>
              }
            >
              {changeStatus.isError && (
                <div className="mb-3">
                  <ErrorNotice error={changeStatus.error} />
                </div>
              )}
              {a.items.length === 0 ? (
                <EmptyState title="No items yet" />
              ) : (
                <div className="grid gap-3 md:grid-cols-2">
                  {a.items.map((item) => (
                    <ItemCard
                      key={item.id}
                      item={item}
                      busy={changeStatus.isPending}
                      onEdit={() => setEditing(item)}
                      onMarkDefective={() => setStatus(item, 'Defective')}
                      onMarkAvailable={() => setStatus(item, 'Available')}
                    />
                  ))}
                </div>
              )}
            </Section>

            <DocumentsSection assetId={a.id} documents={a.documents} />
          </div>
        )}
      </QueryState>

      <EditItemModal assetId={id} item={editing} onClose={() => setEditing(null)} />
      <AddItemsModal assetId={id} open={adding} onClose={() => setAdding(false)} />
    </>
  )
}
