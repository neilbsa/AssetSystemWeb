import { useState } from 'react'
import { UserRound, X } from 'lucide-react'
import { Button, SearchInput, Spinner } from '@/shared/ui'
import { useDebounced } from '@/shared/hooks/useDebounced'
import { useEmployees } from '@/features/organization/api'
import type { Employee } from '@/features/organization/types'

interface EmployeePickerProps {
  value: Employee | null
  onChange: (employee: Employee | null) => void
}

/** Search by name or employee number, tap a result to choose. */
export function EmployeePicker({ value, onChange }: EmployeePickerProps) {
  const [search, setSearch] = useState('')
  const employees = useEmployees(useDebounced(search))

  if (value) {
    return (
      <div className="flex items-center gap-3 rounded-lg border border-line p-3">
        <UserRound className="size-8 shrink-0 rounded-full bg-brand-50 p-1.5 text-brand-600 dark:bg-brand-600/20" aria-hidden />
        <div className="min-w-0 flex-1">
          <p className="truncate font-medium">{value.fullName}</p>
          <p className="truncate text-xs text-ink-muted">
            #{value.employeeNumber} · {value.departmentName ?? 'No department'}
          </p>
        </div>
        <Button size="sm" variant="ghost" aria-label="Choose someone else" onClick={() => onChange(null)}>
          <X className="size-4" aria-hidden />
        </Button>
      </div>
    )
  }

  return (
    <div className="space-y-2">
      <SearchInput value={search} onChange={setSearch} placeholder="Search employee name or number…" />
      {employees.isFetching && <Spinner label="" />}
      {employees.data && (
        <ul className="max-h-64 divide-y divide-line overflow-y-auto rounded-lg border border-line">
          {employees.data.length === 0 && <li className="p-3 text-sm text-ink-muted">No employee found.</li>}
          {employees.data.slice(0, 20).map((e) => (
            <li key={e.id}>
              <button type="button" onClick={() => onChange(e)} className="flex min-h-12 w-full flex-col items-start px-3 py-2 text-left hover:bg-surface-muted">
                <span className="text-sm font-medium">{e.fullName}</span>
                <span className="text-xs text-ink-muted">
                  #{e.employeeNumber} · {e.departmentName ?? 'No department'}
                </span>
              </button>
            </li>
          ))}
        </ul>
      )}
    </div>
  )
}
