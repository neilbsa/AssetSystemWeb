import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { api } from '@/shared/api/http'
import type { Branch, Company, Department, Employee, ItemType, Supplier } from './types'

/** Reference data changes rarely, so keep it fresh for longer than the default. */
const REFERENCE_STALE_MS = 5 * 60_000

export const orgKeys = {
  branches: ['organization', 'branches'] as const,
  departments: ['organization', 'departments'] as const,
  itemTypes: ['organization', 'item-types'] as const,
  companies: ['organization', 'companies'] as const,
  employees: (search: string) => ['organization', 'employees', search] as const,
  suppliers: (q: string) => ['organization', 'suppliers', q] as const,
}

export const useBranches = () =>
  useQuery({ queryKey: orgKeys.branches, queryFn: () => api<Branch[]>('/organization/branches'), staleTime: REFERENCE_STALE_MS })

export const useDepartments = () =>
  useQuery({ queryKey: orgKeys.departments, queryFn: () => api<Department[]>('/organization/departments'), staleTime: REFERENCE_STALE_MS })

export const useItemTypes = () =>
  useQuery({ queryKey: orgKeys.itemTypes, queryFn: () => api<ItemType[]>('/organization/item-types'), staleTime: REFERENCE_STALE_MS })

export const useCompanies = () =>
  useQuery({ queryKey: orgKeys.companies, queryFn: () => api<Company[]>('/organization/companies') })

export const useSuppliers = (q: string) =>
  useQuery({
    queryKey: orgKeys.suppliers(q),
    queryFn: () => api<Supplier[]>('/organization/suppliers', { query: { q } }),
    enabled: q.trim().length >= 2,
  })

export const useEmployees = (search: string) =>
  useQuery({ queryKey: orgKeys.employees(search), queryFn: () => api<Employee[]>('/organization/employees', { query: { search } }) })

// ---- mutations: each one refreshes the lists it affects ----------------------------------------

function useCreate<TBody>(path: string, invalidate: readonly unknown[]) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (body: TBody) => api(path, { method: 'POST', body }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: invalidate }),
  })
}

function useUpdate<TBody>(path: (id: string) => string, invalidate: readonly unknown[]) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ id, body }: { id: string; body: TBody }) => api(path(id), { method: 'PUT', body }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: invalidate }),
  })
}

export const useCreateBranch = () => useCreate<Omit<Branch, 'id'>>('/organization/branches', orgKeys.branches)
export const useUpdateBranch = () => useUpdate<Omit<Branch, 'id'>>((id) => `/organization/branches/${id}`, orgKeys.branches)
export const useCreateDepartment = () => useCreate<Omit<Department, 'id'>>('/organization/departments', orgKeys.departments)
export const useCreateItemType = () => useCreate<ItemType>('/organization/item-types', orgKeys.itemTypes)
export const useCreateCompany = () => useCreate<Omit<Company, 'id' | 'isActive'>>('/organization/companies', orgKeys.companies)
export const useUpdateCompany = () =>
  useUpdate<Omit<Company, 'id' | 'isActive' | 'code'>>((id) => `/organization/companies/${id}`, orgKeys.companies)

export interface EmployeeInput {
  employeeNumber: string
  firstName: string
  middleName: string
  lastName: string
  email: string
  departmentId: string
  branchId: string
}

export const useRegisterEmployee = () => useCreate<EmployeeInput>('/organization/employees', ['organization', 'employees'])

export function useUpdateEmployee() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ id, body }: { id: string; body: Omit<EmployeeInput, 'employeeNumber'> }) =>
      api(`/organization/employees/${id}`, { method: 'PUT', body }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['organization', 'employees'] }),
  })
}

export function useDeactivateEmployee() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (id: string) => api(`/organization/employees/${id}`, { method: 'DELETE' }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['organization', 'employees'] }),
  })
}
