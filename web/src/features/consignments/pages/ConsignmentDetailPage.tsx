import { useRef } from 'react'
import { useParams } from 'react-router'
import { FileText, Printer, Upload } from 'lucide-react'
import { openFile } from '@/shared/api/http'
import { Badge, Button, DescriptionList, ErrorNotice, PageHeader, QueryState, ResponsiveTable, Section, useToast, errorMessage } from '@/shared/ui'
import { formatBytes, formatDate } from '@/shared/lib/format'
import { useConsignment, useUploadReceivingCopy } from '../api'
import type { LineStatus } from '../types'

const lineTone: Record<LineStatus, 'blue' | 'neutral' | 'red'> = { Active: 'blue', Superseded: 'neutral', Cancelled: 'red' }

export function ConsignmentDetailPage() {
  const { id = '' } = useParams()
  const toast = useToast()
  const consignment = useConsignment(id)
  const upload = useUploadReceivingCopy(id)
  const fileInput = useRef<HTMLInputElement>(null)
  const c = consignment.data

  async function onFile(file: File | undefined) {
    if (!file) return
    const ok = await upload.mutateAsync(file).then(() => true, () => false)
    if (ok) toast.success('Receiving copy attached')
    if (fileInput.current) fileInput.current.value = ''
  }

  return (
    <>
      <PageHeader
        title={c ? `Consignment #${c.number}` : 'Consignment'}
        subtitle={c && formatDate(c.issuedOn)}
        back="/consignments"
        actions={
          c && (
            <Button variant="secondary" onClick={() => window.print()}>
              <Printer className="size-4" aria-hidden /> Print
            </Button>
          )
        }
      />

      <QueryState query={consignment}>
        {c && (
          <div className="space-y-4">
            {c.status === 'Cancelled' && (
              <div role="alert" className="rounded-lg border border-red-500/40 bg-red-500/10 p-3 text-sm text-red-700 dark:text-red-300">
                <strong>Cancelled.</strong> {c.cancellationReason}
              </div>
            )}

            <Section title="Consignee">
              <DescriptionList
                items={[
                  { label: 'Name', value: c.consigneeName },
                  { label: 'Employee number', value: c.consigneeNumber },
                  { label: 'Department', value: c.departmentName },
                  { label: 'Remarks', value: c.remarks },
                  { label: 'Issued by', value: `${c.createdBy} · ${formatDate(c.createdAt)}` },
                  { label: 'Status', value: <Badge tone={c.status === 'Open' ? 'blue' : 'red'}>{c.status}</Badge> },
                ]}
              />
            </Section>

            <Section title={`Items (${c.lines.length})`}>
              <ResponsiveTable
                rows={c.lines}
                rowKey={(l) => l.itemId}
                href={(l) => `/items/${l.itemId}`}
                columns={[
                  { header: 'Code', cell: (l) => l.itemCode, title: true },
                  { header: 'Type', cell: (l) => l.typeName },
                  { header: 'Description', cell: (l) => l.description },
                  { header: 'Serial', cell: (l) => l.serialNumber },
                  { header: 'Asset', cell: (l) => l.assetNumber },
                  { header: 'Held', cell: (l) => <Badge tone={lineTone[l.status]}>{l.status === 'Superseded' ? 'Reassigned' : l.status}</Badge> },
                ]}
              />
            </Section>

            <Section title="Signed receiving copy">
              {upload.isError && (
                <div className="mb-3">
                  <ErrorNotice error={upload.error} />
                </div>
              )}
              <div className="flex flex-wrap items-center gap-3">
                {c.receivingCopy ? (
                  <button
                    type="button"
                    className="inline-flex items-center gap-2 text-sm font-medium text-brand-600 dark:text-brand-200"
                    onClick={() => openFile(`/consignments/${c.id}/receiving-copy`).catch((e) => toast.error(errorMessage(e)))}
                  >
                    <FileText className="size-4" aria-hidden />
                    {c.receivingCopy.fileName} ({formatBytes(c.receivingCopy.sizeBytes)})
                  </button>
                ) : (
                  <span className="text-sm text-ink-muted">Not attached yet.</span>
                )}
                {c.status === 'Open' && (
                  <>
                    <input ref={fileInput} type="file" hidden accept="application/pdf,image/*" onChange={(e) => onFile(e.target.files?.[0])} />
                    <Button variant="secondary" size="sm" loading={upload.isPending} className="print:hidden" onClick={() => fileInput.current?.click()}>
                      <Upload className="size-4" aria-hidden /> {c.receivingCopy ? 'Replace' : 'Attach'}
                    </Button>
                  </>
                )}
              </div>
            </Section>

            {/* Only on paper: signature lines for the receiving copy. */}
            <div className="hidden grid-cols-2 gap-12 pt-16 text-center text-sm print:grid">
              <div className="border-t border-black pt-2">Issued by</div>
              <div className="border-t border-black pt-2">Received by: {c.consigneeName}</div>
            </div>
          </div>
        )}
      </QueryState>
    </>
  )
}
