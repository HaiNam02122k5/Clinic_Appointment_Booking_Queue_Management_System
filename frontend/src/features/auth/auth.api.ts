import { env } from '@/config/env'
import { http } from '@/lib/api/http'
import { mockGetMe, mockLogin, mockRegister } from '@/mock/clinic-data'
import type { LoginPayload, RegisterPayload, LoginResponse, AuthUser } from './auth.types'

export const authApi = {
  login(payload: LoginPayload): Promise<LoginResponse> {
    if (env.enableMock) {
      return Promise.resolve(mockLogin(payload.email, payload.password))
    }

    return http.post<LoginResponse>('/auth/login', payload).then((r) => r.data)
  },

  register(payload: RegisterPayload): Promise<LoginResponse> {
    if (env.enableMock) {
      return Promise.resolve(mockRegister(payload as any))
    }

    return http.post<LoginResponse>('/auth/register', payload).then((r) => r.data)
  },

  getMe(): Promise<AuthUser> {
    if (env.enableMock) {
      return Promise.resolve(mockGetMe())
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
