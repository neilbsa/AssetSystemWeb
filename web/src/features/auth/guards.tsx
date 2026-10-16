import { Navigate, Outlet, useLocation } from 'react-router'
import { Spinner } from '@/shared/ui'
import { useAuth } from './AuthProvider'

/** Signed in AND working in a company. Everything inside the app shell sits behind this. */
export function RequireSession() {
  const { status, company } = useAuth()
  const location = useLocation()

  if (status === 'loading') return <Spinner />
  if (status === 'anonymous') return <Navigate to="/login" replace state={{ from: location.pathname }} />
  if (!company) return <Navigate to="/select-company" replace />
  return <Outlet />
}

export function RequireAdministrator() {
  const { isAdministrator } = useAuth()
  return isAdministrator ? <Outlet /> : <Navigate to="/" replace />
}
