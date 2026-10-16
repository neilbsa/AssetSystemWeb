import { forwardRef, useId, type ComponentProps } from 'react'
import { TextField } from '@/shared/ui'
import { useSuggestions, type SuggestionField } from '../api'
import { useDebounced } from '@/shared/hooks/useDebounced'

interface SuggestInputProps extends ComponentProps<typeof TextField> {
  field: SuggestionField
  /** Current text; the parent already holds it (react-hook-form `watch`). */
  term: string
}

/** Text field that offers previously used values through a native <datalist> (works well on mobile keyboards). */
export const SuggestInput = forwardRef<HTMLInputElement, SuggestInputProps>(function SuggestInput({ field, term, ...rest }, ref) {
  const listId = useId()
  const { data } = useSuggestions(field, useDebounced(term))
  return (
    <>
      <TextField ref={ref} list={listId} autoComplete="off" {...rest} />
      <datalist id={listId}>
        {data?.map((value) => (
          <option key={value} value={value} />
        ))}
      </datalist>
    </>
  )
})
