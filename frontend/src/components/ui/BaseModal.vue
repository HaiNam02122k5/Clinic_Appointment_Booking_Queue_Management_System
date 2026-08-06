<script setup lang="ts">
defineProps<{
  open: boolean
  title?: string
}>()

const emit = defineEmits<{
  (e: 'close'): void
}>()
</script>

<template>
  <Teleport to="body">
    <div
      v-if="open"
      class="fixed inset-0 z-50 flex items-center justify-center bg-black/50"
      @click.self="emit('close')"
    >
      <div class="w-full max-w-lg rounded-lg bg-white shadow-lg">

        <!-- Header -->
        <div class="flex items-center justify-between border-b p-4">
          <h2 class="text-lg font-semibold">
            {{ title }}
          </h2>

          <button
            class="text-2xl text-gray-500 hover:text-black"
            @click="emit('close')"
          >
            ×
          </button>
        </div>

        <!-- Body -->
        <div class="p-4">
          <slot />
        </div>

        <!-- Footer -->
        <div
          v-if="$slots.footer"
          class="flex justify-end gap-2 border-t p-4"
        >
          <slot name="footer" />
        </div>

      </div>
    </div>
  </Teleport>
</template>
