import { Badge } from '@/shared/ui'
import { itemStatusLabel, type ItemStatus } from '../types'

const tone = { Available: 'green', InUse: 'blue', Defective: 'red' } as const

export function StatusBadge({ status }: { status: ItemStatus }) {
  return <Badge tone={tone[status]}>{itemStatusLabel[status]}</Badge>
}
