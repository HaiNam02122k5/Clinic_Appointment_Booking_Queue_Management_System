<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { RouterLink } from 'vue-router'

import { useAuthStore } from '@/stores/auth'
import { usePatientStore } from '@/stores/patient'

const auth = useAuthStore()
const patient = usePatientStore()

const today = new Date().toLocaleDateString('vi-VN', {
  weekday: 'long',
  day: '2-digit',
  month: '2-digit',
  year: 'numeric',
})

// initial loading when page first mounts
const initialLoading = ref(true)

// Compute overall connectivity status for display (true when any of the services is loading on first load)
const connecting = computed(() => {
  return (
    initialLoading.value ||
    patient.appointmentsLoading ||
    patient.queueLoading ||
    patient.historyLoading
  )
})

// Derived helpers for template convenience
const hasAnyError = computed(() => {
  return Boolean(
    patient.appointmentsError || patient.queueError || patient.historyError,
  )
})

onMounted(async () => {
  // Ensure backend has a patient profile for this user if possible
  try {
    // ensurePatientProfile returns true when profile exists or was created
    await auth.ensurePatientProfile().catch(() => {
      // swallow — ensurePatientProfile puts friendly messages into auth.error
    })
  } catch {
    // noop
  }

  // Start three loads in parallel but wait for all settled so we can stop initialLoading
  const tasks = [
    patient.loadAppointments(),
    patient.loadQueue(),
    patient.loadHistory(),
  ]

  try {
    await Promise.allSettled(tasks)
  } finally {
    // Mark initial load complete — UI will then reflect per-service states from the store
    initialLoading.value = false
  }
})

// Retry helpers for each section
async function retryAppointments() {
  await patient.loadAppointments()
}

async function retryQueue() {
  await patient.loadQueue()
}

async function retryHistory() {
  await patient.loadHistory()
}
</script>

