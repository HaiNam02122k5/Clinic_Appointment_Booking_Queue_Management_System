/**
 * Single source of truth for auth tokens. Swap this implementation
 * (e.g. to httpOnly-cookie auth) without touching call sites.
 */
const ACCESS_TOKEN_KEY = 'auth.accessToken'
const REFRESH_TOKEN_KEY = 'auth.refreshToken'

export const tokenStorage = {
  getAccess: () => {
    try {
      return localStorage.getItem(ACCESS_TOKEN_KEY)
    } catch {
      return null
    }
  },
  getRefresh: () => {
    try {
      return localStorage.getItem(REFRESH_TOKEN_KEY)
    } catch {
      return null
    }
  },
  set: (access: string, refresh?: string) => {
    try {
      localStorage.setItem(ACCESS_TOKEN_KEY, access)
      if (refresh) localStorage.setItem(REFRESH_TOKEN_KEY, refresh)
    } catch (e) {
      // best-effort; if storage fails, log but don't throw
      // eslint-disable-next-line no-console
      console.error('tokenStorage.set failed', e)
    }
  },
  clear: () => {
    try {
      localStorage.removeItem(ACCESS_TOKEN_KEY)
      localStorage.removeItem(REFRESH_TOKEN_KEY)
    } catch (e) {
      // eslint-disable-next-line no-console
      console.error('tokenStorage.clear failed', e)
    }
  },
}
