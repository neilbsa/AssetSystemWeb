import { useState, type FormEvent } from 'react'
import { useNavigate } from 'react-router'
import { Plus, Trash2 } from 'lucide-react'
import { Button, EmptyState, ErrorNotice, PageHeader, Section, TextAreaField, TextField, useToast, errorMessage } from '@/shared/ui'
import { StatusBadge } from '@/features/assets/components/StatusBadge'
import { lookupAvailableItems } from '@/features/assets/api'
import type { ItemRow } from '@/features/assets/types'
import type { Employee } from '@/features/organization/types'
import { todayIso } from '@/shared/lib/format'
import { useIssueConsignment } from '../api'
import { EmployeePicker } from '../components/EmployeePicker'

export function ConsignmentCreatePage() {
  const navigate = useNavigate()
  const toast = useToast()
  const issue = useIssueConsignment()

  const [employee, setEmployee] = useState<Employee | null>(null)
  const [issuedOn, setIssuedOn] = useState(todayIso())
  const [remarks, setRemarks] = useState('')
  const [items, setItems] = useState<ItemRow[]>([])
  const [code, setCode] = useState('')
  const [lookupError, setLookupError] = useState<string | null>(null)
  const [looking, setLooking] = useState(false)

  /** An asset number adds all of that asset's available items; an item code adds one item. */
  async function addByCode() {
    if (!code.trim()) return
    setLooking(true)
    setLookupError(null)
    try {
      const found = await lookupAvailableItems(code.trim())
      if (found.length === 0) {
        setLookupError(`No available item or asset matches "${code.trim()}".`)
        return
      }
      setItems((current) => [...current, ...found.filter((f) => !current.some((c) => c.id === f.id))])
      setCode('')
    } catch (err) {
      setLookupError(errorMessage(err))
    } finally {
      setLooking(false)
    }
  }

  const canSubmit = employee !== null && items.length > 0

  async function submit(e: FormEvent) {
    e.preventDefault()
    if (!employee) return
    const created = await issue
      .mutateAsync({ employeeId: employee.id, issuedOn, remarks, itemIds: items.map((i) => i.id) })
      .catch(() => null)
    if (!created) return
    toast.success(`Consignment #${created.number} issued`)
    navigate(`/consignments/${created.id}`)
  }

  return (
    <>
      <PageHeader title="New consignment" subtitle="Hand equipment to an employee" back="/consignments" />
      <form onSubmit={submit} className="space-y-4">
        {issue.isError && <ErrorNotice error={issue.error} />}

        <Section title="Consignee">
          <EmployeePicker value={employee} onChange={setEmployee} />
        </Section>

        <Section title="Items">
          {/* Not a nested <form>: Enter in the code box adds the item instead of submitting the consignment. */}
          <div className="flex gap-2">
            <TextField
              label="Asset number or item code"
              containerClassName="flex-1"
              placeholder="e.g. 2CS0001 or CVCDVOM-001"
              autoCapitalize="characters"
              value={code}
              onChange={(e) => setCode(e.target.value)}
              onKeyDown={(e) => {
                if (e.key === 'Enter') {
                  e.preventDefault()
                  addByCode()
                }
              }}
            />
            <Button variant="secondary" loading={looking} className="mt-[26px]" onClick={addByCode}>
              <Plus className="size-4" aria-hidden /> Add
            </Button>
          </div>
          {lookupError && <p className="mt-2 text-sm text-red-600 dark:text-red-400">{lookupError}</p>}

          {items.length === 0 ? (
            <EmptyState title="No items yet" description="Only items with status Available can be issued." />
          ) : (
            <ul className="mt-4 divide-y divide-line rounded-lg border border-line">
              {items.map((item) => (
                <li key={item.id} className="flex items-center gap-3 p-3">
                  <div className="min-w-0 flex-1">
                    <p className="text-sm font-medium">{item.itemCode}</p>
                    <p className="truncate text-xs text-ink-muted">
                      {item.typeName} · {item.description} · {item.serialNumber}
                    </p>
                  </div>
                  <StatusBadge status={item.status} />
                  <Button size="sm" variant="ghost" aria-label={`Remove ${item.itemCode}`} onClick={() => setItems((c) => c.filter((x) => x.id !== item.id))}>
                    <Trash2 className="size-4" aria-hidden />
                  </Button>
                </li>
              ))}
            </ul>
          )}
        </Section>

        <Section title="Details">
          <div className="grid gap-4 sm:grid-cols-2">
            <TextField label="Date issued" type="date" max={todayIso()} required value={issuedOn} onChange={(e) => setIssuedOn(e.target.value)} />
            <TextAreaField label="Remarks" rows={2} value={remarks} onChange={(e) => setRemarks(e.target.value)} />
          </div>
        </Section>

        <Button type="submit" loading={issue.isPending} disabled={!canSubmit} className="w-full sm:w-auto">
          Issue {items.length > 0 ? `${items.length} item${items.length > 1 ? 's' : ''}` : 'consignment'}
        </Button>
      </form>
    </>
  )
}