<template>
  <div class="mx-auto max-w-3xl space-y-5">
    <!-- Top banners (global) -->
    <div v-if="connecting" class="rounded-xl border border-blue-200 bg-blue-50 px-4 py-3 text-sm text-blue-800 mb-4">
      {{ initialLoading ? 'Đang kết nối tới dịch vụ bệnh nhân...' : 'Đang đồng bộ dữ liệu...' }}
    </div>

    <div v-else-if="!connecting && hasAnyError" class="rounded-xl border border-amber-200 bg-amber-50 px-4 py-3 text-sm text-amber-800 mb-4">
      Một hoặc nhiều dịch vụ đang gặp sự cố — xem chi tiết bên dưới và thử lại từng mục.
    </div>

      <main class="flex-1 space-y-5">
        <!-- Profile summary (always visible) -->
        <section class="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm mb-2">
          <div class="flex items-center justify-between gap-4">
            <div class="flex items-center gap-3 min-w-0">
              <div class="h-12 w-12 rounded-full bg-[#6b46c1] flex items-center justify-center text-white font-bold text-lg">{{ (auth.user?.name || 'B').slice(0,1) }}</div>
              <div class="min-w-0">
                <p class="text-sm font-semibold text-slate-800 truncate">{{ auth.user?.name || 'Bệnh nhân' }}</p>
                <p class="text-xs text-slate-400 truncate">{{ auth.user?.email || '' }}</p>
              </div>
            </div>

            <div class="flex items-center gap-2">
              <div class="hidden sm:block text-sm text-slate-600">Lịch sắp tới: <span class="font-semibold text-[#0E4D92]">{{ patient.upcomingAppointments?.length || 0 }}</span></div>
              <div class="hidden sm:block text-sm text-slate-600">Trong hàng đợi: <span class="font-semibold text-[#0E4D92]">{{ patient.queue ? 'Có' : 'Chưa' }}</span></div>

              <RouterLink to="/patient/booking" class="px-3 py-1 rounded-md bg-[#0E4D92] text-white text-sm">Đặt khám</RouterLink>
            </div>
          </div>

          <div v-if="auth.error" class="mt-3 text-sm text-amber-700">
            ⚠️ {{ auth.error }}
            <RouterLink to="/profile" class="ml-3 text-sm font-medium text-[#0E4D92]">Hoàn thiện hồ sơ</RouterLink>
          </div>
        </section>

        <!-- Greeting -->
        <section
          class="flex items-center justify-between rounded-2xl bg-gradient-to-r from-[#0E4D92] to-[#1a6bbf] p-5 text-white"
        >
          <div>
            <p class="text-sm text-blue-200">Xin chào,</p>
            <h1 class="text-2xl font-bold">{{ auth.user?.name || 'Bệnh nhân' }} 👋</h1>
            <p class="mt-1 text-xs text-blue-200">{{ today }}</p>
          </div>

          <div class="flex h-16 w-16 items-center justify-center rounded-2xl bg-white/10 text-3xl">🧑‍⚕️</div>
        </section>

        <!-- Quick Actions (kept compact on main) -->
        <div class="grid grid-cols-1 gap-3 sm:grid-cols-3">
          <RouterLink to="/patient/booking" class="rounded-2xl bg-[#0E4D92] p-4 text-white transition hover:-translate-y-0.5 hover:shadow-md">
            <div class="mb-2 text-xl">📅</div>
            <p class="text-sm font-semibold">Đặt lịch khám</p>
          </RouterLink>

          <RouterLink to="/patient/queue" class="rounded-2xl bg-[#00A878] p-4 text-white transition hover:-translate-y-0.5 hover:shadow-md">
            <div class="mb-2 text-xl">🔢</div>
            <p class="text-sm font-semibold">Theo dõi hàng đợi</p>
          </RouterLink>

          <RouterLink to="/patient/history" class="rounded-2xl border border-slate-200 bg-white p-4 text-slate-700 transition hover:bg-slate-50">
            <div class="mb-2 text-xl">📋</div>
            <p class="text-sm font-semibold">Lịch sử khám</p>
          </RouterLink>
        </div>

        <!-- Upcoming Appointment (unchanged) -->
        <section class="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
          <div class="mb-4 flex items-center justify-between">
            <h2 class="font-semibold text-slate-700">Lịch hẹn sắp tới</h2>
            <div class="flex items-center gap-3">
              <RouterLink to="/patient/booking" class="text-xs font-medium text-[#0E4D92]">+ Đặt thêm</RouterLink>
              <button v-if="patient.appointmentsError" @click="retryAppointments" class="text-xs rounded px-2 py-1 bg-red-50 text-red-600 border border-red-100">Thử lại</button>
            </div>
          </div>

          <div v-if="patient.appointmentsLoading && initialLoading" class="rounded-xl bg-slate-50 p-4 text-sm text-slate-500">Đang tải lịch hẹn...</div>

          <div v-else-if="patient.appointmentsError" class="rounded-xl bg-red-50 p-4 text-sm text-red-600">{{ patient.appointmentsError }}</div>

          <div v-else-if="!patient.upcomingAppointments?.length" class="rounded-xl bg-slate-50 p-4 text-sm text-slate-500">Bạn chưa có lịch hẹn sắp tới.</div>

          <div v-else>
            <div v-for="appointment in patient.upcomingAppointments" :key="appointment.id" class="rounded-xl border border-blue-200 bg-blue-50 p-4 mb-3">
              <div class="flex items-center gap-4">
                <div class="w-16 text-center">
                  <p class="text-xs font-semibold text-blue-600">{{ appointment.appointmentDate }}</p>
                  <p class="text-xl font-bold text-[#0E4D92]">{{ appointment.appointmentTime }}</p>
                </div>

                <div class="h-10 w-px bg-blue-200" />

                <div class="min-w-0 flex-1">
                  <p class="font-semibold text-slate-800">{{ appointment.doctorName }}</p>
                  <p class="text-xs text-slate-400">{{ appointment.specialty }}</p>
                </div>

                <div class="text-right">
                  <span class="rounded-full bg-amber-50 px-2.5 py-1 text-xs font-medium text-amber-700">{{ appointment.status }}</span>

                  <p v-if="appointment.queueNumber" class="mt-1 text-xs font-bold text-[#0E4D92]">{{ appointment.queueNumber }}</p>
                </div>
              </div>

              <button v-if="['Pending', 'Confirmed'].includes(appointment.status)" class="mt-3 text-xs font-medium text-red-500" @click="patient.cancelAppointment(appointment.id)">Hủy lịch</button>
            </div>
          </div>
        </section>

        <!-- Queue (unchanged) -->
        <section class="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
          <div class="flex items-center justify-between mb-4">
            <h2 class="font-semibold text-slate-700">Trạng thái hàng đợi</h2>
            <div>
              <button v-if="patient.queueError" @click="retryQueue" class="text-xs rounded px-2 py-1 bg-red-50 text-red-600 border border-red-100">Thử lại</button>
            </div>
          </div>

          <div v-if="patient.queueLoading && initialLoading" class="py-6 text-center text-sm text-slate-400">Đang tải hàng đợi...</div>

          <div v-else-if="patient.queueError" class="rounded-xl border border-red-200 bg-red-50 p-6 text-center text-sm text-red-600">{{ patient.queueError }}</div>

          <div v-else-if="patient.queue" class="flex items-center gap-5">
            <div class="text-center">
              <p class="text-xs text-slate-400">Số của bạn</p>
              <p class="text-4xl font-bold text-[#0E4D92]">{{ patient.queue.myTicket }}</p>
            </div>

            <div class="grid flex-1 grid-cols-2 gap-2">
              <div class="rounded-xl bg-slate-50 p-3">
                <p class="text-xs text-slate-400">Vị trí</p>
                <p class="font-bold">#{{ patient.queue.position }}</p>
              </div>

              <div class="rounded-xl bg-slate-50 p-3">
                <p class="text-xs text-slate-400">Chờ ~</p>
                <p class="font-bold text-amber-600">{{ patient.queue.estimatedWaitMinutes }} phút</p>
              </div>
            </div>
          </div>

          <div v-else class="text-sm text-slate-500">Hiện tại bạn không có trong hàng đợi.</div>

          <RouterLink to="/patient/queue" class="mt-4 block w-full rounded-xl border-2 border-[#0E4D92] py-2 text-center text-sm font-semibold text-[#0E4D92]">Xem chi tiết hàng đợi</RouterLink>
        </section>

        <!-- Recent History (unchanged) -->
        <section class="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
          <div class="mb-4 flex items-center justify-between">
            <h2 class="font-semibold text-slate-700">Lịch khám gần đây</h2>
            <div class="flex items-center gap-3">
              <RouterLink to="/patient/history" class="text-xs font-medium text-[#0E4D92]">Xem tất cả</RouterLink>
              <button v-if="patient.historyError" @click="retryHistory" class="text-xs rounded px-2 py-1 bg-red-50 text-red-600 border border-red-100">Thử lại</button>
            </div>
          </div>

          <div v-if="patient.historyLoading && initialLoading" class="py-6 text-center text-sm text-slate-400">Đang tải lịch sử khám...</div>

          <div v-else-if="patient.historyError" class="rounded-xl border border-red-200 bg-red-50 p-6 text-center text-sm text-red-600">{{ patient.historyError }}</div>

          <template v-else>
            <div v-if="!patient.history || patient.history.length === 0" class="rounded-2xl border border-slate-200 bg-white p-6 text-center text-sm text-slate-400">Chưa có lịch sử khám.</div>

            <article v-else v-for="record in patient.history?.slice(0, 3)" :key="record.id" class="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
              <div class="mb-3 flex items-start justify-between gap-3">
                <div>
                  <p class="text-xs text-slate-400">{{ new Date(record.examinationDate).toLocaleDateString('vi-VN') }}</p>

                  <h2 class="font-bold text-slate-800">{{ record.doctorName }}</h2>
                </div>

                <span class="rounded-full border border-blue-200 bg-blue-50 px-2.5 py-1 text-xs text-blue-700">{{ record.specialty }}</span>
              </div>

              <div class="space-y-3 rounded-xl bg-slate-50 p-4 text-sm">
                <div>
                  <p class="text-xs font-medium uppercase tracking-wide text-slate-400">Chẩn đoán</p>
                  <p class="mt-1 text-slate-800">{{ record.diagnosis }}</p>
                </div>

                <div class="border-t border-slate-200 pt-3">
                  <p class="text-xs font-medium uppercase tracking-wide text-slate-400">Đơn thuốc</p>
                  <p class="mt-1 text-slate-800">{{ record.prescription }}</p>
                </div>

                <div v-if="record.note" class="border-t border-slate-200 pt-3">
                  <p class="text-xs font-medium uppercase tracking-wide text-slate-400">Ghi chú</p>
                  <p class="mt-1 italic text-slate-600">{{ record.note }}</p>
                </div>
              </div>
            </article>
          </template>
        </section>
      </main>
  </div>
</template>