/**
 * Centralized, typed access to runtime configuration.
 * Never read `import.meta.env` directly elsewhere — import from here.
 */
export const env = {
  appName: import.meta.env.VITE_APP_NAME ?? 'Clinic',
  // địa chỉ API của backend chạy riêng
  apiBaseUrl: import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5000/api',
  defaultLocale: import.meta.env.VITE_DEFAULT_LOCALE ?? 'vo',
  enableMock: import.meta.env.VITE_ENABLE_MOCK === 'true',
  isDev: import.meta.env.DEV,
  isProd: import.meta.env.PROD,
} as const

export type AppEnv = typeof env
