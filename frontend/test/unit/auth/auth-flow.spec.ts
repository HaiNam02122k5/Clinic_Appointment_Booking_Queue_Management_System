import { beforeEach, describe, expect, it, vi } from 'vitest'
import { createPinia, setActivePinia } from 'pinia'

import { useAuthStore } from '@/stores/auth'
import { authApi } from '@/features/auth/auth.api'
import { tokenStorage } from '@/lib/api/token-storage'

const makeJwt = (payload: Record<string, unknown>) => {
  const encode = (value: unknown) => {
    const json = JSON.stringify(value)
    const base64 = typeof Buffer !== 'undefined'
      ? Buffer.from(json, 'utf8').toString('base64')
      : btoa(json)

    return base64.replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/g, '')
  }

  return `${encode({ alg: 'HS256', typ: 'JWT' })}.${encode(payload)}.signature`
}

describe('auth unit flow - register/login', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    localStorage.clear()
    sessionStorage.clear()
    tokenStorage.clear()
  })

  it('login success stores JWT token and user role', async () => {
    const accessToken = makeJwt({
      sub: '12',
      name: 'Nguyễn Văn A',
      email: 'a@example.com',
      role: 'Patient',
      roles: ['Patient'],
    })

    vi.spyOn(authApi, 'login').mockResolvedValue({
      accessToken,
      refreshToken: 'refresh-login-01',
      user: {
        id: 12,
        name: 'Nguyễn Văn A',
        email: 'a@example.com',
        role: 'Patient',
        roles: ['Patient'],
        activeRole: 'Patient',
      },
    } as any)

    const auth = useAuthStore()
    const result = await auth.login({
      email: 'a@example.com',
      password: 'Password123!',
    })

    expect(result.role).toBe('Patient')
    expect(result.roles).toEqual(['Patient'])
    expect(auth.isAuthenticated).toBe(true)
    expect(auth.currentUserRole).toBe('Patient')
    expect(tokenStorage.getAccess()).toBe(accessToken)
  })

  it('register success persists access token and active role from JWT claim', async () => {
    const accessToken = makeJwt({
      sub: '77',
      name: 'Ms. Lan',
      email: 'lan@example.com',
      role: 'Patient',
      roles: ['Patient'],
    })

    vi.spyOn(authApi, 'register').mockResolvedValue({
      accessToken,
      refreshToken: 'refresh-register-01',
      user: {
        id: 77,
        name: 'Ms. Lan',
        email: 'lan@example.com',
        role: 'Patient',
        roles: ['Patient'],
        activeRole: 'Patient',
      },
    } as any)

    const auth = useAuthStore()
    const result = await auth.register({
      fullName: 'Ms. Lan',
      phoneNumber: '0912345678',
      email: 'lan@example.com',
      password: 'Password123!',
      gender: 'Female',
      dateOfBirth: '1998-05-10',
    })

    expect(result.accessToken).toBe(accessToken)
    expect(result.user.role).toBe('Patient')
    expect(result.user.activeRole).toBe('Patient')
    expect(auth.currentUserRole).toBe('Patient')
    expect(tokenStorage.getAccess()).toBe(accessToken)
  })

  it('login failure keeps auth clean and exposes backend message', async () => {
    vi.spyOn(authApi, 'login').mockRejectedValue({
      response: { data: { message: 'Email hoặc mật khẩu không đúng' } },
    })

    const auth = useAuthStore()

    await expect(
      auth.login({
        email: 'wrong@example.com',
        password: 'badpass',
      }),
    ).rejects.toBeDefined()

    expect(auth.isAuthenticated).toBe(false)
    expect(auth.error).toBe('Email hoặc mật khẩu không đúng')
    expect(tokenStorage.getAccess()).toBeNull()
  })
})
