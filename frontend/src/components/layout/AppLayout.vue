<script setup lang="ts">
import { RouterLink, RouterView, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useAuthStore } from '@/stores/auth'
import { supportedLocales, setLocale } from '@/lib/i18n'
import BaseButton from '@/components/ui/BaseButton.vue'

const { t, locale } = useI18n()
const router = useRouter()
const auth = useAuthStore()

function onLocaleChange(e: Event) {
  setLocale((e.target as HTMLSelectElement).value)
}

function logout() {
  auth.logout()
  router.push('/login')
}
</script>

<template>
  <div class="mx-auto flex min-h-full max-w-5xl flex-col">
    <header
      class="flex items-center justify-between gap-4 border-b border-gray-200 px-4 py-3 dark:border-gray-800"
    >
      <div class="flex items-center gap-6">
        <span class="text-brand-600 text-lg font-bold">{{ t('app.name') }}</span>
        <nav class="flex items-center gap-1">
          <RouterLink
            to="/"
            class="rounded-lg px-3 py-2 text-sm font-medium text-gray-600 hover:bg-gray-100 dark:text-gray-300 dark:hover:bg-gray-800"
            active-class="!bg-brand-50 !text-brand-700 dark:!bg-brand-500/10"
            >{{ t('nav.home') }}</RouterLink
          >
          <RouterLink
            to="/users"
            class="rounded-lg px-3 py-2 text-sm font-medium text-gray-600 hover:bg-gray-100 dark:text-gray-300 dark:hover:bg-gray-800"
            active-class="!bg-brand-50 !text-brand-700 dark:!bg-brand-500/10"
            >{{ t('nav.users') }}</RouterLink
          >
        </nav>
      </div>
      <div class="flex items-center gap-3">
        <select
          aria-label="Language"
          :value="locale"
          class="h-9 rounded-lg border border-gray-300 bg-white px-2 text-sm dark:border-gray-700 dark:bg-gray-900"
          @change="onLocaleChange"
        >
          <option v-for="l in supportedLocales" :key="l" :value="l">
            {{ l.toUpperCase() }}
          </option>
        </select>
        <span v-if="auth.user" class="text-sm text-gray-500">{{ auth.user.name }}</span>
        <BaseButton variant="ghost" size="sm" @click="logout">
          {{ t('nav.logout') }}
        </BaseButton>
      </div>
    </header>

    <main class="flex-1 p-4">
      <RouterView />
    </main>
  </div>
</template>
