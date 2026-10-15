import { FormProvider, useFieldArray, useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { useNavigate, useSearchParams } from 'react-router'
import { Plus, Trash2 } from 'lucide-react'
import { Button, ErrorNotice, PageHeader, Section, SelectField, Segmented, useToast } from '@/shared/ui'
import { todayIso } from '@/shared/lib/format'
import { useBranches } from '@/features/organization/api'
import { useRegisterAsset } from '../api'
import { AssetDetailsFields } from '../components/AssetDetailsFields'
import { ItemFields } from '../components/ItemFields'
import { emptyItem, registerAssetSchema, type RegisterAssetForm } from '../schemas'
import type { AssetCategory } from '../types'

export function AssetCreatePage() {
  const navigate = useNavigate()
  const toast = useToast()
  const [params] = useSearchParams()
  const branches = useBranches()
  const register = useRegisterAsset()

  const form = useForm<RegisterAssetForm>({
    resolver: zodResolver(registerAssetSchema),
    defaultValues: {
      category: params.get('category') === 'Salesman' ? 'Salesman' : 'Company',
      branchId: '',
      description: '',
      vendor: '',
      purchaseOrderNumber: '',
      controlNumber: '',
      remarks: '',
      price: 0,
      lifeSpanMonths: 36,
      warrantyMonths: 12,
      invoiceNumber: '',
      invoiceDate: todayIso(),
      supplier: { name: '', address: '', contactNumber: '', email: '' },
      items: [{ ...emptyItem }],
    },
  })
  const items = useFieldArray({ control: form.control, name: 'items' })
  const { errors } = form.formState
  const category = form.watch('category')

  const onSubmit = form.handleSubmit(async (values) => {
    const created = await register.mutateAsync(values).catch(() => null)
    if (!created) return
    toast.success(`Registered ${created.assetNumber}`)
    navigate(`/assets/${created.id}`)
  })

  return (
    <>
      <PageHeader title="Register asset" subtitle="A purchase made of one or more trackable items" back="/assets" />
      <FormProvider {...form}>
        <form onSubmit={onSubmit} className="space-y-4" noValidate>
          {register.isError && <ErrorNotice error={register.error} />}

          <Section title="Where does it belong?">
            <div className="grid gap-4 sm:grid-cols-2">
              <div className="space-y-1.5">
                <span className="block text-sm font-medium">Inventory</span>
                <Segmented<AssetCategory>
                  label="Inventory"
                  value={category}
                  onChange={(v) => form.setValue('category', v)}
                  options={[
                    { value: 'Company', label: 'Company' },
                    { value: 'Salesman', label: 'Salesman' },
                  ]}
                />
              </div>
              <SelectField label="Branch" error={errors.branchId?.message} {...form.register('branchId')}>
                <option value="">Select branch…</option>
                {branches.data?.map((b) => (
                  <option key={b.id} value={b.id}>
                    {b.name}
                  </option>
                ))}
              </SelectField>
            </div>
          </Section>

          <AssetDetailsFields />

          <Section
            title={`Items (${items.fields.length})`}
            action={
              <Button size="sm" variant="secondary" onClick={() => items.append({ ...emptyItem })}>
                <Plus className="size-4" aria-hidden /> Add item
              </Button>
            }
          >
            <div className="space-y-6">
              {items.fields.map((field, index) => (
                <div key={field.id} className="space-y-3 rounded-lg border border-line p-3 sm:p-4">
                  <div className="flex items-center justify-between">
                    <h3 className="text-sm font-medium">Item {index + 1}</h3>
                    {items.fields.length > 1 && (
                      <Button size="sm" variant="ghost" onClick={() => items.remove(index)} aria-label={`Remove item ${index + 1}`}>
                        <Trash2 className="size-4" aria-hidden /> Remove
                      </Button>
                    )}
                  </div>
                  <ItemFields prefix={`items.${index}`} />
                </div>
              ))}
              {errors.items?.root?.message && <p className="text-sm text-red-600">{errors.items.root.message}</p>}
            </div>
          </Section>

          <Button type="submit" loading={register.isPending} className="w-full sm:w-auto">
            Register asset
          </Button>
        </form>
      </FormProvider>
    </>
  )
}
