export type ConsignmentStatus = 'Open' | 'Cancelled'
export type LineStatus = 'Active' | 'Superseded' | 'Cancelled'

export interface ConsignmentSummary {
  id: string
  number: string
  consigneeId: string
  consigneeNumber: string
  consigneeName: string
  issuedOn: string
  status: ConsignmentStatus
  itemCount: number
  hasReceivingCopy: boolean
}

export interface ConsigneeSummary {
  consigneeId: string
  consigneeNumber: string
  consigneeName: string
  departmentName: string | null
  consignmentCount: number
  activeItemCount: number
}

export interface ConsignmentLine {
  itemId: string
  itemCode: string
  typeName: string
  description: string
  serialNumber: string
  assetNumber: string
  status: LineStatus
}

export interface ConsignmentDetails {
  id: string
  number: string
  consigneeId: string
  consigneeNumber: string
  consigneeName: string
  departmentName: string | null
  issuedOn: string
  remarks: string | null
  status: ConsignmentStatus
  cancellationReason: string | null
  receivingCopy: { fileName: string; contentType: string; sizeBytes: number } | null
  lines: ConsignmentLine[]
  createdAt: string
  createdBy: string
}

export interface IssueConsignmentInput {
  employeeId: string
  issuedOn: string
  remarks: string
  itemIds: string[]
}
