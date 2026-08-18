import { env } from '@/config/env'
import { http } from '@/lib/api/http'
import { mockGetMe, mockLogin, mockRegister } from '@/mock/clinic-data'
import type { LoginPayload, RegisterPayload, LoginResponse, AuthUser } from './auth.types'

function buildLoginBody(payload: LoginPayload) {
  const username = (payload.username ?? payload.email ?? '').trim()
  return {
    username,
    password: payload.password,
  }
}

function buildRegisterBody(payload: RegisterPayload) {
  return {
    username: payload.username ?? payload.email,
    fullName: payload.fullName,
    phoneNumber: payload.phoneNumber,
    email: payload.email,
    password: payload.password,
    gender: payload.gender,
    dateOfBirth: payload.dateOfBirth,
    address: payload.address ?? 'Chưa cập nhật',
  }
}

export const authApi = {
  login(payload: LoginPayload): Promise<LoginResponse> {
    if (env.enableMock) {
      try {
        const r = mockLogin(payload.email ?? payload.username ?? '', payload.password, payload.role)
        return Promise.resolve(r)
      } catch (e) {
        return Promise.reject(e)
      }
    }

    return http.post<any>('/auth/login', buildLoginBody(payload)).then((r) => {
      const d = r.data as any
      if (d && d.token && !d.accessToken) d.accessToken = d.token
      const accessToken = d?.accessToken ?? ''
      const fallbackUser = {
        id: 'user',
        name: 'User',
        email: payload.username ?? payload.email ?? '',
        role: 'Patient',
        roles: ['Patient'],
        activeRole: 'Patient',
      } as AuthUser
      return {
        accessToken,
        refreshToken: d?.refreshToken,
        user: d?.user ?? fallbackUser,
      } as LoginResponse
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

    return http.post<any>('/auth/register', buildRegisterBody(payload)).then((r) => {
      const d = r.data as any
      if (d && d.token && !d.accessToken) d.accessToken = d.token
      const accessToken = d?.accessToken ?? ''
      const fallbackUser = {
        id: 'user',
        name: payload.fullName,
        email: payload.email,
        role: 'Patient',
        roles: ['Patient'],
        activeRole: 'Patient',
      } as AuthUser
      return {
        accessToken,
        refreshToken: d?.refreshToken,
        user: d?.user ?? fallbackUser,
      } as LoginResponse
    })
  },

  async getMe(): Promise<AuthUser> {
    if (env.enableMock) {
      try {
        const r = mockGetMe()
        return r
      } catch (e) {
        throw e
      }
    }

    try {
      const response = await http.get<AuthUser>('/auth/me')
      return response.data
    } catch (error: any) {
      if (error?.response?.status === 404) {
        const token = localStorage.getItem('auth.accessToken') ?? sessionStorage.getItem('auth.accessToken')
        if (!token) throw error

        const payload = JSON.parse(atob(token.split('.')[1] ?? ''))
        const roles = Array.isArray(payload.role)
          ? payload.role
          : payload.role
            ? [payload.role]
            : ['Patient']

        return {
          id: payload.sub ?? 'user',
          name: payload.name ?? 'User',
          email: payload.email ?? '',
          role: roles[0] as any,
          roles: roles as any,
          activeRole: roles[0] as any,
        }
      }
      throw error
    }
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
