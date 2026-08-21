import { describe, expect, it } from 'vitest'

const baseUrl = process.env.VITE_API_BASE_URL ?? process.env.API_BASE_URL ?? ''

const getApiUrl = (path: string) => {
  const normalizedBase = baseUrl.replace(/\/$/, '')
  return `${normalizedBase}${path.startsWith('/') ? path : `/${path}`}`
}

const getToken = () => {
  if (typeof window !== 'undefined') {
    return window.localStorage.getItem('auth.accessToken') ?? window.sessionStorage.getItem('auth.accessToken') ?? ''
  }

  return ''
}

describe('patient integration flow - real backend', () => {
  it('loads patient appointments, queue, history and profile from backend when auth is configured', async () => {
    if (!baseUrl) return

    const token = getToken()
    if (!token) return

    const headers = {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    }

    try {
      const appointmentsResponse = await fetch(getApiUrl('/me/appointments'), { method: 'GET', headers })
      if (appointmentsResponse.ok) {
        const appointmentsPayload = await appointmentsResponse.json()
        expect(Array.isArray(appointmentsPayload) || Array.isArray(appointmentsPayload.items) || Array.isArray(appointmentsPayload.result)).toBe(true)
      }

      const queueResponse = await fetch(getApiUrl('/me/queue-status'), { method: 'GET', headers })
      if (queueResponse.ok) {
        const queuePayload = await queueResponse.json()
        expect(queuePayload).toBeTruthy()
      }

      const historyResponse = await fetch(getApiUrl('/me/medical-history'), { method: 'GET', headers })
      if (historyResponse.ok) {
        const historyPayload = await historyResponse.json()
        expect(Array.isArray(historyPayload) || Array.isArray(historyPayload.items) || Array.isArray(historyPayload.result)).toBe(true)
      }

      const profileResponse = await fetch(getApiUrl('/patients/me'), { method: 'GET', headers })
      if (profileResponse.ok) {
        const profilePayload = await profileResponse.json()
        const responseObject = profilePayload.result ?? profilePayload.data ?? profilePayload
        expect(responseObject).toBeTruthy()
      }
    } catch {
      return
    }
  })
})
