import { env } from '@/config/env'
import { http } from '@/lib/api/http'
import { mockGetMe, mockLogin, mockRegister } from '@/mock/clinic-data'
import type { LoginPayload, RegisterPayload, LoginResponse, AuthUser } from './auth.types'

export const authApi = {
  login(payload: LoginPayload): Promise<LoginResponse> {
    if (env.enableMock) {
      try {
        const r = mockLogin(payload.email, payload.password, payload.role)
        return Promise.resolve(r)
      } catch (e) {
        return Promise.reject(e)
      }
    }

    return http.post<LoginResponse>('/auth/login', payload).then((r) => {
      const d = r.data as any
      if (d && d.token && !d.accessToken) d.accessToken = d.token
      return d as LoginResponse
    })
  },



  register(payload: RegisterPayload): Promise<LoginResponse> {
    if (env.enableMock) {
      try {
        const r = mockRegister(payload as any)
        return Promise.resolve(r)
      } catch (e) {
        return Promise.reject(e)
      }
    }

    return http.post<LoginResponse>('/auth/register', payload).then((r) => {
      const d = r.data as any
      if (d && d.token && !d.accessToken) d.accessToken = d.token
      return d as LoginResponse
    })
  },



  getMe(): Promise<AuthUser> {
    if (env.enableMock) {
      try {
        const r = mockGetMe()
        return Promise.resolve(r)
      } catch (e) {
        return Promise.reject(e)
      }
    }

    return http.get<AuthUser>('/auth/me').then((r) => r.data)
  },


  mockMultiRoleLogin(user: Partial<AuthUser> = {}): LoginResponse {
    return {
      accessToken: 'mock-access-token',
      refreshToken: 'mock-refresh-token',
      user: {
        id: 1,
        name: 'Demo User',
        email: 'demo@clinic.com',
        role: 'Patient',
        roles: ['Patient', 'Doctor'],
        activeRole: 'Patient',
        ...user,
      },
    }
  },
}
