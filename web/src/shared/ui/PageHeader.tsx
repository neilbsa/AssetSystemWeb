import type { ReactNode } from 'react'
import { Link } from 'react-router'
import { ChevronLeft } from 'lucide-react'

interface PageHeaderProps {
  title: string
  subtitle?: ReactNode
  back?: string
  actions?: ReactNode
}

export function PageHeader({ title, subtitle, back, actions }: PageHeaderProps) {
  return (
    <header className="mb-4 space-y-2 print:mb-2">
      {back && (
        <Link to={back} className="inline-flex items-center gap-1 text-sm text-ink-muted hover:text-ink print:hidden">
          <ChevronLeft className="size-4" aria-hidden /> Back
        </Link>
      )}
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div className="min-w-0">
          <h1 className="truncate text-xl font-semibold sm:text-2xl">{title}</h1>
          {subtitle && <div className="text-sm text-ink-muted">{subtitle}</div>}
        </div>
        {actions && <div className="flex flex-wrap items-center gap-2 print:hidden">{actions}</div>}
      </div>
    </header>
  )
}
