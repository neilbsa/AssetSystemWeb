import { forwardRef, useId, type InputHTMLAttributes, type ReactNode, type SelectHTMLAttributes, type TextareaHTMLAttributes } from 'react'
import { cn } from '@/shared/lib/cn'

const control =
  'block w-full min-h-11 rounded-lg border bg-surface px-3 py-2 text-ink placeholder:text-ink-muted/70 ' +
  'focus:outline-2 focus:outline-offset-0 focus:outline-brand-500 disabled:bg-surface-muted disabled:opacity-70'

interface FieldShellProps {
  label: string
  error?: string
  hint?: string
  className?: string
  children: (props: { id: string; 'aria-invalid': boolean; 'aria-describedby'?: string; className: string }) => ReactNode
}

/** Label + control + error/hint, wired together for screen readers. */
function FieldShell({ label, error, hint, className, children }: FieldShellProps) {
  const id = useId()
  const messageId = `${id}-msg`
  return (
    <div className={cn('space-y-1.5', className)}>
      <label htmlFor={id} className="block text-sm font-medium">
        {label}
      </label>
      {children({
        id,
        'aria-invalid': !!error,
        'aria-describedby': error || hint ? messageId : undefined,
        className: cn(control, error ? 'border-red-500' : 'border-line'),
      })}
      {(error || hint) && (
        <p id={messageId} className={cn('text-xs', error ? 'text-red-600 dark:text-red-400' : 'text-ink-muted')}>
          {error ?? hint}
        </p>
      )}
    </div>
  )
}

type Common = { label: string; error?: string; hint?: string; containerClassName?: string }

export const TextField = forwardRef<HTMLInputElement, Common & InputHTMLAttributes<HTMLInputElement>>(function TextField(
  { label, error, hint, containerClassName, ...rest },
  ref,
) {
  return (
    <FieldShell label={label} error={error} hint={hint} className={containerClassName}>
      {(p) => <input ref={ref} {...p} {...rest} className={cn(p.className, rest.className)} />}
    </FieldShell>
  )
})

export const SelectField = forwardRef<HTMLSelectElement, Common & SelectHTMLAttributes<HTMLSelectElement>>(function SelectField(
  { label, error, hint, containerClassName, children, ...rest },
  ref,
) {
  return (
    <FieldShell label={label} error={error} hint={hint} className={containerClassName}>
      {(p) => (
        <select ref={ref} {...p} {...rest}>
          {children}
        </select>
      )}
    </FieldShell>
  )
})

export const TextAreaField = forwardRef<HTMLTextAreaElement, Common & TextareaHTMLAttributes<HTMLTextAreaElement>>(function TextAreaField(
  { label, error, hint, containerClassName, ...rest },
  ref,
) {
  return (
    <FieldShell label={label} error={error} hint={hint} className={containerClassName}>
      {(p) => <textarea ref={ref} rows={3} {...p} {...rest} />}
    </FieldShell>
  )
})
