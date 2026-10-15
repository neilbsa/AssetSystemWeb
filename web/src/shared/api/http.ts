/**
 * Thin fetch wrapper: adds the bearer token, turns RFC 9457 problem details into `ApiError`s
 * and signs the user out when the API says the token is no longer valid.
 */

const TOKEN_KEY = 'assetsystem.token'

export const tokenStore = {
  get: () => localStorage.getItem(TOKEN_KEY),
  set: (token: string) => localStorage.setItem(TOKEN_KEY, token),
  clear: () => localStorage.removeItem(TOKEN_KEY),
}

export class ApiError extends Error {
  constructor(
    public readonly status: number,
    message: string,
    public readonly code?: string,
    /** Field → messages, present for 400 validation failures. */
    public readonly fieldErrors?: Record<string, string[]>,
  ) {
    super(message)
    this.name = 'ApiError'
  }
}

let onUnauthorized: (() => void) | undefined
export const setUnauthorizedHandler = (handler: () => void) => {
  onUnauthorized = handler
}

type Query = Record<string, string | number | boolean | null | undefined>

interface RequestOptions {
  method?: 'GET' | 'POST' | 'PUT' | 'DELETE'
  body?: unknown
  query?: Query
  signal?: AbortSignal
}

const toQueryString = (query?: Query) => {
  if (!query) return ''
  const params = new URLSearchParams()
  for (const [key, value] of Object.entries(query)) {
    if (value !== undefined && value !== null && value !== '') params.set(key, String(value))
  }
  const text = params.toString()
  return text ? `?${text}` : ''
}

async function toError(response: Response): Promise<ApiError> {
  let problem: { title?: string; code?: string; errors?: Record<string, string[]> } = {}
  try {
    problem = await response.json()
  } catch {
    /* body was not JSON */
  }
  const firstFieldError = problem.errors ? Object.values(problem.errors).flat()[0] : undefined
  return new ApiError(
    response.status,
    firstFieldError ?? problem.title ?? `Request failed (${response.status})`,
    problem.code,
    problem.errors,
  )
}

async function send(path: string, init: RequestInit & { query?: Query }): Promise<Response> {
  const headers = new Headers(init.headers)
  const token = tokenStore.get()
  if (token) headers.set('Authorization', `Bearer ${token}`)

  const response = await fetch(`/api${path}${toQueryString(init.query)}`, { ...init, headers })

  if (response.status === 401 && token) onUnauthorized?.()
  if (!response.ok) throw await toError(response)
  return response
}

export async function api<T = void>(path: string, { method = 'GET', body, query, signal }: RequestOptions = {}): Promise<T> {
  const response = await send(path, {
    method,
    query,
    signal,
    headers: body === undefined ? undefined : { 'Content-Type': 'application/json' },
    body: body === undefined ? undefined : JSON.stringify(body),
  })
  if (response.status === 204) return undefined as T
  return (await response.json()) as T
}

export async function upload<T = void>(path: string, form: FormData, method: 'POST' | 'PUT' = 'POST'): Promise<T> {
  const response = await send(path, { method, body: form })
  if (response.status === 204) return undefined as T
  return (await response.json()) as T
}

/** Fetches a protected file and opens it in a new tab (an <a href> cannot send the bearer token). */
export async function openFile(path: string): Promise<void> {
  const response = await send(path, { method: 'GET' })
  const url = URL.createObjectURL(await response.blob())
  window.open(url, '_blank', 'noopener')
  setTimeout(() => URL.revokeObjectURL(url), 60_000)
}
