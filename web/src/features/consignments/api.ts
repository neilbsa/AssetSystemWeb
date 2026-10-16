import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { api, upload } from '@/shared/api/http'
import type { Paged } from '@/features/assets/types'
import type { ConsigneeSummary, ConsignmentDetails, ConsignmentSummary, IssueConsignmentInput } from './types'

export const consignmentKeys = {
  all: ['consignments'] as const,
  list: (params: object) => ['consignments', 'list', params] as const,
  detail: (id: string) => ['consignments', 'detail', id] as const,
  consignees: (search: string) => ['consignments', 'consignees', search] as const,
}

export const useConsignments = (params: { consigneeId?: string; search: string; page: number }) =>
  useQuery({
    queryKey: consignmentKeys.list(params),
    queryFn: () => api<Paged<ConsignmentSummary>>('/consignments', { query: { ...params, pageSize: 20 } }),
    placeholderData: (previous) => previous,
  })

export const useConsignment = (id: string) =>
  useQuery({ queryKey: consignmentKeys.detail(id), queryFn: () => api<ConsignmentDetails>(`/consignments/${id}`) })

export const useConsignees = (search: string) =>
  useQuery({ queryKey: consignmentKeys.consignees(search), queryFn: () => api<ConsigneeSummary[]>('/consignments/consignees', { query: { search } }) })

/**
 * Issuing a consignment makes Assets mark the items "in use" a moment later (asynchronously),
 * so the asset and item caches are refreshed as well.
 */
export function useIssueConsignment() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (body: IssueConsignmentInput) => api<{ id: string; number: string }>('/consignments', { method: 'POST', body }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: consignmentKeys.all })
      queryClient.invalidateQueries({ queryKey: ['assets'] })
    },
  })
}

export function useUploadReceivingCopy(id: string) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (file: File) => {
      const form = new FormData()
      form.set('file', file)
      return upload(`/consignments/${id}/receiving-copy`, form, 'PUT')
    },
    onSuccess: () => queryClient.invalidateQueries({ queryKey: consignmentKeys.detail(id) }),
  })
}
