import { formatBytes, formatDate, formatMoney } from './format'

describe('formatDate', () => {
  it('does not shift date-only values across time zones', () => {
    expect(formatDate('2026-10-01')).toBe('Oct 1, 2026')
  })

  it('shows a dash for missing or invalid dates', () => {
    expect(formatDate(null)).toBe('—')
    expect(formatDate('not a date')).toBe('—')
  })
})

describe('formatMoney', () => {
  it('formats Philippine pesos', () => {
    expect(formatMoney(1234.5)).toContain('1,234.50')
  })
})

describe('formatBytes', () => {
  it.each([
    [512, '512 B'],
    [2048, '2.0 KB'],
    [5 * 1024 * 1024, '5.0 MB'],
  ])('formats %i bytes', (bytes, expected) => {
    expect(formatBytes(bytes)).toBe(expected)
  })
})
