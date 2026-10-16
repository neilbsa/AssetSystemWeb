import { Link } from 'react-router'

export function NotFoundPage() {
  return (
    <main className="grid min-h-dvh place-items-center px-4 text-center">
      <div className="space-y-3">
        <p className="text-5xl font-semibold text-brand-600">404</p>
        <p className="text-ink-muted">We couldn't find that page.</p>
        <Link to="/" className="inline-block font-medium text-brand-600 hover:underline">
          Back to home
        </Link>
      </div>
    </main>
  )
}
