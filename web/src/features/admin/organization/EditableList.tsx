import { useState, type FormEvent, type ReactNode } from 'react'
import { Plus } from 'lucide-react'
import { Button, EmptyState, ErrorNotice, Modal, QueryState, ResponsiveTable, useToast, type Column } from '@/shared/ui'

interface EditableListProps<T, TForm> {
  /** What one thing is called, for button labels: "branch". */
  noun: string
  query: { isPending: boolean; isError: boolean; error: unknown; data: T[] | undefined }
  rowKey: (row: T) => string
  columns: Column<T>[]
  /** Values for the "add" form. */
  blank: TForm
  /** Values for editing a row; omit to make the list add-only. */
  toForm?: (row: T) => TForm
  save: (values: TForm, existing: T | null) => Promise<unknown>
  fields: (values: TForm, set: <K extends keyof TForm>(key: K, value: TForm[K]) => void, existing: T | null) => ReactNode
}

/** The list + "add/edit in a dialog" pattern shared by every master-data tab. */
export function EditableList<T, TForm>({ noun, query, rowKey, columns, blank, toForm, save, fields }: EditableListProps<T, TForm>) {
  const toast = useToast()
  const [open, setOpen] = useState(false)
  const [existing, setExisting] = useState<T | null>(null)
  const [values, setValues] = useState<TForm>(blank)
  const [error, setError] = useState<unknown>(null)
  const [busy, setBusy] = useState(false)

  const openFor = (row: T | null) => {
    setExisting(row)
    setValues(row && toForm ? toForm(row) : blank)
    setError(null)
    setOpen(true)
  }

  async function submit(e: FormEvent) {
    e.preventDefault()
    setBusy(true)
    setError(null)
    try {
      await save(values, existing)
      toast.success(existing ? `${noun} updated` : `${noun} added`)
      setOpen(false)
    } catch (err) {
      setError(err)
    } finally {
      setBusy(false)
    }
  }

  const allColumns: Column<T>[] = toForm
    ? [...columns, { header: 'Edit', className: 'text-right', cell: (row) => <Button size="sm" variant="ghost" onClick={() => openFor(row)}>Edit</Button> }]
    : columns

  return (
    <>
      <div className="mb-3 flex justify-end">
        <Button onClick={() => openFor(null)}>
          <Plus className="size-4" aria-hidden /> Add {noun}
        </Button>
      </div>
      <QueryState query={query}>
        {query.data && <ResponsiveTable rows={query.data} rowKey={rowKey} columns={allColumns} empty={<EmptyState title={`No ${noun}s yet`} />} />}
      </QueryState>
      <Modal open={open} title={existing ? `Edit ${noun}` : `Add ${noun}`} onClose={() => setOpen(false)}>
        <form onSubmit={submit} className="space-y-4">
          {error != null && <ErrorNotice error={error} />}
          {fields(values, (key, value) => setValues((v) => ({ ...v, [key]: value })), existing)}
          <Button type="submit" loading={busy} className="w-full">
            Save
          </Button>
        </form>
      </Modal>
    </>
  )
}
