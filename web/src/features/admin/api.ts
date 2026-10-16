import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { api } from '@/shared/api/http'

export interface SystemUser {
  id: string
  userName: string
  email: string
  firstName: string
  middleName: string | null
  lastName: string
  fullName: string
  isLocked: boolean
  roles: string[]
  companyIds: string[]
}

export interface RegisterUserInput {
  userName: string
  email: string
  firstName: string
  middleName: string
  lastName: string
  password: string
  roles: string[]
  companyIds: string[]
}

export interface UpdateAccessInput {
  roles: string[]
  companyIds: string[]
  isLocked: boolean
  newPassword: string | null
}

export const ROLES = ['Administrator', 'Common'] as const

const usersKey = ['identity', 'users'] as const

export const useUsers = () => useQuery({ queryKey: usersKey, queryFn: () => api<SystemUser[]>('/identity/users') })

export function useRegisterUser() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (body: RegisterUserInput) => api('/identity/users', { method: 'POST', body }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: usersKey }),
  })
}

export function useUpdateUserAccess() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ id, body }: { id: string; body: UpdateAccessInput }) => api(`/identity/users/${id}/access`, { method: 'PUT', body }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: usersKey }),
  })
}
