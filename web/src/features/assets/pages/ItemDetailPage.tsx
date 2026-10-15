import { Link, useParams } from 'react-router'
import { Badge, DescriptionList, EmptyState, PageHeader, QueryState, ResponsiveTable, Section } from '@/shared/ui'
import { formatDate } from '@/shared/lib/format'
import { useItemHistory } from '../api'
import { StatusBadge } from '../components/StatusBadge'

export function ItemDetailPage() {
  const { id = '' } = useParams()
  const history = useItemHistory(id)
  const item = history.data?.item

  return (
    <>
      <PageHeader title={item?.itemCode ?? 'Item'} subtitle={item?.description} back="/items" actions={item && <StatusBadge status={item.status} />} />
      <QueryState query={history}>
        {history.data && item && (
          <div className="space-y-4">
            <Section title="Details">
              <DescriptionList
                items={[
                  { label: 'Type', value: item.typeName },
                  { label: 'Part number', value: item.partNumber },
                  { label: 'Serial number', value: item.serialNumber },
                  { label: 'Key / licence', value: item.keyDetail },
                  { label: 'Control no.', value: item.controlNumber },
                  { label: 'Remarks', value: item.remarks },
                  {
                    label: 'Asset',
                    value: (
                      <Link to={`/assets/${item.assetId}`} className="text-brand-600 hover:underline dark:text-brand-200">
                        {item.assetNumber} · {item.assetDescription}
                      </Link>
                    ),
                  },
                ]}
              />
            </Section>

            <Section title="Consignment history">
              <ResponsiveTable
                rows={history.data.consignments}
                rowKey={(c) => c.consignmentId}
                href={(c) => `/consignments/${c.consignmentId}`}
                empty={<EmptyState title="Never issued" description="This item has not been on a consignment." />}
                columns={[
                  { header: 'Consignment', cell: (c) => `#${c.consignmentNumber}`, title: true },
                  { header: 'Consignee', cell: (c) => c.consigneeName },
                  { header: 'Issued', cell: (c) => formatDate(c.issuedOn) },
                  {
                    header: 'Held',
                    cell: (c) => <Badge tone={c.lineStatus === 'Active' ? 'blue' : 'neutral'}>{c.lineStatus === 'Active' ? 'Currently' : c.lineStatus}</Badge>,
                  },
                ]}
              />
            </Section>
          </div>
        )}
      </QueryState>
    </>
  )
}
