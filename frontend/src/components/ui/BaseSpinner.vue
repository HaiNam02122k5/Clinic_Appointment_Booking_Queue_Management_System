<script setup lang="ts">
import { useI18n } from 'vue-i18n'
defineProps<{ label?: string }>()
const { t } = useI18n()
</script>

<template>
  <div role="status" class="flex items-center justify-center gap-3 p-8 text-gray-500">
    <span class="h-6 w-6 animate-spin rounded-full border-2 border-current border-t-transparent" />
    <span>{{ label ?? t('common.loading') }}</span>
  </div>
</template>
