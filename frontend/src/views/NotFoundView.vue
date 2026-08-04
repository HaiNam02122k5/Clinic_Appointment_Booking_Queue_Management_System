<script setup lang="ts">
import { RouterLink } from 'vue-router'
import { useI18n } from 'vue-i18n'
const { t } = useI18n()
</script>

<template>
  <div class="flex min-h-screen flex-col items-center justify-center gap-4 p-8 text-center">
    <p class="text-brand-600 text-6xl font-black">404</p>
    <p class="text-gray-500">{{ t('common.notFound') }}</p>
    <RouterLink to="/" class="text-brand-600 hover:underline">
      {{ t('common.backHome') }}
    </RouterLink>
  </div>
</template>
