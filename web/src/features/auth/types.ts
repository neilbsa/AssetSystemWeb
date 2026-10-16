export interface UserInfo {
  id: string
  userName: string
  fullName: string
  email: string
  roles: string[]
}

export interface CompanyInfo {
  id: string
  code: string
  name: string
}

export interface SessionResponse {
  token: string | null
  expiresAt: string | null
  user: UserInfo
  companies: CompanyInfo[]
  /** The company the user is currently working in; null until one is selected. */
  companyId: string | null
}
