import { createBrowserRouter, type RouteObject } from 'react-router'
import { AppShell } from './AppShell'
import { RequireAdministrator, RequireSession } from '@/features/auth/guards'
import { LoginPage } from '@/features/auth/LoginPage'
import { SelectCompanyPage } from '@/features/auth/SelectCompanyPage'

/** Pages are loaded on demand so the first screen stays small on mobile networks. */
const page = <T extends Record<string, unknown>>(load: () => Promise<T>, name: keyof T): Pick<RouteObject, 'lazy'> => ({
  lazy: async () => ({ Component: (await load())[name] as React.ComponentType }),
})

export const router = createBrowserRouter([
  { path: '/login', element: <LoginPage /> },
  { path: '/select-company', element: <SelectCompanyPage /> },
  {
    element: <RequireSession />,
    children: [
      {
        element: <AppShell />,
        children: [
          { index: true, ...page(() => import('@/features/dashboard/DashboardPage'), 'DashboardPage') },

          { path: 'assets', ...page(() => import('@/features/assets/pages/AssetListPage'), 'AssetListPage') },
          { path: 'assets/new', ...page(() => import('@/features/assets/pages/AssetCreatePage'), 'AssetCreatePage') },
          { path: 'assets/:id', ...page(() => import('@/features/assets/pages/AssetDetailPage'), 'AssetDetailPage') },
          { path: 'assets/:id/edit', ...page(() => import('@/features/assets/pages/AssetEditPage'), 'AssetEditPage') },
          { path: 'items', ...page(() => import('@/features/assets/pages/ItemListPage'), 'ItemListPage') },
          { path: 'items/:id', ...page(() => import('@/features/assets/pages/ItemDetailPage'), 'ItemDetailPage') },

          { path: 'consignments', ...page(() => import('@/features/consignments/pages/ConsignmentListPage'), 'ConsignmentListPage') },
          { path: 'consignments/new', ...page(() => import('@/features/consignments/pages/ConsignmentCreatePage'), 'ConsignmentCreatePage') },
          { path: 'consignments/:id', ...page(() => import('@/features/consignments/pages/ConsignmentDetailPage'), 'ConsignmentDetailPage') },
          { path: 'consignees', ...page(() => import('@/features/consignments/pages/ConsigneeListPage'), 'ConsigneeListPage') },

          { path: 'employees', ...page(() => import('@/features/employees/EmployeesPage'), 'EmployeesPage') },
          { path: 'account', ...page(() => import('@/features/auth/ChangePasswordPage'), 'ChangePasswordPage') },

          {
            element: <RequireAdministrator />,
            children: [
              { path: 'admin/users', ...page(() => import('@/features/admin/UsersPage'), 'UsersPage') },
              { path: 'admin/organization', ...page(() => import('@/features/admin/OrganizationPage'), 'OrganizationPage') },
            ],
          },
        ],
      },
    ],
  },
  { path: '*', ...page(() => import('@/app/NotFoundPage'), 'NotFoundPage') },
])
