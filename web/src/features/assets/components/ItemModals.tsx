import { FormProvider, useFieldArray, useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Plus, Trash2 } from 'lucide-react'
import { Button, ErrorNotice, Modal, useToast } from '@/shared/ui'
import { useAddItems, useUpdateItem } from '../api'
import { emptyItem, newItemSchema, type NewItemForm } from '../schemas'
import type { AssetItem } from '../types'
import { ItemFields } from './ItemFields'

// ---- Edit one item ------------------------------------------------------------------------------

const editSchema = z.object({ item: newItemSchema.omit({ typeCode: true }) })
type EditForm = z.infer<typeof editSchema>

export function EditItemModal({ assetId, item, onClose }: { assetId: string; item: AssetItem | null; onClose: () => void }) {
  return (
    <Modal open={item !== null} title={item ? `Edit ${item.itemCode}` : ''} onClose={onClose}>
      {item && <EditItemForm key={item.id} assetId={assetId} item={item} onClose={onClose} />}
    </Modal>
  )
}

function EditItemForm({ assetId, item, onClose }: { assetId: string; item: AssetItem; onClose: () => void }) {
  const toast = useToast()
  const update = useUpdateItem(assetId)
  const form = useForm<EditForm>({
    resolver: zodResolver(editSchema),
    defaultValues: {
      item: {
        description: item.description,
        partNumber: item.partNumber,
        serialNumber: item.serialNumber === 'N/A' ? '' : item.serialNumber,
        keyDetail: item.keyDetail === 'N/A' ? '' : item.keyDetail,
        controlNumber: item.controlNumber ?? '',
        remarks: item.remarks ?? '',
      },
    },
  })

  const onSubmit = form.handleSubmit(async ({ item: values }) => {
    const ok = await update.mutateAsync({ itemId: item.id, ...values }).then(() => true, () => false)
    if (!ok) return
    toast.success('Item updated')
    onClose()
  })

  return (
    <FormProvider {...form}>
      <form onSubmit={onSubmit} className="space-y-4" noValidate>
        {update.isError && <ErrorNotice error={update.error} />}
        <ItemFields prefix="item" showType={false} />
        <Button type="submit" loading={update.isPending} className="w-full">
          Save item
        </Button>
      </form>
    </FormProvider>
  )
}

// ---- Add items to an existing asset ----------------------------------------------------------------

const addSchema = z.object({ items: z.array(newItemSchema).min(1) })
type AddForm = z.infer<typeof addSchema>

export function AddItemsModal({ assetId, open, onClose }: { assetId: string; open: boolean; onClose: () => void }) {
  return (
    <Modal open={open} title="Add items" onClose={onClose}>
      {open && <AddItemsForm assetId={assetId} onClose={onClose} />}
    </Modal>
  )
}

function AddItemsForm({ assetId, onClose }: { assetId: string; onClose: () => void }) {
  const toast = useToast()
  const add = useAddItems(assetId)
  const form = useForm<AddForm>({ resolver: zodResolver(addSchema), defaultValues: { items: [{ ...emptyItem } as NewItemForm] } })
  const items = useFieldArray({ control: form.control, name: 'items' })

  const onSubmit = form.handleSubmit(async (values) => {
    const codes = await add.mutateAsync(values.items).catch(() => null)
    if (!codes) return
    toast.success(`Added ${codes.join(', ')}`)
    onClose()
  })

  return (
    <FormProvider {...form}>
      <form onSubmit={onSubmit} className="space-y-6" noValidate>
        {add.isError && <ErrorNotice error={add.error} />}
        {items.fields.map((field, index) => (
          <div key={field.id} className="space-y-3 rounded-lg border border-line p-3">
            <div className="flex items-center justify-between">
              <h3 className="text-sm font-medium">New item {index + 1}</h3>
              {items.fields.length > 1 && (
                <Button size="sm" variant="ghost" onClick={() => items.remove(index)} aria-label={`Remove item ${index + 1}`}>
                  <Trash2 className="size-4" aria-hidden />
                </Button>
              )}
            </div>
            <ItemFields prefix={`items.${index}`} />
          </div>
        ))}
        <div className="flex flex-col gap-2 sm:flex-row">
          <Button variant="secondary" onClick={() => items.append({ ...emptyItem })}>
            <Plus className="size-4" aria-hidden /> Another item
          </Button>
          <Button type="submit" loading={add.isPending} className="sm:ml-auto">
            Add to asset
          </Button>
        </div>
      </form>
    </FormProvider>
  )
}
