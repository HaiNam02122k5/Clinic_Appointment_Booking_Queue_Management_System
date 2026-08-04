import { createI18n } from 'vue-i18n'
import { env } from '@/config/env'
import en from './locales/en.json'
import vi from './locales/vi.json'

export const messages = { en, vi }
export const supportedLocales = Object.keys(messages) as Array<keyof typeof messages>

const LOCALE_KEY = 'app.locale'

function resolveInitialLocale(): string {
  const stored = localStorage.getItem(LOCALE_KEY)
  if (stored && supportedLocales.includes(stored as never)) return stored
  const nav = navigator.language.split('-')[0] ?? ''
  return supportedLocales.includes(nav as never) ? nav : env.defaultLocale
}

export const i18n = createI18n({
  legacy: false, // Composition API mode
  locale: resolveInitialLocale(),
  fallbackLocale: env.defaultLocale,
  messages,
})

export function setLocale(locale: string) {
  i18n.global.locale.value = locale as never
  localStorage.setItem(LOCALE_KEY, locale)
  document.documentElement.lang = locale
}
