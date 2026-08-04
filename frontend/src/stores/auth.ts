import { defineStore } from 'pinia'
import { computed, ref } from 'vue'
import { tokenStorage } from '@/lib/api/token-storage'
import { authApi } from '@/features/auth/auth.api'
import type { AuthUser, LoginPayload } from '@/features/auth/auth.types'

const USER_KEY = 'auth.user'

export const useAuthStore = defineStore('auth', () => {
  // Rehydrate identity from storage (tokens live in tokenStorage).
  const stored = localStorage.getItem(USER_KEY)
  const user = ref<AuthUser | null>(stored ? JSON.parse(stored) : null)
  const status = ref<'idle' | 'loading' | 'error'>('idle')
  const error = ref<string | null>(null)

  const isAuthenticated = computed(() => user.value !== null)

  async function login(payload: LoginPayload) {
    status.value = 'loading'
    error.value = null
    try {
      const res = await authApi.login(payload)
      tokenStorage.set(res.accessToken, res.refreshToken)
      user.value = res.user
      localStorage.setItem(USER_KEY, JSON.stringify(res.user))
      status.value = 'idle'
    } catch (e) {
      status.value = 'error'
      error.value = e instanceof Error ? e.message : 'error'
      throw e
    }
  }

  function logout() {
    tokenStorage.clear()
    localStorage.removeItem(USER_KEY)
    user.value = null
    status.value = 'idle'
    error.value = null
  }

  return { user, status, error, isAuthenticated, login, logout }
})
