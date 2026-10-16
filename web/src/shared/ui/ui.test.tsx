import { MemoryRouter } from 'react-router'
import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { Pagination } from './Pagination'
import { ResponsiveTable } from './ResponsiveTable'
import { TextField } from './Field'

describe('Pagination', () => {
  it('renders nothing for a single page', () => {
    const { container } = render(<Pagination page={1} pageSize={20} total={5} onChange={() => {}} />)
    expect(container).toBeEmptyDOMElement()
  })

  it('moves between pages and disables the ends', async () => {
    const onChange = vi.fn()
    render(<Pagination page={1} pageSize={20} total={45} onChange={onChange} />)

    expect(screen.getByRole('button', { name: /prev/i })).toBeDisabled()
    await userEvent.click(screen.getByRole('button', { name: /next/i }))
    expect(onChange).toHaveBeenCalledWith(2)
    expect(screen.getByText('Page 1 of 3')).toBeInTheDocument()
  })
})

describe('ResponsiveTable', () => {
  const rows = [{ id: '1', name: 'Dell monitor', code: 'M-001' }]
  const columns = [
    { header: 'Code', cell: (r: (typeof rows)[0]) => r.code, title: true },
    { header: 'Name', cell: (r: (typeof rows)[0]) => r.name },
  ]

  it('renders the same data as phone cards and as a table', () => {
    render(
      <MemoryRouter>
        <ResponsiveTable rows={rows} columns={columns} rowKey={(r) => r.id} href={(r) => `/items/${r.id}`} />
      </MemoryRouter>,
    )

    expect(screen.getAllByText('M-001')).toHaveLength(2) // card title + table cell
    expect(screen.getAllByRole('link').every((l) => l.getAttribute('href') === '/items/1')).toBe(true)
    expect(screen.getByRole('columnheader', { name: 'Name' })).toBeInTheDocument()
  })

  it('shows the empty state when there are no rows', () => {
    render(<ResponsiveTable rows={[]} columns={columns} rowKey={(r: (typeof rows)[0]) => r.id} empty={<p>Nothing here</p>} />)
    expect(screen.getByText('Nothing here')).toBeInTheDocument()
  })
})

describe('TextField', () => {
  it('links the label, the input and the error message', () => {
    render(<TextField label="Serial number" error="Already registered" />)

    const input = screen.getByLabelText('Serial number')
    expect(input).toHaveAttribute('aria-invalid', 'true')
    expect(input).toHaveAccessibleDescription('Already registered')
  })
})
