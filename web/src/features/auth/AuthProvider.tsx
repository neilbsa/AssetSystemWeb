import { createContext, useCallback, useContext, useEffect, useMemo, useState, type ReactNode } from 'react'
import { useQueryClient } from '@tanstack/react-query'
import { api, setUnauthorizedHandler, tokenStore } from '@/shared/api/http'
import type { CompanyInfo, SessionResponse, UserInfo } from './types'

type Status = 'loading' | 'anonymous' | 'authenticated'

interface AuthContextValue {
  status: Status
  user: UserInfo | null
  companies: CompanyInfo[]
  company: CompanyInfo | null
  isAdministrator: boolean
  login: (userName: string, password: string) => Promise<SessionResponse>
  selectCompany: (companyId: string) => Promise<void>
  /** Re-reads the session (e.g. after an administrator created the first company). */
  refresh: () => Promise<void>
  logout: () => void
}

const AuthContext = createContext<AuthContextValue | null>(null)

export function AuthProvider({ children }: { children: ReactNode }) {
  const queryClient = useQueryClient()
  const [status, setStatus] = useState<Status>(tokenStore.get() ? 'loading' : 'anonymous')
  const [session, setSession] = useState<SessionResponse | null>(null)

  const logout = useCallback(() => {
    tokenStore.clear()
    setSession(null)
    setStatus('anonymous')
    queryClient.clear() // never show one user's cached data to the next
  }, [queryClient])

  const accept = useCallback((next: SessionResponse) => {
    if (next.token) tokenStore.set(next.token)
    setSession(next)
    setStatus('authenticated')
  }, [])

  useEffect(() => setUnauthorizedHandler(logout), [logout])

  // Restore the session after a page reload.
  useEffect(() => {
    if (!tokenStore.get()) return
    api<SessionResponse>('/auth/session')
      .then((s) => {
        setSession(s)
        setStatus('authenticated')
      })
      .catch(logout)
  }, [logout])

  const login = useCallback(
    async (userName: string, password: string) => {
      const next = await api<SessionResponse>('/auth/login', { method: 'POST', body: { userName, password } })
      accept(next)
      return next
    },
    [accept],
  )

  const selectCompany = useCallback(
    async (companyId: string) => {
      const next = await api<SessionResponse>('/auth/select-company', { method: 'POST', body: { companyId } })
      queryClient.clear() // data belongs to the previous company
      accept(next)
    },
    [accept, queryClient],
  )

  const refresh = useCallback(async () => {
    setSession(await api<SessionResponse>('/auth/session'))
  }, [])

  const value = useMemo<AuthContextValue>(() => {
    const companies = session?.companies ?? []
    return {
      status,
      user: session?.user ?? null,
      companies,
      company: companies.find((c) => c.id === session?.companyId) ?? null,
      isAdministrator: session?.user.roles.includes('Administrator') ?? false,
      login,
      selectCompany,
      refresh,
      logout,
    }
  }, [status, session, login, selectCompany, refresh, logout])

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used inside <AuthProvider>')
  return ctx
}
