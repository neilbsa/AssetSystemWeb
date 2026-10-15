import { z } from 'zod'

const requiredText = (label: string, max = 200) =>
  z.string().trim().min(1, `${label} is required`).max(max, `${label} is too long`)

const optionalText = (max = 500) => z.string().trim().max(max, 'Too long')

const number = (label: string, min = 0, max = Number.MAX_SAFE_INTEGER) =>
  z.number({ error: `${label} must be a number` }).min(min, `${label} cannot be below ${min}`).max(max, `${label} is too large`)

export const supplierSchema = z.object({
  name: requiredText('Supplier name'),
  address: requiredText('Supplier address', 500),
  contactNumber: optionalText(50),
  email: z.email('Enter a valid email address'),
})

export const newItemSchema = z.object({
  typeCode: requiredText('Item type', 20),
  description: requiredText('Description'),
  partNumber: requiredText('Part number', 100),
  serialNumber: optionalText(100),
  keyDetail: optionalText(200),
  controlNumber: optionalText(100),
  remarks: optionalText(),
})

export const assetDetailsSchema = z.object({
  description: requiredText('Description'),
  vendor: requiredText('Vendor'),
  purchaseOrderNumber: optionalText(100),
  controlNumber: optionalText(100),
  remarks: optionalText(),
  price: number('Price'),
  lifeSpanMonths: number('Life span', 0, 600),
  warrantyMonths: number('Warranty', 0, 600),
  invoiceNumber: requiredText('Invoice number', 100),
  invoiceDate: z.string().min(1, 'Invoice date is required'),
  supplier: supplierSchema,
})

export const registerAssetSchema = assetDetailsSchema.extend({
  category: z.enum(['Company', 'Salesman']),
  branchId: z.string().min(1, 'Choose a branch'),
  items: z.array(newItemSchema).min(1, 'Add at least one item'),
})

export type AssetDetailsForm = z.infer<typeof assetDetailsSchema>
export type RegisterAssetForm = z.infer<typeof registerAssetSchema>
export type NewItemForm = z.infer<typeof newItemSchema>

export const emptyItem: NewItemForm = {
  typeCode: '',
  description: '',
  partNumber: '',
  serialNumber: '',
  keyDetail: '',
  controlNumber: '',
  remarks: '',
}
