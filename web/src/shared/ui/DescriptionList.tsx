import type { ReactNode } from 'react'

/** Label/value pairs: stacked on phones, two columns on wider screens. */
export function DescriptionList({ items }: { items: { label: string; value: ReactNode }[] }) {
  return (
    <dl className="grid gap-x-6 gap-y-3 sm:grid-cols-2">
      {items.map((i) => (
        <div key={i.label}>
          <dt className="text-xs text-ink-muted">{i.label}</dt>
          <dd className="text-sm font-medium break-words">{i.value || '—'}</dd>
        </div>
      ))}
    </dl>
  )
}
