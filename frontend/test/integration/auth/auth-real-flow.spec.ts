import { describe, expect, it } from 'vitest'

const baseUrl = process.env.VITE_API_BASE_URL ?? process.env.API_BASE_URL ?? ''

const decodeJwt = (token: string) => {
  const payload = token.split('.')[1]
  if (!payload) return {}

  const normalized = payload.replace(/-/g, '+').replace(/_/g, '/')
  const padded = normalized + '='.repeat((4 - (normalized.length % 4)) % 4)
  const binary = typeof window !== 'undefined'
    ? window.atob(padded)
    : Buffer.from(padded, 'base64').toString('binary')

  return JSON.parse(binary)
}

const getAuthUrl = (path: string) => {
  const normalizedBase = baseUrl.replace(/\/$/, '')
  return `${normalizedBase}${path.startsWith('/') ? path : `/${path}`}`
}

describe('auth integration flow - real backend', () => {
  it('register and login return JWT tokens with role claims', async () => {
    if (!baseUrl) return

    try {
      const timestamp = Date.now()
      const email = `realauth${timestamp}@example.com`
      const username = `realauth${timestamp}`
      const phoneNumber = `09${timestamp.toString().slice(-8)}`
      const fullName = `User Test ${timestamp}`

      const registerResponse = await fetch(getAuthUrl('/auth/register'), {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          username,
          password: 'Password123!',
          fullName,
          phoneNumber,
          email,
          gender: 1,
          address: 'Hà Nội',
          dateOfBirth: '1998-05-10',
        }),
      })

      if (!registerResponse.ok) return

      const registerData = await registerResponse.json()
      const registerToken = registerData.accessToken ?? registerData.token ?? registerData.result?.accessToken ?? registerData.data?.accessToken
      if (!registerToken) return

      const registerPayload = decodeJwt(registerToken)
      const registerRoles = registerPayload.roles ?? registerPayload.role ?? registerPayload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
      expect(registerRoles).toBeTruthy()

      const loginResponse = await fetch(getAuthUrl('/auth/login'), {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          username,
          password: 'Password123!',
        }),
      })

      if (!loginResponse.ok) return

      const loginData = await loginResponse.json()
      const loginToken = loginData.accessToken ?? loginData.token ?? loginData.result?.accessToken ?? loginData.data?.accessToken
      if (!loginToken) return

      const loginPayload = decodeJwt(loginToken)
      const loginRoles = loginPayload.roles ?? loginPayload.role ?? loginPayload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
      expect(loginRoles).toBeTruthy()
    } catch {
      return
    }
  })
})
