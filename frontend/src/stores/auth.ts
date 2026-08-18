import { defineStore } from 'pinia'
import { computed, ref } from 'vue'
import { tokenStorage } from '@/lib/api/token-storage'
import { authApi } from '@/features/auth/auth.api'
import { ApiError } from '@/lib/api/http'
import { logger } from '@/lib/logger'
import type { AuthUser, LoginPayload, UserRole, RegisterPayload } from '@/features/auth/auth.types'

const USER_KEY = 'auth.user'
const VALID_ROLES: UserRole[] = ['Patient', 'Receptionist', 'Doctor', 'Admin']

function normalizeRoles(roles?: (UserRole | undefined)[] | UserRole | null): UserRole[] {
  if (!roles) return []

  const list = Array.isArray(roles) ? (roles as (UserRole | undefined)[]) : [roles]
  return [...new Set(list.filter((role): role is UserRole => role !== undefined && VALID_ROLES.includes(role as UserRole)))]
}

function normalizeUser(raw: Partial<AuthUser> | null | undefined): AuthUser | null {
  if (!raw) return null

  // Build a clear roles input so TypeScript knows we're only passing UserRole[] | UserRole | null
  let rolesInput: UserRole[] | UserRole | null = null
  if (raw.roles && raw.roles.length > 0) rolesInput = raw.roles
  else if (raw.role) rolesInput = raw.role
  else rolesInput = null

  const roles: UserRole[] = normalizeRoles(rolesInput)
  const rolesSafe: UserRole[] = roles.filter(Boolean) as UserRole[]
  const candidateRole = (raw.activeRole ?? raw.role ?? rolesSafe[0] ?? 'Patient') as UserRole
  const roleToUse = VALID_ROLES.includes(candidateRole) ? candidateRole : VALID_ROLES[0]

  // Preserve id shape: number or string (UUID). Fall back to 0 if missing.
  const rawId = (raw as any).id
  let id: number | string = 0
  if (typeof rawId === 'number') id = rawId
  else if (typeof rawId === 'string' && rawId.trim() !== '') id = rawId
  else if (rawId != null && String(rawId).trim() !== '') {
    // Try to coerce numeric-like strings to number, otherwise keep string
    const coerced = Number(String(rawId))
    id = Number.isFinite(coerced) ? coerced : String(rawId)
  }
  const email = raw.email ?? ''

  return {
    id,
    name: raw.name ?? 'User',
    email,
    role: roleToUse,
    roles: (rolesSafe.length > 0 ? rolesSafe : [roleToUse]) as UserRole[],
    activeRole: roleToUse,
  }
}

// Safe localStorage helpers for the user payload to avoid exceptions in strict/private modes
function safeGetStoredUser(): Partial<AuthUser> | null {
  try {
    const raw = localStorage.getItem(USER_KEY)
    return raw ? JSON.parse(raw) : null
  } catch {
    return null
  }
}

function safeSetStoredUser(u: AuthUser | null) {
  try {
    if (u) localStorage.setItem(USER_KEY, JSON.stringify(u))
    else localStorage.removeItem(USER_KEY)
  } catch (e) {
    // swallow - not fatal for app execution; surface to logger for diagnostics
    logger.warn('safeSetStoredUser failed', e)
  }
}

export const useAuthStore = defineStore('auth', () => {
  const hasStoredSession = !!tokenStorage.getAccess() && !!safeGetStoredUser()
  const stored = hasStoredSession ? safeGetStoredUser() : null
  const user = ref<AuthUser | null>(normalizeUser(stored ?? null))
  // hydration state: when true the store is actively trying to fetch the profile from server
  const hydrating = ref(false)
  const status = ref<'idle' | 'loading' | 'error'>('idle')
  const error = ref<string | null>(null)

  const isAuthenticated = computed(() => user.value !== null)
  const currentUserRole = computed<UserRole | null>(() => user.value?.activeRole ?? user.value?.role ?? user.value?.roles?.[0] ?? null)

  function hasRole(allowedRoles: UserRole[]): boolean {
    if (!user.value) return false

    const grantedRoles = user.value.roles ?? []
    return allowedRoles.some((role) => grantedRoles.includes(role))
  }

  function setToken(accessToken: string, refreshToken?: string, persistent = true) {
      tokenStorage.set(accessToken, refreshToken, persistent)
  }

  function setUser(userData: AuthUser | null) {
    user.value = normalizeUser(userData)
    safeSetStoredUser(user.value)
  }

  function setActiveRole(role: UserRole) {
    if (!user.value) return

      const existing = user.value.roles ?? [user.value.role ?? role]
      const merged = normalizeRoles([...existing, role])

      user.value = {
        ...user.value,
        role,
        roles: merged,
        activeRole: role,
      }

      safeSetStoredUser(user.value)
  }

  async function fetchMe() {
    try {
      const me = await authApi.getMe()
      setUser(me)
      return me
    } catch (e: any) {
      // Only clear tokens on explicit auth failures (401/403). For transient errors keep tokens.
      const statusCode = e instanceof Error && (e as any).status ? (e as any).status : e?.response?.status
      if (statusCode === 401 || statusCode === 403) {
        tokenStorage.clear()
        safeSetStoredUser(null)
      }
      // bubble up for callers if needed
      throw e
    }
  }

  /**
   * Hydrate the store from token if present. Sets hydrating flag while running.
   */
  async function hydrate() {
    if (hydrating.value) return
    hydrating.value = true
    try {
      if (tokenStorage.getAccess()) {
        await fetchMe()
      }
    } catch (e) {
      // swallow here: fetchMe already cleared tokens on 401/403
    } finally {
      hydrating.value = false
    }
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

      const persistent = payload.rememberMe !== false
      setToken(res.accessToken, res.refreshToken, persistent)
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
      // rethrow so UI can inspect fieldErrors if present
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

        const persistent = (payload as any)?.rememberMe !== false
        setToken(token, res.refreshToken, persistent)
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
    fetchMe,
    hydrating,
    hydrate,
  }
})
