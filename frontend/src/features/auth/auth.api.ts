import { env } from '@/config/env'
import { http } from '@/lib/api/http'
import { tokenStorage } from '@/lib/api/token-storage'
import { mockGetMe, mockLogin, mockRegister } from '@/mock/clinic-data'
import type { LoginPayload, RegisterPayload, LoginResponse, AuthUser } from './auth.types'

function decodeJwtPayload(token: string): Record<string, any> {
  try {
    const base64 = token.split('.')[1]
    if (!base64) return {}
    const normalized = base64.replace(/-/g, '+').replace(/_/g, '/')
    const padded = normalized + '='.repeat((4 - (normalized.length % 4)) % 4)
    return JSON.parse(atob(padded))
  } catch {
    return {}
  }
}

function normalizeRoleValues(value: unknown): string[] {
  if (value == null) return []

  if (Array.isArray(value)) {
    return value.flatMap((item) => normalizeRoleValues(item))
  }

  if (typeof value === 'string') {
    return value
      .split(',')
      .map((part) => part.trim())
      .filter(Boolean)
  }

  if (typeof value === 'object') {
    const obj = value as Record<string, unknown>
    const candidates = [obj.name, obj.role, obj.value]
    return candidates.flatMap((candidate) => normalizeRoleValues(candidate))
  }

  return [String(value)]
}

function extractJwtRoleValues(payload: Record<string, any>): string[] {
  const keys = [
    'role',
    'roles',
    'http://schemas.microsoft.com/ws/2008/06/identity/claims/role',
    'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role',
  ]

  const flattened = keys.flatMap((key) => normalizeRoleValues(payload?.[key]))
  return [...new Set(flattened.filter(Boolean))]
}

function buildUserFromToken(token?: string | null): AuthUser {
  const fallback: AuthUser = {
    id: 'user',
    name: 'User',
    email: '',
    role: 'Patient',
    roles: ['Patient'],
    activeRole: 'Patient',
  }

  if (!token) return fallback

  const payload = decodeJwtPayload(token)
  const roles = extractJwtRoleValues(payload)
  const primaryRole = (roles[0] ?? 'Patient') as any

  return {
    id: payload.sub ?? 'user',
    name: payload.name ?? payload.unique_name ?? payload.username ?? 'User',
    email: payload.email ?? '',
    role: primaryRole,
    roles: roles as any,
    activeRole: primaryRole,
  }
}

function buildLoginBody(payload: LoginPayload) {
  const username = (payload.username ?? payload.email ?? '').trim()
  return {
    username,
    password: payload.password,
  }
}

function buildRegisterBody(payload: RegisterPayload) {
  const username = (payload.username ?? payload.email ?? '').trim()
  const fullName = (payload.fullName ?? '').trim()
  const phoneNumber = (payload.phoneNumber ?? '').trim()
  const email = (payload.email ?? '').trim()
  const address = (payload.address ?? '').trim() || 'Chưa cập nhật'
  const dateOfBirth = (payload.dateOfBirth ?? '').trim()

  const genderMap: Record<string, number> = {
    Male: 0,
    Female: 1,
    Other: 2,
  }

  return {
    username,
    password: payload.password,
    fullName,
    phoneNumber,
    email: email || null,
    gender: genderMap[payload.gender] ?? 0,
    dateOfBirth,
    address,
  }
}

function unwrapApiResult<T>(payload: any): T {
  if (!payload || typeof payload !== 'object') return payload as T
  if (payload.result !== undefined) return payload.result as T
  return payload as T
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
      const d = unwrapApiResult<any>(r.data)
      const accessToken = d?.accessToken ?? d?.token ?? ''
      const jwtUser = accessToken ? buildUserFromToken(accessToken) : {
        id: 'user',
        name: 'User',
        email: payload.username ?? payload.email ?? '',
        role: 'Patient',
        roles: ['Patient'],
        activeRole: 'Patient',
      } as AuthUser

      const backendUser = (d?.user ?? {}) as Partial<AuthUser>
      const explicitRoleCandidates = [
        backendUser.role,
        backendUser.roles,
        d?.role,
        d?.roles,
        backendUser.activeRole,
      ]

      const roleFromBody = explicitRoleCandidates.flatMap((item) => normalizeRoleValues(item)).find(Boolean) ?? jwtUser.role
      const rolesFromBody = explicitRoleCandidates
        .flatMap((item) => normalizeRoleValues(item))
        .filter(Boolean)
      const resolvedRoles = rolesFromBody.length > 0 ? [...new Set(rolesFromBody)] : (jwtUser.roles ?? ['Patient'])
      const resolvedRole = (roleFromBody ?? resolvedRoles[0] ?? jwtUser.role ?? 'Patient') as AuthUser['role']

      const mergedUser: AuthUser = {
        ...jwtUser,
        ...backendUser,
        id: backendUser.id ?? jwtUser.id,
        name: backendUser.name ?? jwtUser.name,
        email: backendUser.email ?? jwtUser.email ?? payload.username ?? payload.email ?? '',
        role: resolvedRole,
        roles: resolvedRoles as AuthUser['roles'],
        activeRole: (backendUser.activeRole ?? resolvedRole ?? jwtUser.activeRole ?? 'Patient') as AuthUser['activeRole'],
      }

      return {
        accessToken,
        refreshToken: d?.refreshToken,
        role: (mergedUser.role ?? 'Patient') as LoginResponse['role'],
        roles: (mergedUser.roles ?? [mergedUser.role ?? 'Patient']) as LoginResponse['roles'],
        user: mergedUser,
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
      const d = unwrapApiResult<any>(r.data)
      if (d && d.token && !d.accessToken) d.accessToken = d.token
      const accessToken = d?.accessToken ?? d?.token ?? ''
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
      const token = tokenStorage.getAccess()
      if (token) {
        return buildUserFromToken(token)
      }

      if (error?.response?.status === 404) {
        throw new Error('Backend does not expose /auth/me. Falling back to JWT payload.')
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
