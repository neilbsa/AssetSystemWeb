import { useFormContext, useWatch, type FieldPath, type FieldValues } from 'react-hook-form'
import { TextAreaField, TextField, SelectField } from '@/shared/ui'
import { useItemTypes } from '@/features/organization/api'
import { SuggestInput } from './SuggestInput'

interface ItemFieldsProps {
  /** Form path of this item, e.g. `items.0`. */
  prefix: string
  /** Item type is chosen when an item is created and fixed afterwards. */
  showType?: boolean
}

/** Inputs for one item. Works for any form whose path `prefix` points at a NewItemForm-shaped object. */
export function ItemFields({ prefix, showType = true }: ItemFieldsProps) {
  const { register, control, formState: { errors } } = useFormContext<FieldValues>()
  const types = useItemTypes()
  const name = (field: string) => `${prefix}.${field}` as FieldPath<FieldValues>
  const itemErrors = prefix.split('.').reduce<Record<string, unknown> | undefined>(
    (node, key) => node?.[key] as Record<string, unknown> | undefined,
    errors as Record<string, unknown>,
  ) as Record<string, { message?: string }> | undefined
  const [description, partNumber] = useWatch({ control, name: [name('description'), name('partNumber')] }) as [string, string]

  return (
    <div className="grid gap-4 sm:grid-cols-2">
      {showType && (
        <SelectField label="Type" error={itemErrors?.typeCode?.message} {...register(name('typeCode'))}>
          <option value="">Select type…</option>
          {types.data?.map((t) => (
            <option key={t.code} value={t.code}>
              {t.name}
            </option>
          ))}
        </SelectField>
      )}
      <SuggestInput label="Description" field="ItemDescription" term={description ?? ''} error={itemErrors?.description?.message} {...register(name('description'))} />
      <SuggestInput label="Part number" field="PartNumber" term={partNumber ?? ''} error={itemErrors?.partNumber?.message} {...register(name('partNumber'))} />
      <TextField label="Serial number" placeholder="N/A" autoCapitalize="characters" error={itemErrors?.serialNumber?.message} {...register(name('serialNumber'))} />
      <TextField label="Key / licence" placeholder="N/A" autoCapitalize="characters" error={itemErrors?.keyDetail?.message} {...register(name('keyDetail'))} />
      <TextField label="Control no." error={itemErrors?.controlNumber?.message} {...register(name('controlNumber'))} />
      <TextAreaField label="Remarks" containerClassName="sm:col-span-2" rows={2} error={itemErrors?.remarks?.message} {...register(name('remarks'))} />
    </div>
  )
}
