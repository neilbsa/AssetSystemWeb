import { useState } from 'react'
import { NavLink, Outlet, useNavigate } from 'react-router'
import { ArrowRightLeft, Boxes, LogOut, Menu } from 'lucide-react'
import { cn } from '@/shared/lib/cn'
import { Modal } from '@/shared/ui'
import { useAuth } from '@/features/auth/AuthProvider'
import { navItems, type NavItem } from './navigation'

/**
 * Phones: top bar + bottom tab bar (thumb reachable) + "More" sheet.
 * Desktop (lg+): fixed sidebar. Everything is hidden when printing.
 */
export function AppShell() {
  const { user, company, companies, isAdministrator, logout } = useAuth()
  const navigate = useNavigate()
  const [moreOpen, setMoreOpen] = useState(false)

  const visible = navItems.filter((i) => !i.adminOnly || isAdministrator)
  const primary = visible.filter((i) => i.primary)
  const secondary = visible.filter((i) => !i.primary)

  const switchCompany = () => navigate('/select-company')

  return (
    <div className="min-h-dvh lg:pl-64">
      {/* Desktop sidebar */}
      <aside className="fixed inset-y-0 left-0 hidden w-64 flex-col border-r border-line bg-surface lg:flex print:hidden">
        <div className="flex items-center gap-2 border-b border-line px-5 py-4">
          <Boxes className="size-6 text-brand-600" aria-hidden />
          <span className="font-semibold">Asset System</span>
        </div>
        <nav className="flex-1 space-y-1 overflow-y-auto p-3" aria-label="Main">
          {visible.map((item) => (
            <SideLink key={item.to} item={item} />
          ))}
        </nav>
        <div className="space-y-2 border-t border-line p-3 text-sm">
          <div className="px-2">
            <p className="font-medium">{user?.fullName}</p>
            <p className="truncate text-xs text-ink-muted">{company?.name}</p>
          </div>
          {companies.length > 1 && (
            <button type="button" onClick={switchCompany} className="flex w-full items-center gap-2 rounded-lg px-2 py-2 text-ink-muted hover:bg-surface-muted hover:text-ink">
              <ArrowRightLeft className="size-4" aria-hidden /> Switch company
            </button>
          )}
          <button type="button" onClick={logout} className="flex w-full items-center gap-2 rounded-lg px-2 py-2 text-ink-muted hover:bg-surface-muted hover:text-ink">
            <LogOut className="size-4" aria-hidden /> Sign out
          </button>
        </div>
      </aside>

      {/* Phone top bar */}
      <header className="sticky top-0 z-30 flex items-center justify-between border-b border-line bg-surface/95 px-4 py-3 backdrop-blur lg:hidden print:hidden">
        <div className="flex min-w-0 items-center gap-2">
          <Boxes className="size-5 shrink-0 text-brand-600" aria-hidden />
          <span className="truncate font-semibold">{company?.name}</span>
        </div>
        <button type="button" onClick={() => setMoreOpen(true)} aria-label="Open menu" className="-mr-2 rounded-lg p-2 text-ink-muted hover:bg-surface-muted">
          <Menu className="size-5" />
        </button>
      </header>

      <main className="mx-auto w-full max-w-5xl px-4 pt-4 pb-28 lg:px-8 lg:py-8 print:p-0">
        <Outlet />
      </main>

      {/* Phone bottom tab bar */}
      <nav
        aria-label="Main"
        className="fixed inset-x-0 bottom-0 z-30 grid grid-cols-5 border-t border-line bg-surface/95 pb-[env(safe-area-inset-bottom)] backdrop-blur lg:hidden print:hidden"
      >
        {primary.map((item) => (
          <TabLink key={item.to} item={item} />
        ))}
        <button type="button" onClick={() => setMoreOpen(true)} className="flex min-h-14 flex-col items-center justify-center gap-0.5 text-xs text-ink-muted">
          <Menu className="size-5" aria-hidden />
          More
        </button>
      </nav>

      <Modal open={moreOpen} title="Menu" onClose={() => setMoreOpen(false)}>
        <div className="space-y-1">
          {secondary.map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              onClick={() => setMoreOpen(false)}
              className="flex min-h-12 items-center gap-3 rounded-lg px-3 hover:bg-surface-muted"
            >
              <item.icon className="size-5 text-ink-muted" aria-hidden /> {item.label}
            </NavLink>
          ))}
          {companies.length > 1 && (
            <button type="button" onClick={switchCompany} className="flex min-h-12 w-full items-center gap-3 rounded-lg px-3 hover:bg-surface-muted">
              <ArrowRightLeft className="size-5 text-ink-muted" aria-hidden /> Switch company
            </button>
          )}
          <button type="button" onClick={logout} className="flex min-h-12 w-full items-center gap-3 rounded-lg px-3 text-red-600 hover:bg-surface-muted">
            <LogOut className="size-5" aria-hidden /> Sign out ({user?.userName})
          </button>
        </div>
      </Modal>
    </div>
  )
}

function SideLink({ item }: { item: NavItem }) {
  return (
    <NavLink
      to={item.to}
      end={item.end}
      className={({ isActive }) =>
        cn('flex min-h-10 items-center gap-3 rounded-lg px-3 text-sm font-medium', isActive ? 'bg-brand-50 text-brand-700 dark:bg-brand-600/20 dark:text-brand-200' : 'text-ink-muted hover:bg-surface-muted hover:text-ink')
      }
    >
      <item.icon className="size-5" aria-hidden /> {item.label}
    </NavLink>
  )
}

function TabLink({ item }: { item: NavItem }) {
  return (
    <NavLink
      to={item.to}
      end={item.end}
      className={({ isActive }) => cn('flex min-h-14 flex-col items-center justify-center gap-0.5 text-xs', isActive ? 'text-brand-600 dark:text-brand-200' : 'text-ink-muted')}
    >
      <item.icon className="size-5" aria-hidden />
      {item.label}
    </NavLink>
  )
}
