import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'
import router from './router'
import { i18n } from './lib/i18n'

const app = createApp(App)

app.use(createPinia())
app.use(router)
app.use(i18n)

// React to forced logout from the HTTP layer (refresh failed).
window.addEventListener('auth:logout', () => {
  router.push('/login')
})

app.mount('#app')
