import { MemoryRouter, Route, Routes } from 'react-router'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { AuthProvider } from './AuthProvider'
import { LoginPage } from './LoginPage'

const session = (companyId: string | null) => ({
  token: 'jwt',
  expiresAt: '2030-01-01T00:00:00Z',
  user: { id: 'u1', userName: 'neil', fullName: 'Neil A', email: 'n@x.com', roles: ['Common'] },
  companies: [{ id: 'c1', code: '09', name: 'Civic' }],
  companyId,
})

function renderLogin() {
  render(
    <QueryClientProvider client={new QueryClient()}>
      <MemoryRouter initialEntries={['/login']}>
        <AuthProvider>
          <Routes>
            <Route path="/login" element={<LoginPage />} />
            <Route path="/" element={<p>home</p>} />
            <Route path="/select-company" element={<p>pick a company</p>} />
          </Routes>
        </AuthProvider>
      </MemoryRouter>
    </QueryClientProvider>,
  )
}

describe('LoginPage', () => {
  beforeEach(() => {
    localStorage.clear()
    vi.restoreAllMocks()
  })

  it('signs in and goes straight home when the user has a single company', async () => {
    vi.spyOn(globalThis, 'fetch').mockResolvedValue(new Response(JSON.stringify(session('c1')), { status: 200 }))
    renderLogin()

    await userEvent.type(screen.getByLabelText('User name'), 'neil')
    await userEvent.type(screen.getByLabelText('Password'), 'Secret#123')
    await userEvent.click(screen.getByRole('button', { name: 'Sign in' }))

    expect(await screen.findByText('home')).toBeInTheDocument()
    expect(localStorage.getItem('assetsystem.token')).toBe('jwt')
  })

  it('asks for a company when none is selected yet', async () => {
    vi.spyOn(globalThis, 'fetch').mockResolvedValue(new Response(JSON.stringify(session(null)), { status: 200 }))
    renderLogin()

    await userEvent.type(screen.getByLabelText('User name'), 'neil')
    await userEvent.type(screen.getByLabelText('Password'), 'Secret#123')
    await userEvent.click(screen.getByRole('button', { name: 'Sign in' }))

    expect(await screen.findByText('pick a company')).toBeInTheDocument()
  })

  it('shows the server message when the credentials are wrong', async () => {
    vi.spyOn(globalThis, 'fetch').mockResolvedValue(new Response(JSON.stringify({ title: 'Invalid user name or password.' }), { status: 403 }))
    renderLogin()

    await userEvent.type(screen.getByLabelText('User name'), 'neil')
    await userEvent.type(screen.getByLabelText('Password'), 'nope')
    await userEvent.click(screen.getByRole('button', { name: 'Sign in' }))

    await waitFor(() => expect(screen.getByRole('alert')).toHaveTextContent('Invalid user name or password.'))
  })
})
