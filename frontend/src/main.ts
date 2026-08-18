import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'
import router from './router'
import { i18n } from './lib/i18n'
import { logger } from '@/lib/logger'

const app = createApp(App) as any

// Install Pinia first so stores can be used for hydration
(app as any).use(createPinia())

// Before registering router, hydrate auth if we have a token to avoid router redirect races.
;(async () => {
  try {
    // Check both localStorage and sessionStorage for stored access token to avoid importing tokenStorage here and creating circular types
    const hasAccessToken = (() => {
      try {
        return !!(localStorage.getItem('auth.accessToken') || sessionStorage.getItem('auth.accessToken'))
      } catch {
        return false
      }
    })()

    if (hasAccessToken) {
      const mod = await import('@/stores/auth')
      const auth = mod.useAuthStore()
      // prefer hydrate helper that sets the store.hydrating flag
      if (typeof auth.hydrate === 'function') {
        await auth.hydrate()
      } else if (!auth.isAuthenticated) {
        await auth.fetchMe()
      }
    }
  } catch (e) {
    // fetching profile failed; ensure app reacts to cleared session
    logger.warn('Failed to hydrate auth profile on startup', e)
  } finally {
    // Now register router and i18n and mount the app
    ;(app as any).use(router)
    ;(app as any).use(i18n)
    app.mount('#app')
  }
})()

// React to forced logout from the HTTP layer (refresh failed).
window.addEventListener('auth:logout', () => {
  // router will be registered by the time this runs
  router.push('/login')
})
