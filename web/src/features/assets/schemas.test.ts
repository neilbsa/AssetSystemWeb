import { emptyItem, registerAssetSchema } from './schemas'

const valid = {
  category: 'Company',
  branchId: 'b1',
  description: 'Desktop',
  vendor: 'Dell',
  purchaseOrderNumber: '',
  controlNumber: '',
  remarks: '',
  price: 1000,
  lifeSpanMonths: 36,
  warrantyMonths: 12,
  invoiceNumber: 'INV-1',
  invoiceDate: '2026-01-01',
  supplier: { name: 'PC Express', address: 'Davao', contactNumber: '', email: 's@pcx.com' },
  items: [{ ...emptyItem, typeCode: 'M', description: 'Monitor', partNumber: 'P2422H' }],
}

describe('registerAssetSchema', () => {
  it('accepts a complete asset (serial number and key may be blank: they become N/A)', () => {
    expect(registerAssetSchema.safeParse(valid).success).toBe(true)
  })

  it('requires at least one item', () => {
    const result = registerAssetSchema.safeParse({ ...valid, items: [] })
    expect(result.success).toBe(false)
  })

  it('requires the item type and part number', () => {
    const result = registerAssetSchema.safeParse({ ...valid, items: [{ ...emptyItem, description: 'Monitor' }] })
    expect(result.success).toBe(false)
    expect(result.error?.issues.map((i) => i.path.join('.'))).toEqual(expect.arrayContaining(['items.0.typeCode', 'items.0.partNumber']))
  })

  it('rejects an invalid supplier email and a negative price', () => {
    const result = registerAssetSchema.safeParse({ ...valid, price: -5, supplier: { ...valid.supplier, email: 'nope' } })
    expect(result.success).toBe(false)
    expect(result.error?.issues.map((i) => i.path.join('.'))).toEqual(expect.arrayContaining(['price', 'supplier.email']))
  })
})
