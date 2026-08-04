<script setup lang="ts">
import { useId } from 'vue'

defineProps<{
  label?: string
  error?: string
  type?: string
  placeholder?: string
  required?: boolean
}>()

// Two-way binding via defineModel (Vue 3.4+).
const model = defineModel<string>({ required: true })
const inputId = useId()
</script>

<template>
  <div class="flex flex-col gap-1">
    <label v-if="label" :for="inputId" class="text-sm font-medium text-gray-700 dark:text-gray-300">
      {{ label }}
    </label>
    <input
      :id="inputId"
      v-model="model"
      :type="type ?? 'text'"
      :placeholder="placeholder"
      :required="required"
      :aria-invalid="!!error"
      class="focus:border-brand-500 focus:ring-brand-500/30 h-10 rounded-lg border border-gray-300 bg-white px-3 text-sm text-gray-900 transition-colors outline-none focus:ring-2 dark:border-gray-700 dark:bg-gray-900 dark:text-gray-100"
      :class="error && 'border-red-500 focus:border-red-500 focus:ring-red-500/30'"
    />
    <p v-if="error" class="text-xs text-red-600">{{ error }}</p>
  </div>
</template>
