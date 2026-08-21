<script setup lang="ts">
import type { QueueStatus } from '../receptionist.mock'

defineProps<{
  status: QueueStatus | string
}>()
</script>

<template>
  <span
    class="text-xs font-medium px-2.5 py-1 rounded"
    :class="{
      'bg-blue-50 text-blue-700': status === 'examining',
      'bg-amber-50 text-amber-700': status === 'waiting',
      'bg-emerald-50 text-emerald-700': status === 'completed'
    }"
  >
    {{
      status === 'examining'
        ? 'Đang khám'
        : status === 'waiting'
          ? 'Chờ khám'
          : 'Đã khám xong'
    }}
  </span>
</template>
