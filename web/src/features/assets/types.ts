export type AssetCategory = 'Company' | 'Salesman'
export type ItemStatus = 'Available' | 'InUse' | 'Defective'

export interface Paged<T> {
  items: T[]
  total: number
  page: number
  pageSize: number
}

export interface AssetSummary {
  id: string
  assetNumber: string
  category: AssetCategory
  description: string
  vendor: string
  itemCount: number
  availableCount: number
  purchaseDate: string
  price: number
}

/** One item with the asset it belongs to (inventory rows, lookups). */
export interface ItemRow {
  id: string
  itemCode: string
  typeCode: string
  typeName: string
  description: string
  partNumber: string
  serialNumber: string
  keyDetail: string
  controlNumber: string | null
  remarks: string | null
  status: ItemStatus
  assetId: string
  assetNumber: string
  assetDescription: string
}

export interface AssetItem {
  id: string
  itemCode: string
  typeCode: string
  typeName: string
  description: string
  partNumber: string
  serialNumber: string
  keyDetail: string
  controlNumber: string | null
  remarks: string | null
  status: ItemStatus
}

export interface AssetDocument {
  id: string
  documentType: string
  fileName: string
  contentType: string
  sizeBytes: number
}

export interface AssetDetails {
  id: string
  assetNumber: string
  category: AssetCategory
  branchId: string
  branchName: string
  description: string
  vendor: string
  purchaseOrderNumber: string | null
  controlNumber: string | null
  remarks: string | null
  price: number
  purchaseDate: string
  lifeSpanMonths: number
  warrantyMonths: number
  warrantyEndsOn: string
  depreciatedValue: number
  invoiceNumber: string
  invoiceDate: string
  supplierId: string
  supplierName: string
  items: AssetItem[]
  documents: AssetDocument[]
  createdAt: string
  createdBy: string
}

export interface SupplierInput {
  name: string
  address: string
  contactNumber: string
  email: string
}

export interface NewItemInput {
  typeCode: string
  description: string
  partNumber: string
  serialNumber: string
  keyDetail: string
  controlNumber: string
  remarks: string
}

export interface AssetDetailsInput {
  description: string
  vendor: string
  purchaseOrderNumber: string
  controlNumber: string
  remarks: string
  price: number
  lifeSpanMonths: number
  warrantyMonths: number
  invoiceNumber: string
  invoiceDate: string
  supplier: SupplierInput
}

export interface RegisterAssetInput extends AssetDetailsInput {
  category: AssetCategory
  branchId: string
  items: NewItemInput[]
}

export interface RegisteredAsset {
  id: string
  assetNumber: string
  itemCodes: string[]
}

export interface ItemConsignment {
  consignmentId: string
  consignmentNumber: string
  consigneeName: string
  issuedOn: string
  consignmentStatus: string
  lineStatus: string
}

export interface ItemHistory {
  item: ItemRow
  consignments: ItemConsignment[]
}

export interface Dashboard {
  assets: number
  items: number
  itemsByStatus: { status: ItemStatus; count: number }[]
}

export const itemStatusLabel: Record<ItemStatus, string> = {
  Available: 'Available',
  InUse: 'In use',
  Defective: 'Defective',
}
