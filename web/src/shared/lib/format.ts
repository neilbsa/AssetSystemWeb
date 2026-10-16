const money = new Intl.NumberFormat('en-PH', { style: 'currency', currency: 'PHP' })
const dateFmt = new Intl.DateTimeFormat('en-PH', { year: 'numeric', month: 'short', day: 'numeric' })

export const formatMoney = (value: number) => money.format(value)

/** Accepts an ISO date ("2026-10-01") or timestamp; date-only strings are not shifted by the time zone. */
export function formatDate(value: string | null | undefined): string {
  if (!value) return '—'
  const dateOnly = /^\d{4}-\d{2}-\d{2}$/.test(value)
  const date = new Date(dateOnly ? `${value}T00:00:00` : value)
  return Number.isNaN(date.getTime()) ? '—' : dateFmt.format(date)
}

export const todayIso = () => {
  const d = new Date()
  const pad = (n: number) => String(n).padStart(2, '0')
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`
}

export const formatBytes = (bytes: number) =>
  bytes < 1024 ? `${bytes} B` : bytes < 1024 * 1024 ? `${(bytes / 1024).toFixed(1)} KB` : `${(bytes / 1024 / 1024).toFixed(1)} MB`
