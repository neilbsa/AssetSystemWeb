import type { ReactNode } from 'react'
import { Boxes } from 'lucide-react'

/** Centred card used by the sign-in and company picker screens. */
export function AuthLayout({ title, subtitle, children }: { title: string; subtitle?: string; children: ReactNode }) {
  return (
    <main className="flex min-h-dvh flex-col items-center justify-center px-4 py-10">
      <div className="w-full max-w-sm space-y-6">
        <div className="text-center">
          <div className="mx-auto mb-3 grid size-12 place-items-center rounded-xl bg-brand-600 text-white">
            <Boxes className="size-6" aria-hidden />
          </div>
          <h1 className="text-2xl font-semibold">{title}</h1>
          {subtitle && <p className="mt-1 text-sm text-ink-muted">{subtitle}</p>}
        </div>
        <div className="rounded-xl border border-line bg-surface p-5 shadow-sm">{children}</div>
      </div>
    </main>
  )
}
