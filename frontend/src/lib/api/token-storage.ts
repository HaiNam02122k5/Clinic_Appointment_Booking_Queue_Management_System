/**
 * Single source of truth for auth tokens. Swap this implementation
 * (e.g. to httpOnly-cookie auth) without touching call sites.
 */
import { logger } from '@/lib/logger'
const ACCESS_TOKEN_KEY = 'auth.accessToken'
const REFRESH_TOKEN_KEY = 'auth.refreshToken'

export const tokenStorage = {
  getAccess: () => {
    try {
      // Prefer localStorage first, then fall back to sessionStorage for session-only tokens
      return localStorage.getItem(ACCESS_TOKEN_KEY) ?? sessionStorage.getItem(ACCESS_TOKEN_KEY)
    } catch {
      return null
    }
  },
  getRefresh: () => {
    try {
      return localStorage.getItem(REFRESH_TOKEN_KEY) ?? sessionStorage.getItem(REFRESH_TOKEN_KEY)
    } catch {
      return null
    }
  },
  /**
   * Set tokens. By default persistent=true -> use localStorage. If persistent=false -> use sessionStorage.
   */
  set: (access: string, refresh?: string, persistent = true) => {
    try {
      const storage = persistent ? localStorage : sessionStorage
      storage.setItem(ACCESS_TOKEN_KEY, access)
      if (refresh) storage.setItem(REFRESH_TOKEN_KEY, refresh)
    } catch (e) {
      // best-effort; if storage fails, log but don't throw
      logger.error('tokenStorage.set failed', e)
    }
  },
  clear: () => {
    try {
      localStorage.removeItem(ACCESS_TOKEN_KEY)
      localStorage.removeItem(REFRESH_TOKEN_KEY)
      sessionStorage.removeItem(ACCESS_TOKEN_KEY)
      sessionStorage.removeItem(REFRESH_TOKEN_KEY)
    } catch (e) {
      logger.error('tokenStorage.clear failed', e)
    }
  },
}
