import { Boxes, Building2, ClipboardList, KeyRound, LayoutDashboard, Package, Users, UserCog, type LucideIcon } from 'lucide-react'

export interface NavItem {
  to: string
  label: string
  icon: LucideIcon
  /** Show in the phone's bottom bar; everything else goes in the "More" sheet. */
  primary?: boolean
  adminOnly?: boolean
  end?: boolean
}

export const navItems: NavItem[] = [
  { to: '/', label: 'Home', icon: LayoutDashboard, primary: true, end: true },
  { to: '/assets', label: 'Assets', icon: Boxes, primary: true },
  { to: '/items', label: 'Items', icon: Package, primary: true },
  { to: '/consignments', label: 'Consignments', icon: ClipboardList, primary: true },
  { to: '/employees', label: 'Employees', icon: Users },
  { to: '/admin/organization', label: 'Organization', icon: Building2, adminOnly: true },
  { to: '/admin/users', label: 'System users', icon: UserCog, adminOnly: true },
  { to: '/account', label: 'Account', icon: KeyRound },
]
