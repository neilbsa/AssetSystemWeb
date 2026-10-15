import { Link } from 'react-router'
import { History, Pencil, Wrench, CheckCircle2 } from 'lucide-react'
import { Button, Card } from '@/shared/ui'
import type { AssetItem } from '../types'
import { StatusBadge } from './StatusBadge'

interface ItemCardProps {
  item: AssetItem
  busy: boolean
  onEdit: () => void
  onMarkDefective: () => void
  onMarkAvailable: () => void
}

/** One component of an asset, with the actions that make sense for its current status. */
export function ItemCard({ item, busy, onEdit, onMarkDefective, onMarkAvailable }: ItemCardProps) {
  return (
    <Card className="p-4">
      <div className="flex items-start justify-between gap-3">
        <div className="min-w-0">
          <p className="font-medium">{item.itemCode}</p>
          <p className="truncate text-sm text-ink-muted">
            {item.typeName} · {item.description}
          </p>
        </div>
        <StatusBadge status={item.status} />
      </div>

      <dl className="mt-3 grid grid-cols-2 gap-x-3 gap-y-1.5 text-sm">
        <div>
          <dt className="text-xs text-ink-muted">Part no.</dt>
          <dd className="truncate">{item.partNumber}</dd>
        </div>
        <div>
          <dt className="text-xs text-ink-muted">Serial</dt>
          <dd className="truncate">{item.serialNumber}</dd>
        </div>
        {item.keyDetail !== 'N/A' && (
          <div className="col-span-2">
            <dt className="text-xs text-ink-muted">Key / licence</dt>
            <dd className="truncate">{item.keyDetail}</dd>
          </div>
        )}
      </dl>

      <div className="mt-3 flex flex-wrap gap-2 print:hidden">
        <Button size="sm" variant="secondary" onClick={onEdit}>
          <Pencil className="size-4" aria-hidden /> Edit
        </Button>
        {item.status === 'Available' && (
          <Button size="sm" variant="secondary" disabled={busy} onClick={onMarkDefective}>
            <Wrench className="size-4" aria-hidden /> Mark defective
          </Button>
        )}
        {item.status === 'Defective' && (
          <Button size="sm" variant="secondary" disabled={busy} onClick={onMarkAvailable}>
            <CheckCircle2 className="size-4" aria-hidden /> Mark available
          </Button>
        )}
        <Link to={`/items/${item.id}`}>
          <Button size="sm" variant="ghost">
            <History className="size-4" aria-hidden /> History
          </Button>
        </Link>
      </div>
    </Card>
  )
}
