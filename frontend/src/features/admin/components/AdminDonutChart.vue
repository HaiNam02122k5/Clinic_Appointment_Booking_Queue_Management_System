<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps<{
  data: {
    label: string
    value: number
    color: string
  }[]
}>()

const total = computed(() =>
  props.data.reduce((sum, item) => sum + item.value, 0),
)

const gradient = computed(() => {
  if (total.value === 0) {
    return '#E2E8F0'
  }

  let currentDegree = 0

  const parts = props.data.map((item) => {
    const degree = (item.value / total.value) * 360
    const start = currentDegree
    const end = currentDegree + degree

    currentDegree = end

    return `${item.color} ${start}deg ${end}deg`
  })

  return `conic-gradient(${parts.join(', ')})`
})
</script>

<template>
  <div class="flex items-center gap-8">
    <div class="relative h-40 w-40 shrink-0">
      <div
        class="h-full w-full rounded-full"
        :style="{
          background: gradient,
        }"
      />

      <div
        class="absolute inset-7 flex items-center justify-center rounded-full bg-white"
      >
        <span class="text-lg font-bold text-slate-800">
          {{ total }}
        </span>
      </div>
    </div>

    <div class="space-y-3">
      <div
        v-for="item in data"
        :key="item.label"
        class="flex items-center gap-2"
      >
        <span
          class="h-2.5 w-2.5 rounded-full"
          :style="{ backgroundColor: item.color }"
        />

        <span class="text-xs text-slate-600">
          {{ item.label }}
        </span>

        <span class="ml-2 text-xs font-semibold text-slate-800">
          {{ item.value }}
        </span>
      </div>
    </div>
  </div>
</template>
