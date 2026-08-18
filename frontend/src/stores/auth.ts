import { defineStore } from 'pinia'
import { computed, ref } from 'vue'
import { tokenStorage } from '@/lib/api/token-storage'
import { authApi } from '@/features/auth/auth.api'
import { ApiError } from '@/lib/api/http'
import type { AuthUser, LoginPayload, UserRole, RegisterPayload } from '@/features/auth/auth.types'

const USER_KEY = 'auth.user'
const VALID_ROLES: UserRole[] = ['Patient', 'Receptionist', 'Doctor', 'Admin']

function normalizeRoles(roles?: UserRole[] | UserRole | null): UserRole[] {
  if (!roles) return []

  const list = Array.isArray(roles) ? roles : [roles]
  return [...new Set(list.filter((role): role is UserRole => VALID_ROLES.includes(role as UserRole)))]
}

function normalizeUser(raw: Partial<AuthUser> | null | undefined): AuthUser | null {
  if (!raw) return null

  const roles = normalizeRoles(raw.roles ?? (raw.role ? [raw.role] : []))
  const candidateRole = (raw.activeRole ?? raw.role ?? roles[0] ?? 'Patient') as UserRole
  const roleToUse = VALID_ROLES.includes(candidateRole) ? candidateRole : VALID_ROLES[0]

  return {
    id: Number(raw.id ?? 0),
    name: raw.name ?? 'User',
    email: raw.email ?? '',
    role: roleToUse,
    roles: roles.length > 0 ? roles : [roleToUse],
    activeRole: roleToUse,
  }
}

export const useAuthStore = defineStore('auth', () => {
  const hasStoredSession = !!tokenStorage.getAccess() && !!localStorage.getItem(USER_KEY)
  const stored = hasStoredSession ? localStorage.getItem(USER_KEY) : null
  const user = ref<AuthUser | null>(normalizeUser(stored ? JSON.parse(stored) : null))

  if (!hasStoredSession && localStorage.getItem(USER_KEY)) {
    localStorage.removeItem(USER_KEY)
  }

  const status = ref<'idle' | 'loading' | 'error'>('idle')
  const error = ref<string | null>(null)

  const isAuthenticated = computed(() => user.value !== null)
  const currentUserRole = computed<UserRole | null>(() => user.value?.activeRole ?? user.value?.role ?? user.value?.roles?.[0] ?? null)

  function hasRole(allowedRoles: UserRole[]): boolean {
    if (!user.value) return false

    const grantedRoles = user.value.roles ?? []
    return allowedRoles.some((role) => grantedRoles.includes(role))
  }

  function setToken(accessToken: string, refreshToken?: string) {
    tokenStorage.set(accessToken, refreshToken)
  }

  function setUser(userData: AuthUser | null) {
    user.value = normalizeUser(userData)

    if (user.value) {
      localStorage.setItem(USER_KEY, JSON.stringify(user.value))
    } else {
      localStorage.removeItem(USER_KEY)
    }
  }

  function setActiveRole(role: UserRole) {
    if (!user.value) return

    const roles = user.value.roles ?? [user.value.role ?? role]
    if (!roles.includes(role)) {
      roles.push(role)
    }

    user.value = {
      ...user.value,
      role,
      roles,
      activeRole: role,
    }

    localStorage.setItem(USER_KEY, JSON.stringify(user.value))
  }

  async function login(payload: LoginPayload) {
    status.value = 'loading'
    error.value = null

    try {
      const res = await authApi.login(payload)
      const normalizedUser = normalizeUser(res.user)

      if (normalizedUser) {
        const activeRole = payload.role ?? normalizedUser.activeRole ?? normalizedUser.roles?.[0]
        if (activeRole) {
          normalizedUser.activeRole = activeRole
          normalizedUser.role = activeRole
          normalizedUser.roles = normalizeRoles(normalizedUser.roles ?? [activeRole])
        }
      }

      setToken(res.accessToken, res.refreshToken)
      setUser(normalizedUser)
      status.value = 'idle'
      return normalizedUser ?? res.user
    } catch (e: any) {
      status.value = 'error'
      if (e instanceof ApiError) {
        error.value = e.message
      } else {
        error.value = e?.response?.data?.message || (e instanceof Error ? e.message : 'Đăng nhập thất bại')
      }
      throw e
    }
  }

  async function register(payload: RegisterPayload) {
    status.value = 'loading'
    error.value = null

    try {
      const res = await authApi.register(payload)
      const token = res.accessToken || (res as any).token

      if (res && token) {
        const normalizedUser = normalizeUser(res.user)
        if (normalizedUser) {
          const activeRole = normalizedUser.activeRole ?? normalizedUser.role ?? normalizedUser.roles?.[0]
          if (activeRole) {
            normalizedUser.activeRole = activeRole
            normalizedUser.role = activeRole
            normalizedUser.roles = normalizeRoles(normalizedUser.roles ?? [activeRole])
          }
        }

        setToken(token, res.refreshToken)
        setUser(normalizedUser)
      }

      status.value = 'idle'
      return res
    } catch (e: any) {
      status.value = 'error'
      if (e instanceof ApiError) {
        error.value = e.message
      } else {
        error.value = e?.response?.data?.message || (e instanceof Error ? e.message : 'Đăng ký thất bại')
      }
      throw e
    }
  }

  function logout() {
    tokenStorage.clear()
    setUser(null)
    status.value = 'idle'
    error.value = null
  }

  return {
    user,
    status,
    error,
    isAuthenticated,
    currentUserRole,

    setToken,
    setUser,
    setActiveRole,
    logout,
    hasRole,
    login,
    register,
  }
})
