<script setup lang="ts">
const props = defineProps<{
  currentPage: number
  totalPages: number
}>()

const emit = defineEmits<{
  'update:currentPage': [page: number]
}>()

function goToPage(page: number) {
  if (page < 1 || page > props.totalPages) {
    return
  }

  emit('update:currentPage', page)
}
</script>

<template>
  <div class="flex items-center justify-between pt-4">
    <p class="text-xs text-slate-500">
      Trang {{ currentPage }} / {{ totalPages }}
    </p>

    <div class="flex items-center gap-1">
      <button
        class="rounded-lg border border-slate-200 px-3 py-1.5
               text-xs text-slate-600
               hover:bg-slate-50
               disabled:cursor-not-allowed
               disabled:opacity-40"
        :disabled="currentPage === 1"
        @click="goToPage(currentPage - 1)"
      >
        Trước
      </button>

      <button
        v-for="page in totalPages"
        :key="page"
        class="h-8 min-w-8 rounded-lg px-2 text-xs font-medium"
        :class="
          page === currentPage
            ? 'bg-violet-600 text-white'
            : 'text-slate-600 hover:bg-slate-100'
        "
        @click="goToPage(page)"
      >
        {{ page }}
      </button>

      <button
        class="rounded-lg border border-slate-200 px-3 py-1.5
               text-xs text-slate-600
               hover:bg-slate-50
               disabled:cursor-not-allowed
               disabled:opacity-40"
        :disabled="currentPage === totalPages"
        @click="goToPage(currentPage + 1)"
      >
        Sau
      </button>
    </div>
  </div>
</template>
