<script setup lang="ts">
import { ref } from 'vue'
import { receptionistApi } from '../receptionist.api'
import type { PatientSearchItem } from '../receptionist.types'

const props = defineProps<{
  selectedPatientId?: string | null
}>()

const emit = defineEmits<{
  (e: 'select-patient', patient: PatientSearchItem): void
  (e: 'clear'): void
}>()

const search = ref('')
const results = ref<PatientSearchItem[]>([])
const loading = ref(false)
const hasSearched = ref(false)

let debounceTimer: ReturnType<typeof setTimeout> | null = null

function onInput() {
  if (debounceTimer) clearTimeout(debounceTimer)
  const query = search.value.trim()
  if (query.length < 2) {
    results.value = []
    hasSearched.value = false
    return
  }

  debounceTimer = setTimeout(async () => {
    loading.value = true
    hasSearched.value = true
    try {
      const res = await receptionistApi.searchPatients(query, 1, 10)
      results.value = res.items || []
    } catch {
      results.value = []
    } finally {
      loading.value = false
    }
  }, 350)
}

function selectPatient(patient: PatientSearchItem) {
  emit('select-patient', patient)
}

function clearSearch() {
  search.value = ''
  results.value = []
  hasSearched.value = false
  emit('clear')
}
</script>

<template>
  <div class="rounded-2xl border border-slate-200 bg-white p-5 shadow-2xs space-y-4">
    <div>
      <h2 class="text-sm font-bold uppercase tracking-wider text-slate-700">
        1. Tìm kiếm bệnh nhân
      </h2>
      <p class="text-xs text-slate-500 mt-0.5">
        Nhập số điện thoại, họ tên hoặc mã BHYT để tra cứu
      </p>
    </div>

    <div class="relative">
      <input
        v-model="search"
        type="text"
        placeholder="Ví dụ: 0987654321, Nguyễn Văn A..."
        class="w-full rounded-xl border border-slate-200 bg-slate-50/50 px-3.5 py-2.5 pl-10 pr-10 text-sm text-slate-800 placeholder:text-slate-400 focus:border-[#0E4D92] focus:bg-white focus:outline-none"
        @input="onInput"
      />
      <svg
        class="absolute left-3.5 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400"
        viewBox="0 0 24 24"
        fill="none"
        stroke="currentColor"
        stroke-width="2"
      >
        <circle cx="11" cy="11" r="7" />
        <path d="m20 20-3.5-3.5" />
      </svg>
      <button
        v-if="search"
        type="button"
        class="absolute right-3 top-1/2 -translate-y-1/2 text-xs font-bold text-slate-400 hover:text-slate-600"
        @click="clearSearch"
      >
        ✕
      </button>
    </div>

    <!-- SEARCH RESULTS DROPDOWN / LIST -->
    <div v-if="loading" class="text-xs text-violet-600 font-semibold flex items-center gap-2 py-2">
      <span class="animate-spin">🌀</span>
      <span>Đang tìm kiếm bệnh nhân...</span>
    </div>

    <div
      v-else-if="results.length > 0"
      class="max-h-64 overflow-y-auto divide-y divide-slate-100 rounded-xl border border-slate-200 bg-slate-50/30"
    >
      <div
        v-for="p in results"
        :key="p.id"
        class="p-3 hover:bg-violet-50/60 cursor-pointer transition-colors flex items-center justify-between"
        :class="selectedPatientId === p.id ? 'bg-violet-50 border-l-4 border-[#0E4D92]' : ''"
        @click="selectPatient(p)"
      >
        <div>
          <div class="font-bold text-slate-800 text-sm">
            {{ p.fullName }}
          </div>
          <div class="text-xs text-slate-500 mt-0.5 flex flex-wrap gap-x-3">
            <span v-if="p.phoneNumber">📞 {{ p.phoneNumber }}</span>
            <span v-if="p.insuranceNumber">🏥 BHYT: {{ p.insuranceNumber }}</span>
          </div>
        </div>

        <button
          type="button"
          class="rounded-lg bg-white border border-slate-200 px-2.5 py-1 text-xs font-semibold text-[#0E4D92] hover:bg-[#0E4D92] hover:text-white transition-colors"
        >
          Chọn
        </button>
      </div>
    </div>

    <div
      v-else-if="hasSearched && !loading"
      class="rounded-xl border border-dashed border-slate-200 p-4 text-center text-xs text-slate-400"
    >
      Không tìm thấy bệnh nhân nào khớp với "{{ search }}"
    </div>
  </div>
</template>
