<script setup lang="ts">
import { RouterLink } from 'vue-router'
import { useI18n } from 'vue-i18n'
import BaseCard from '@/components/ui/BaseCard.vue'

const { t } = useI18n()
const features = [
  'Vue 3.5 + Vite',
  'Pinia stores',
  'Vue Router + guards',
  'Tailwind CSS v4',
  'vue-i18n (en/vi)',
  'Axios API layer',
]
</script>

<template>
  <section class="flex flex-col gap-6">
    <div>
      <h1 class="text-3xl font-bold">{{ t('app.name') }}</h1>
      <p class="mt-2 text-gray-500">{{ t('app.tagline') }}</p>
    </div>
    <div class="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
      <BaseCard v-for="f in features" :key="f">
        <div class="p-4 text-sm font-medium">{{ f }}</div>
      </BaseCard>
    </div>
    <RouterLink to="/users" class="text-brand-600 hover:underline">
      {{ t('nav.users') }} →
    </RouterLink>
  </section>
</template>
