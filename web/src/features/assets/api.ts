import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { api, upload } from '@/shared/api/http'
import type {
  AssetCategory, AssetDetails, AssetDetailsInput, AssetSummary, Dashboard, ItemHistory, ItemRow, ItemStatus,
  NewItemInput, Paged, RegisteredAsset, RegisterAssetInput,
} from './types'

export const assetKeys = {
  all: ['assets'] as const,
  list: (params: object) => ['assets', 'list', params] as const,
  detail: (id: string) => ['assets', 'detail', id] as const,
  items: (params: object) => ['assets', 'items', params] as const,
  item: (id: string) => ['assets', 'item', id] as const,
  history: (id: string) => ['assets', 'item', id, 'history'] as const,
  dashboard: ['assets', 'dashboard'] as const,
}

// ---- queries ------------------------------------------------------------------------------

export const useAssets = (params: { category?: AssetCategory; search: string; page: number }) =>
  useQuery({
    queryKey: assetKeys.list(params),
    queryFn: () => api<Paged<AssetSummary>>('/assets', { query: { ...params, pageSize: 20 } }),
    placeholderData: (previous) => previous, // keep showing the old page while the next one loads
  })

export const useAsset = (id: string) =>
  useQuery({ queryKey: assetKeys.detail(id), queryFn: () => api<AssetDetails>(`/assets/${id}`) })

export const useItems = (params: { status?: ItemStatus; search: string; page: number }) =>
  useQuery({
    queryKey: assetKeys.items(params),
    queryFn: () => api<Paged<ItemRow>>('/assets/items', { query: { ...params, pageSize: 20 } }),
    placeholderData: (previous) => previous,
  })

export const useItemHistory = (id: string) =>
  useQuery({ queryKey: assetKeys.history(id), queryFn: () => api<ItemHistory>(`/assets/items/${id}/history`) })

export const useDashboard = () =>
  useQuery({ queryKey: assetKeys.dashboard, queryFn: () => api<Dashboard>('/assets/dashboard') })

export type SuggestionField = 'Vendor' | 'AssetDescription' | 'ItemDescription' | 'PartNumber'

export const useSuggestions = (field: SuggestionField, term: string) =>
  useQuery({
    queryKey: ['assets', 'suggestions', field, term],
    queryFn: () => api<string[]>('/assets/suggestions', { query: { field, q: term } }),
    enabled: term.trim().length >= 2,
    staleTime: 60_000,
  })

export const lookupAvailableItems = (code: string) => api<ItemRow[]>('/assets/items/lookup', { query: { code } })

// ---- mutations ----------------------------------------------------------------------------

/** Every write below can change lists, details and the dashboard, so refresh the whole assets area. */
function useAssetMutation<TVars, TResult = void>(fn: (vars: TVars) => Promise<TResult>) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: fn,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: assetKeys.all }),
  })
}

export const useRegisterAsset = () =>
  useAssetMutation((body: RegisterAssetInput) => api<RegisteredAsset>('/assets', { method: 'POST', body }))

export const useUpdateAsset = (id: string) =>
  useAssetMutation((body: AssetDetailsInput) => api(`/assets/${id}`, { method: 'PUT', body }))

export const useAddItems = (assetId: string) =>
  useAssetMutation((items: NewItemInput[]) => api<string[]>(`/assets/${assetId}/items`, { method: 'POST', body: { items } }))

export const useUpdateItem = (assetId: string) =>
  useAssetMutation(({ itemId, ...body }: { itemId: string } & Omit<NewItemInput, 'typeCode'>) =>
    api(`/assets/${assetId}/items/${itemId}`, { method: 'PUT', body }))

export const useChangeItemStatus = (assetId: string) =>
  useAssetMutation(({ itemId, status }: { itemId: string; status: ItemStatus }) =>
    api(`/assets/${assetId}/items/${itemId}/status`, { method: 'PUT', body: { status } }))

export const useUploadDocument = (assetId: string) =>
  useAssetMutation(({ file, documentType }: { file: File; documentType: string }) => {
    const form = new FormData()
    form.set('documentType', documentType)
    form.set('file', file)
    return upload(`/assets/${assetId}/documents`, form)
  })

export const useRemoveDocument = (assetId: string) =>
  useAssetMutation((documentId: string) => api(`/assets/${assetId}/documents/${documentId}`, { method: 'DELETE' }))
