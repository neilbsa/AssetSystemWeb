export interface Branch {
  id: string
  name: string
  address: string
  assetTag: string
}

export interface Department {
  id: string
  code: string
  name: string
}

export interface ItemType {
  code: string
  name: string
}

export interface Supplier {
  id: string
  name: string
  address: string
  contactNumber: string | null
  email: string
}

export interface Company {
  id: string
  code: string
  name: string
  address: string
  assetTag: string
  isActive: boolean
}

export interface Employee {
  id: string
  employeeNumber: string
  firstName: string
  middleName: string | null
  lastName: string
  fullName: string
  email: string
  departmentId: string
  departmentName: string | null
  branchId: string
  isActive: boolean
}
