import { cn } from '@/shared/lib/cn'

interface SegmentedProps<T extends string> {
  value: T
  options: { value: T; label: string }[]
  onChange: (value: T) => void
  label: string
}

/** Small toggle group for filters (e.g. Company / Salesman). */
export function Segmented<T extends string>({ value, options, onChange, label }: SegmentedProps<T>) {
  return (
    <div role="group" aria-label={label} className="inline-flex w-full rounded-lg border border-line bg-surface p-0.5 sm:w-auto">
      {options.map((o) => (
        <button
          key={o.value}
          type="button"
          aria-pressed={o.value === value}
          onClick={() => onChange(o.value)}
          className={cn(
            'min-h-10 flex-1 rounded-md px-3 text-sm font-medium transition-colors sm:flex-none',
            o.value === value ? 'bg-brand-600 text-white' : 'text-ink-muted hover:text-ink',
          )}
        >
          {o.label}
        </button>
      ))}
    </div>
  )
}
