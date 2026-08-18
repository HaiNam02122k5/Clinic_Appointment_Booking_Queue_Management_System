import axios, { AxiosError, type AxiosInstance, type InternalAxiosRequestConfig } from 'axios'
import { env } from '@/config/env'
import { tokenStorage } from './token-storage'
import { logger } from '@/lib/logger'

export interface ApiErrorBody {
  message: string
  code?: string
  errors?: Record<string, string[]>
}

/** Normalized error the UI/stores can rely on — never a raw AxiosError. */
export class ApiError extends Error {
  readonly status: number
  readonly code?: string
  readonly fieldErrors?: Record<string, string[]>

  constructor(status: number, body?: ApiErrorBody) {
    super(body?.message ?? `Request failed with status ${status}`)
    this.name = 'ApiError'
    this.status = status
    this.code = body?.code
    this.fieldErrors = body?.errors
  }
}

export const http: AxiosInstance = axios.create({
  baseURL: env.apiBaseUrl,
  timeout: 15_000,
  headers: { 'Content-Type': 'application/json' },
})

// Request: attach bearer token.
http.interceptors.request.use((config: InternalAxiosRequestConfig) => {
  const token = tokenStorage.getAccess()
  if (token) {
    config.headers = config.headers ?? {}
    // Axios may represent headers as a plain object; set Authorization safely
    ;(config.headers as Record<string, any>)['Authorization'] = 'Bearer ' + token
  }
  return config
})

// Response: transparent refresh on 401 (deduped), then normalize errors.
let refreshing: Promise<void> | null = null

async function refreshSession(): Promise<void> {
  const refresh = tokenStorage.getRefresh()
  if (!refresh) throw new Error('No refresh token')
  if (env.enableMock) {
    // In mock mode simulate refresh by rotating access token while keeping refresh the same
    try {
      const newAccess = `mock-access-refreshed-${Date.now()}`
      tokenStorage.set(newAccess, refresh)
      return
    } catch (e) {
      throw e
    }
  }
  const { data } = await axios.post<{ accessToken: string; refreshToken?: string }>(
    `${env.apiBaseUrl}/auth/refresh`,
    { refreshToken: refresh },
  )
  tokenStorage.set(data.accessToken, data.refreshToken)
}

http.interceptors.response.use(
  (response) => response,
  async (error: AxiosError<ApiErrorBody>) => {
    const original = error.config as InternalAxiosRequestConfig & { _retried?: boolean }

    if (
      error.response?.status === 401 &&
      original &&
      !original._retried &&
      tokenStorage.getRefresh()
    ) {
      original._retried = true
      try {
        refreshing ??= refreshSession().finally(() => (refreshing = null))
        await refreshing
        return http(original)
      } catch (err) {
        // Log refresh failure for observability
        logger.warn('refreshSession failed', err)
        tokenStorage.clear()
        window.dispatchEvent(new CustomEvent('auth:logout'))
      }
    }

    throw new ApiError(error.response?.status ?? 0, error.response?.data)
  },
)
