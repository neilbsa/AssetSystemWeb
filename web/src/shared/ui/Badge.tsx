import type { ReactNode } from 'react'
import { cn } from '@/shared/lib/cn'

type Tone = 'neutral' | 'green' | 'blue' | 'red' | 'amber'

const tones: Record<Tone, string> = {
  neutral: 'bg-ink-muted/15 text-ink-muted',
  green: 'bg-green-500/15 text-green-700 dark:text-green-300',
  blue: 'bg-blue-500/15 text-blue-700 dark:text-blue-300',
  red: 'bg-red-500/15 text-red-700 dark:text-red-300',
  amber: 'bg-amber-500/15 text-amber-700 dark:text-amber-300',
}

export function Badge({ tone = 'neutral', children }: { tone?: Tone; children: ReactNode }) {
  return <span className={cn('inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium whitespace-nowrap', tones[tone])}>{children}</span>
}
