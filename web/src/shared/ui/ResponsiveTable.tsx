import type { ReactNode } from 'react'
import { Link } from 'react-router'
import { cn } from '@/shared/lib/cn'

export interface Column<T> {
  header: string
  cell: (row: T) => ReactNode
  /** The column used as the card title on phones. Defaults to the first one. */
  title?: boolean
  className?: string
}

interface ResponsiveTableProps<T> {
  rows: T[]
  columns: Column<T>[]
  rowKey: (row: T) => string
  /** Where clicking a row goes. */
  href?: (row: T) => string
  empty?: ReactNode
}

/**
 * One definition, two layouts: stacked cards on phones (no sideways scrolling),
 * a regular table from `md` up.
 */
export function ResponsiveTable<T>({ rows, columns, rowKey, href, empty }: ResponsiveTableProps<T>) {
  if (rows.length === 0) return <>{empty}</>

  const titleColumn = columns.find((c) => c.title) ?? columns[0]!
  const detailColumns = columns.filter((c) => c !== titleColumn)

  return (
    <>
      {/* Phones */}
      <ul className="space-y-2 md:hidden">
        {rows.map((row) => {
          const body = (
            <>
              <div className="font-medium">{titleColumn.cell(row)}</div>
              <dl className="mt-2 grid grid-cols-2 gap-x-3 gap-y-1.5">
                {detailColumns.map((c) => (
                  <div key={c.header} className="min-w-0">
                    <dt className="text-xs text-ink-muted">{c.header}</dt>
                    <dd className="truncate text-sm">{c.cell(row)}</dd>
                  </div>
                ))}
              </dl>
            </>
          )
          return (
            <li key={rowKey(row)}>
              {href ? (
                <Link to={href(row)} className="block rounded-xl border border-line bg-surface p-4 shadow-sm active:bg-surface-muted">
                  {body}
                </Link>
              ) : (
                <div className="rounded-xl border border-line bg-surface p-4 shadow-sm">{body}</div>
              )}
            </li>
          )
        })}
      </ul>

      {/* md and up */}
      <div className="hidden overflow-x-auto rounded-xl border border-line bg-surface shadow-sm md:block">
        <table className="w-full text-sm">
          <thead className="border-b border-line bg-surface-muted text-left text-xs text-ink-muted uppercase">
            <tr>
              {columns.map((c) => (
                <th key={c.header} scope="col" className={cn('px-4 py-3 font-medium', c.className)}>
                  {c.header}
                </th>
              ))}
            </tr>
          </thead>
          <tbody className="divide-y divide-line">
            {rows.map((row) => (
              <tr key={rowKey(row)} className={cn(href && 'hover:bg-surface-muted')}>
                {columns.map((c, i) => (
                  <td key={c.header} className={cn('px-4 py-3', c.className)}>
                    {href && i === 0 ? (
                      <Link to={href(row)} className="font-medium text-brand-600 hover:underline dark:text-brand-200">
                        {c.cell(row)}
                      </Link>
                    ) : (
                      c.cell(row)
                    )}
                  </td>
                ))}
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </>
  )
}
