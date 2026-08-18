import { ApiError } from './http'

export function getFieldErrors(e: unknown): Record<string, string[]> | undefined {
  if (!e) return undefined

  // ApiError instance
  if (e instanceof ApiError) return e.fieldErrors

  // axios-like error with response.data.errors
  const anyErr = e as any
  if (anyErr?.response?.data?.errors && typeof anyErr.response.data.errors === 'object') {
    return anyErr.response.data.errors as Record<string, string[]>
  }

  // fallback: response.data may have a different shape
  if (anyErr?.response?.data && anyErr.response.data.errors) return anyErr.response.data.errors

  return undefined
}
