import { defineStore } from 'pinia'
import { computed, ref } from 'vue'
import { tokenStorage } from '@/lib/api/token-storage'
import { authApi } from '@/features/auth/auth.api'
import type { AuthUser, LoginPayload, UserRole, RegisterPayload } from '@/features/auth/auth.types'

const USER_KEY = 'auth.user'

export const useAuthStore = defineStore('auth', () => {
  // Rehydrate identity from storage (tokens live in tokenStorage).
  const stored = localStorage.getItem(USER_KEY)
  const user = ref<AuthUser | null>(stored ? JSON.parse(stored) : null)
  const status = ref<'idle' | 'loading' | 'error'>('idle')
  const error = ref<string | null>(null)

  const isAuthenticated = computed(() => user.value !== null)
  const currentUserRole = computed<UserRole | null>(() => user.value?.role ?? null)

  // Kiểm tra xem người dùng có quyền truy cập vào các vai trò được phép hay không
  function hasRole(allowedRoles: UserRole[]): boolean {
    if (!user.value) return false
    return allowedRoles.includes(user.value.role)
  }

  // Lưu trữ Access Token và Refresh Token vào tokenStorage
  function setToken(accessToken: string, refreshToken?: string) {
    tokenStorage.set(accessToken, refreshToken)
  }

  // Lưu trữ thông tin người dùng vào State và localStorage
  function setUser(userData: AuthUser | null) {
    user.value = userData
    if (userData) {
      localStorage.setItem(USER_KEY, JSON.stringify(userData))
    } else {
      localStorage.removeItem(USER_KEY)
    }
  }

  // Tái sử dụng setToken và setUser trong hàm login
  async function login(payload: LoginPayload) {
    status.value = 'loading'
    error.value = null
    try {
      const res = await authApi.login(payload)
      setToken(res.accessToken, res.refreshToken)
      setUser(res.user)
      status.value = 'idle'
    } catch (e) {
      status.value = 'error'
      error.value = e instanceof Error ? e.message : 'Đăng nhập không thành công'
      throw e
    }
  }

  async function register(payload: RegisterPayload) {
    status.value = 'loading'
    error.value = null

    try {
      // Giả lập API delay
      await new Promise((resolve) => setTimeout(resolve, 800))

      // Mock đăng ký thành công
      const newUser: AuthUser = {
        id: Date.now(),
        name: payload.fullName,
        email: payload.email,
        role: 'Patient',
      }

      const mockAccessToken = 'mock-access-token-' + Date.now()
      const mockRefreshToken = 'mock-refresh-token-' + Date.now()

      setToken(mockAccessToken, mockRefreshToken)
      setUser(newUser)

      status.value = 'idle'
    } catch (e: unknown) {
      status.value = 'error'
      error.value = e instanceof Error ? e.message : 'Đăng ký thất bại'
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
    logout,
    hasRole,
    login,
    register
  }
})