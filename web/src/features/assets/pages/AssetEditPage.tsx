import { FormProvider, useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { useNavigate, useParams } from 'react-router'
import { Button, ErrorNotice, PageHeader, QueryState, useToast } from '@/shared/ui'
import { useAsset, useUpdateAsset } from '../api'
import { AssetDetailsFields } from '../components/AssetDetailsFields'
import { assetDetailsSchema, type AssetDetailsForm } from '../schemas'
import type { AssetDetails } from '../types'

export function AssetEditPage() {
  const { id = '' } = useParams()
  const asset = useAsset(id)

  return (
    <>
      <PageHeader title="Edit asset" subtitle={asset.data?.assetNumber} back={`/assets/${id}`} />
      <QueryState query={asset}>{asset.data && <EditForm asset={asset.data} />}</QueryState>
    </>
  )
}

function EditForm({ asset }: { asset: AssetDetails }) {
  const navigate = useNavigate()
  const toast = useToast()
  const update = useUpdateAsset(asset.id)

  const form = useForm<AssetDetailsForm>({
    resolver: zodResolver(assetDetailsSchema),
    defaultValues: {
      description: asset.description,
      vendor: asset.vendor,
      purchaseOrderNumber: asset.purchaseOrderNumber ?? '',
      controlNumber: asset.controlNumber ?? '',
      remarks: asset.remarks ?? '',
      price: asset.price,
      lifeSpanMonths: asset.lifeSpanMonths,
      warrantyMonths: asset.warrantyMonths,
      invoiceNumber: asset.invoiceNumber,
      invoiceDate: asset.invoiceDate,
      // Only the supplier's name is kept on the asset; picking it from the list refills the rest.
      supplier: { name: asset.supplierName, address: '', contactNumber: '', email: '' },
    },
  })

  const onSubmit = form.handleSubmit(async (values) => {
    const ok = await update.mutateAsync(values).then(() => true, () => false)
    if (!ok) return
    toast.success('Asset updated')
    navigate(`/assets/${asset.id}`)
  })

  return (
    <FormProvider {...form}>
      <form onSubmit={onSubmit} className="space-y-4" noValidate>
        {update.isError && <ErrorNotice error={update.error} />}
        <AssetDetailsFields />
        <div className="flex gap-3">
          <Button type="submit" loading={update.isPending} className="flex-1 sm:flex-none">
            Save changes
          </Button>
          <Button variant="secondary" onClick={() => navigate(-1)} className="flex-1 sm:flex-none">
            Cancel
          </Button>
        </div>
      </form>
    </FormProvider>
  )
}
