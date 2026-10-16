import { api, ApiError, setUnauthorizedHandler, tokenStore } from './http'

const json = (status: number, body: unknown) =>
  new Response(JSON.stringify(body), { status, headers: { 'Content-Type': 'application/json' } })

describe('api', () => {
  beforeEach(() => {
    localStorage.clear()
    vi.restoreAllMocks()
  })

  it('sends the bearer token and a JSON body', async () => {
    tokenStore.set('abc')
    const fetchMock = vi.spyOn(globalThis, 'fetch').mockResolvedValue(json(200, { ok: true }))

    await api('/things', { method: 'POST', body: { a: 1 }, query: { page: 2, empty: '' } })

    const [url, init] = fetchMock.mock.calls[0]!
    expect(url).toBe('/api/things?page=2')
    expect((init!.headers as Headers).get('Authorization')).toBe('Bearer abc')
    expect(init!.body).toBe('{"a":1}')
  })

  it('turns problem details into an ApiError with the code', async () => {
    vi.spyOn(globalThis, 'fetch').mockResolvedValue(json(409, { title: 'Serial number is already registered.', code: 'item.serial_in_use' }))

    const error = await api('/assets').catch((e) => e)

    expect(error).toBeInstanceOf(ApiError)
    expect(error).toMatchObject({ status: 409, code: 'item.serial_in_use', message: 'Serial number is already registered.' })
  })

  it('reports the first field error for validation failures', async () => {
    vi.spyOn(globalThis, 'fetch').mockResolvedValue(json(400, { title: 'Validation failed', errors: { Email: ['Email is not valid.'] } }))

    const error = await api('/x').catch((e) => e)

    expect(error.message).toBe('Email is not valid.')
  })

  it('signs the user out when a request with a token comes back 401', async () => {
    tokenStore.set('expired')
    const onUnauthorized = vi.fn()
    setUnauthorizedHandler(onUnauthorized)
    vi.spyOn(globalThis, 'fetch').mockResolvedValue(json(401, {}))

    await api('/assets').catch(() => undefined)

    expect(onUnauthorized).toHaveBeenCalledOnce()
  })

  it('returns undefined for 204 responses', async () => {
    vi.spyOn(globalThis, 'fetch').mockResolvedValue(new Response(null, { status: 204 }))
    await expect(api('/x', { method: 'DELETE' })).resolves.toBeUndefined()
  })
})
