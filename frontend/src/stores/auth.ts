import { defineStore } from 'pinia'
import { computed, ref } from 'vue'
import { tokenStorage } from '@/lib/api/token-storage'
import { authApi } from '@/features/auth/auth.api'
import { ApiError } from '@/lib/api/http'
import { logger } from '@/lib/logger'
import type { AuthUser, LoginPayload, UserRole, RegisterPayload } from '@/features/auth/auth.types'

const USER_KEY = 'auth.user'
const VALID_ROLES: UserRole[] = ['Patient', 'Receptionist', 'Doctor', 'Admin']

function normalizeRoleArray(value: unknown): string[] {
  if (value == null) return []

  if (Array.isArray(value)) {
    return value.flatMap((item) => normalizeRoleArray(item))
  }

  if (typeof value === 'string') {
    return value
      .split(',')
      .map((part) => part.trim())
      .filter(Boolean)
  }

  if (typeof value === 'object') {
    const obj = value as Record<string, unknown>
    return [obj.name, obj.role, obj.value].flatMap((entry) => normalizeRoleArray(entry))
  }

  return [String(value)]
}

function normalizeRoles(roles?: (UserRole | undefined)[] | UserRole | null | unknown[]): UserRole[] {
  if (!roles) return []

  const list = Array.isArray(roles) ? roles : [roles]
  const flattened = list.flatMap((role) => normalizeRoleArray(role))
  return [...new Set(flattened.filter((role): role is UserRole => VALID_ROLES.includes(role as UserRole)))]
}

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

function extractRolesFromJwtPayload(payload: Record<string, any>): UserRole[] {
  const candidateValues: unknown[] = []
  const keys = [
    'role',
    'roles',
    'http://schemas.microsoft.com/ws/2008/06/identity/claims/role',
    'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role',
  ]

  for (const key of keys) {
    if (payload[key] !== undefined) candidateValues.push(payload[key])
  }

  const flattened = candidateValues.flatMap((value) => normalizeRoleArray(value))
  return normalizeRoles(flattened.length > 0 ? flattened : ['Patient'])
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
  const roles = extractRolesFromJwtPayload(payload)
  const activeRole = (roles[0] ?? 'Patient') as UserRole

  return {
    id: payload.sub ?? 'user',
    name: payload.name ?? payload.unique_name ?? payload.username ?? 'User',
    email: payload.email ?? payload['email'] ?? '',
    role: activeRole,
    roles,
    activeRole,
  }
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
    const raw = sessionStorage.getItem(USER_KEY) ?? localStorage.getItem(USER_KEY)
    return raw ? JSON.parse(raw) : null
  } catch {
    return null
  }
}

function safeSetStoredUser(u: AuthUser | null, persistent = false) {
  try {
    const storage = persistent ? localStorage : sessionStorage
  const opposite = persistent ? sessionStorage : localStorage

  if (u) {
    storage.setItem(USER_KEY, JSON.stringify(u))
    opposite.removeItem(USER_KEY)
  } else {
    localStorage.removeItem(USER_KEY)
    sessionStorage.removeItem(USER_KEY)
  }
} catch (e) {
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

  function setToken(accessToken: string, refreshToken?: string, persistent = false) {
      tokenStorage.set(accessToken, refreshToken, persistent)
  }

  function setUser(userData: AuthUser | null, persistent = tokenStorage.isPersistent()) {
    user.value = normalizeUser(userData)
    safeSetStoredUser(user.value, persistent)
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

    const persistent = tokenStorage.isPersistent()
    safeSetStoredUser(user.value, persistent)
  }

  async function fetchMe() {
    try {
      const me = await authApi.getMe()
      setUser(me)
      return me
    } catch (e: any) {
      const statusCode = e instanceof Error && (e as any).status ? (e as any).status : e?.response?.status
      const accessToken = tokenStorage.getAccess()
      const fallbackUser = buildUserFromToken(accessToken)

      if (statusCode === 401 || statusCode === 403) {
        if (fallbackUser) {
          setUser(fallbackUser, tokenStorage.isPersistent())
          return fallbackUser
        }
        tokenStorage.clear()
        safeSetStoredUser(null, false)
      } else if (fallbackUser) {
        setUser(fallbackUser, tokenStorage.isPersistent())
        return fallbackUser
      }

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
      const fallbackUser = buildUserFromToken(res.accessToken)
      const normalizedUser = normalizeUser({
        ...res.user,
        role: res.role ?? res.user?.role ?? fallbackUser.role,
        roles: res.roles ?? res.user?.roles ?? fallbackUser.roles,
        activeRole: res.role ?? res.user?.activeRole ?? fallbackUser.activeRole,
      }) ?? fallbackUser

      const activeRole = payload.role ?? normalizedUser.activeRole ?? normalizedUser.roles?.[0] ?? 'Patient'
      normalizedUser.activeRole = activeRole
      normalizedUser.role = activeRole
      normalizedUser.roles = normalizeRoles(normalizedUser.roles ?? [activeRole])

      console.log('[auth login] resolved role:', normalizedUser.role)
      console.log('[auth login] resolved roles:', normalizedUser.roles)

      const persistent = payload.rememberMe === true
      setToken(res.accessToken, res.refreshToken, persistent)
      setUser(normalizedUser, persistent)
      status.value = 'idle'
      return normalizedUser
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
        const fallbackUser = buildUserFromToken(token)
        const normalizedUser = normalizeUser({
          ...res.user,
          role: res.role ?? res.user?.role ?? fallbackUser.role,
          roles: res.roles ?? res.user?.roles ?? fallbackUser.roles,
          activeRole: res.role ?? res.user?.activeRole ?? fallbackUser.activeRole,
        }) ?? fallbackUser
        const activeRole = normalizedUser.activeRole ?? normalizedUser.role ?? normalizedUser.roles?.[0] ?? 'Patient'
        normalizedUser.activeRole = activeRole
        normalizedUser.role = activeRole
        normalizedUser.roles = normalizeRoles(normalizedUser.roles ?? [activeRole])

        const persistent = (payload as any)?.rememberMe === true
        setToken(token, res.refreshToken, persistent)
        setUser(normalizedUser, persistent)
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
