<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { usePatientStore } from '@/stores/patient'
import { areProtectedPatientEndpointsDisabled, enableProtectedPatientEndpoints } from '@/features/patients/patient.api'

const patient = usePatientStore()
const tryingToConnect = ref(false)

// Tải lịch sử khám bệnh khi component được mount
onMounted(() => {
  patient.loadHistory()
})

// Hàm xử lý khi người dùng nhấn nút "Kết nối lại với backend"
async function connectToBackend() {
  tryingToConnect.value = true
  try {
    enableProtectedPatientEndpoints()
    await patient.loadHistory()
  } finally {
    tryingToConnect.value = false
  }
}

// Hàm bổ trợ định dạng ngày tháng
const formatDate = (dateString?: string) => {
  if (!dateString) return ''

  const normalized = String(dateString).trim()
  if (!normalized) return ''

  const safeDate = normalized.includes('T')
    ? new Date(normalized)
    : new Date(`${normalized}T00:00:00`)

  if (Number.isNaN(safeDate.getTime())) return normalized.slice(0, 10)

  return safeDate.toLocaleDateString('vi-VN')
}
</script>

<template>
  <div class="mx-auto max-w-2xl space-y-4">

    <!-- Header -->
    <div>
      <h1 class="text-2xl font-bold text-slate-800">
        Lịch sử khám bệnh
      </h1>

      <p class="mt-1 text-sm text-slate-400">
        {{ patient.history?.length || 0 }} lần khám đã lưu
      </p>
    </div>

    <!-- Trạng thái 1: Đang tải -->
    <div
      v-if="patient.historyLoading"
      class="py-10 text-center text-sm text-slate-400"
    >
      Đang tải lịch sử khám...
    </div>

    <!-- Nếu protected endpoints đã bị disable, cho phép người dùng bật lại -->
    <div v-else-if="areProtectedPatientEndpointsDisabled()" class="rounded-2xl border border-amber-200 bg-amber-50 p-6 text-center text-sm text-amber-800">
      Hệ thống hiện không kết nối tới API hồ sơ bệnh nhân (được tắt để tránh lỗi). Bạn có muốn thử kết nối lại tới backend?
      <div class="mt-3">
        <button @click="connectToBackend" :disabled="tryingToConnect" class="px-4 py-2 rounded bg-[#0E4D92] text-white">{{ tryingToConnect ? 'Đang kết nối...' : 'Kết nối lại với backend' }}</button>
      </div>
    </div>

    <!-- Trạng thái 2: Lỗi -->
    <div
      v-else-if="patient.historyError"
      class="rounded-2xl border border-red-200 bg-red-50 p-6 text-center text-sm text-red-600"
    >
      {{ patient.historyError }}
    </div>

    <!-- Trạng thái 3: Trống dữ liệu -->
    <div
      v-else-if="!patient.history || patient.history.length === 0"
      class="rounded-2xl border border-slate-200 bg-white p-6 text-center text-sm text-slate-400"
    >
      Chưa có lịch sử khám.
    </div>


    <!-- Trạng thái 4: Hiển thị danh sách -->
    <template v-else>
      <article
        v-for="record in patient.history"
        :key="record.id"
        class="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm"
      >
        <div class="mb-3 flex items-start justify-between gap-3">
          <div>
            <p class="text-xs text-slate-400">
              {{ formatDate(record.examinationDate) }}
            </p>

            <h2 class="font-bold text-slate-800">
              {{ record.doctorName }}
            </h2>
          </div>

          <span
            class="rounded-full border border-blue-200 bg-blue-50 px-2.5 py-1 text-xs text-blue-700"
          >
            {{ record.specialty }}
          </span>
        </div>

        <div class="space-y-3 rounded-xl bg-slate-50 p-4 text-sm">
          <div>
            <p class="text-xs font-medium uppercase tracking-wide text-slate-400">
              Chẩn đoán
            </p>
            <p class="mt-1 text-slate-800">
              {{ record.diagnosis }}
            </p>
          </div>

          <div class="border-t border-slate-200 pt-3">
            <p class="text-xs font-medium uppercase tracking-wide text-slate-400">
              Đơn thuốc
            </p>
            <p class="mt-1 text-slate-800">
              {{ record.prescription }}
            </p>
          </div>

          <div
            v-if="record.note"
            class="border-t border-slate-200 pt-3"
          >
            <p class="text-xs font-medium uppercase tracking-wide text-slate-400">
              Ghi chú
            </p>
            <p class="mt-1 italic text-slate-600">
              {{ record.note }}
            </p>
          </div>
        </div>
      </article>
    </template>

  </div>
</template>