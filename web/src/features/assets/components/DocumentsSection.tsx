import { useRef, useState } from 'react'
import { FileText, Trash2, Upload } from 'lucide-react'
import { openFile } from '@/shared/api/http'
import { Button, EmptyState, ErrorNotice, Section, SelectField, useToast, errorMessage } from '@/shared/ui'
import { formatBytes } from '@/shared/lib/format'
import { useRemoveDocument, useUploadDocument } from '../api'
import type { AssetDocument } from '../types'

const DOCUMENT_TYPES = ['Invoice', 'Delivery receipt', 'Purchase order', 'Warranty', 'Other']

export function DocumentsSection({ assetId, documents }: { assetId: string; documents: AssetDocument[] }) {
  const toast = useToast()
  const upload = useUploadDocument(assetId)
  const remove = useRemoveDocument(assetId)
  const fileInput = useRef<HTMLInputElement>(null)
  const [documentType, setDocumentType] = useState(DOCUMENT_TYPES[0]!)

  async function onFile(file: File | undefined) {
    if (!file) return
    const ok = await upload.mutateAsync({ file, documentType }).then(() => true, () => false)
    if (ok) toast.success(`${file.name} uploaded`)
    if (fileInput.current) fileInput.current.value = ''
  }

  const open = (doc: AssetDocument) => openFile(`/assets/${assetId}/documents/${doc.id}`).catch((e) => toast.error(errorMessage(e)))

  return (
    <Section title="Documents">
      {upload.isError && (
        <div className="mb-3">
          <ErrorNotice error={upload.error} />
        </div>
      )}

      {documents.length === 0 ? (
        <EmptyState title="No documents" description="Attach the invoice, delivery receipt or warranty card." />
      ) : (
        <ul className="divide-y divide-line">
          {documents.map((doc) => (
            <li key={doc.id} className="flex items-center gap-3 py-2">
              <FileText className="size-5 shrink-0 text-ink-muted" aria-hidden />
              <button type="button" onClick={() => open(doc)} className="min-w-0 flex-1 text-left">
                <span className="block truncate text-sm font-medium text-brand-600 dark:text-brand-200">{doc.fileName}</span>
                <span className="block text-xs text-ink-muted">
                  {doc.documentType} · {formatBytes(doc.sizeBytes)}
                </span>
              </button>
              <Button
                size="sm"
                variant="ghost"
                aria-label={`Delete ${doc.fileName}`}
                className="print:hidden"
                onClick={() => window.confirm(`Delete ${doc.fileName}?`) && remove.mutate(doc.id)}
              >
                <Trash2 className="size-4" aria-hidden />
              </Button>
            </li>
          ))}
        </ul>
      )}

      <div className="mt-4 flex flex-col gap-3 border-t border-line pt-4 sm:flex-row sm:items-end print:hidden">
        <SelectField label="Document type" containerClassName="sm:w-56" value={documentType} onChange={(e) => setDocumentType(e.target.value)}>
          {DOCUMENT_TYPES.map((t) => (
            <option key={t}>{t}</option>
          ))}
        </SelectField>
        <input ref={fileInput} type="file" hidden accept="application/pdf,image/*,.doc,.docx,.xls,.xlsx" onChange={(e) => onFile(e.target.files?.[0])} />
        <Button variant="secondary" loading={upload.isPending} onClick={() => fileInput.current?.click()}>
          <Upload className="size-4" aria-hidden /> Upload file
        </Button>
      </div>
    </Section>
  )
}
