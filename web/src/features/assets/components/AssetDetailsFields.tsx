import { useEffect } from 'react'
import { useFormContext, useWatch } from 'react-hook-form'
import { Section, TextAreaField, TextField } from '@/shared/ui'
import { useDebounced } from '@/shared/hooks/useDebounced'
import { useSuppliers } from '@/features/organization/api'
import type { AssetDetailsForm } from '../schemas'
import { SuggestInput } from './SuggestInput'

/** The fields shared by "register asset" and "edit asset". Must be rendered inside a react-hook-form <FormProvider>. */
export function AssetDetailsFields() {
  const { register, setValue, getValues, control, formState: { errors } } = useFormContext<AssetDetailsForm>()
  const [vendor, supplierName] = useWatch({ control, name: ['vendor', 'supplier.name'] })

  // A known supplier (typed or picked from the list) fills in whatever details are still empty.
  const suppliers = useSuppliers(useDebounced(supplierName ?? ''))
  useEffect(() => {
    const match = suppliers.data?.find((s) => s.name.toLowerCase() === (supplierName ?? '').trim().toLowerCase())
    if (!match) return
    if (!getValues('supplier.address')) setValue('supplier.address', match.address)
    if (!getValues('supplier.contactNumber')) setValue('supplier.contactNumber', match.contactNumber ?? '')
    if (!getValues('supplier.email')) setValue('supplier.email', match.email)
  }, [suppliers.data, supplierName, getValues, setValue])

  return (
    <>
      <Section title="Asset">
        <div className="grid gap-4 sm:grid-cols-2">
          <TextField label="Description" containerClassName="sm:col-span-2" placeholder="e.g. Dell Optiplex desktop set" error={errors.description?.message} {...register('description')} />
          <SuggestInput label="Vendor" field="Vendor" term={vendor ?? ''} error={errors.vendor?.message} {...register('vendor')} />
          <TextField label="Purchase order no." error={errors.purchaseOrderNumber?.message} {...register('purchaseOrderNumber')} />
          <TextField label="Control no." error={errors.controlNumber?.message} {...register('controlNumber')} />
          <TextAreaField label="Remarks" containerClassName="sm:col-span-2" error={errors.remarks?.message} {...register('remarks')} />
        </div>
      </Section>

      <Section title="Purchase">
        <div className="grid gap-4 sm:grid-cols-2">
          <TextField label="Price (PHP)" type="number" inputMode="decimal" step="0.01" min={0} error={errors.price?.message} {...register('price', { valueAsNumber: true })} />
          <TextField label="Invoice date" type="date" error={errors.invoiceDate?.message} {...register('invoiceDate')} />
          <TextField label="Invoice no." error={errors.invoiceNumber?.message} {...register('invoiceNumber')} />
          <div className="grid grid-cols-2 gap-4">
            <TextField label="Life span (months)" type="number" inputMode="numeric" min={0} error={errors.lifeSpanMonths?.message} {...register('lifeSpanMonths', { valueAsNumber: true })} />
            <TextField label="Warranty (months)" type="number" inputMode="numeric" min={0} error={errors.warrantyMonths?.message} {...register('warrantyMonths', { valueAsNumber: true })} />
          </div>
        </div>
      </Section>

      <Section title="Supplier">
        <div className="grid gap-4 sm:grid-cols-2">
          <TextField
            label="Name"
            list="supplier-names"
            autoComplete="off"
            error={errors.supplier?.name?.message}
            {...register('supplier.name')}
          />
          <datalist id="supplier-names">
            {suppliers.data?.map((s) => (
              <option key={s.id} value={s.name} />
            ))}
          </datalist>
          <TextField label="Email" type="email" error={errors.supplier?.email?.message} {...register('supplier.email')} />
          <TextField label="Address" containerClassName="sm:col-span-2" error={errors.supplier?.address?.message} {...register('supplier.address')} />
          <TextField label="Contact number" type="tel" error={errors.supplier?.contactNumber?.message} {...register('supplier.contactNumber')} />
        </div>
      </Section>
    </>
  )
}
