<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { RouterLink } from 'vue-router'

import { useAuthStore } from '@/stores/auth'
import { usePatientStore } from '@/stores/patient'
import BaseAlert from '@/components/ui/BaseAlert.vue'
import PatientAppointmentCard from '@/features/patients/components/PatientAppointmentCard.vue'
import PatientQueueCard from '@/features/patients/components/PatientQueueCard.vue'
import PatientHistoryCard from '@/features/patients/components/PatientHistoryCard.vue'
import PatientBackendNotice from '@/features/patients/components/PatientBackendNotice.vue'

const auth = useAuthStore()
const patient = usePatientStore()

const today = new Date().toLocaleDateString('vi-VN', {
  weekday: 'long',
  day: '2-digit',
  month: '2-digit',
  year: 'numeric',
})

const initialLoading = ref(true)
const cancellingAppointmentId = ref<string | number | null>(null)

// Kiểm tra trạng thái kết nối tới backend và các dịch vụ bệnh nhân
const connecting = computed(() => {
  return (
    initialLoading.value ||
    patient.appointmentsLoading ||
    patient.queueLoading ||
    patient.historyLoading
  )
})

// Kiểm tra xem có bất kỳ lỗi nào từ các dịch vụ bệnh nhân hay không
const hasAnyError = computed(() => {
  return Boolean(
    patient.appointmentsError || patient.queueError || patient.historyError,
  )
})

async function reloadAllData() {
  const tasks = [
    patient.loadAppointments(),
    patient.loadQueue(),
    patient.loadHistory(),
  ]

  try {
    if (!patient.profile && localStorage.getItem('clinic.patient.protected-disabled') !== '1') {
      void patient.loadProfile().catch(() => undefined)
    }
  } catch {
    // ignore
  }

  try {
    await Promise.allSettled(tasks)
  } finally {
    initialLoading.value = false
  }
}

// Tải dữ liệu bệnh nhân khi component được mount
onMounted(async () => {
  await reloadAllData()
})

async function retryAppointments() {
  await patient.loadAppointments()
}

async function retryQueue() {
  await patient.loadQueue()
}

async function retryHistory() {
  await patient.loadHistory()
}

async function handleCancelAppointment(id: string | number) {
  if (!confirm('Bạn có chắc chắn muốn hủy lịch hẹn này không?')) return
  cancellingAppointmentId.value = id
  try {
    await patient.cancelAppointment(id)
  } finally {
    cancellingAppointmentId.value = null
  }
}
</script>

<template>
  <div class="mx-auto max-w-3xl space-y-5">
    <!-- Notice when protected patient endpoints are disabled -->
    <PatientBackendNotice @reconnected="reloadAllData" />

    <!-- Top banners (global) -->
    <BaseAlert
      v-if="connecting"
      type="info"
      :message="initialLoading ? 'Đang kết nối tới dịch vụ bệnh nhân...' : 'Đang đồng bộ dữ liệu...'"
    />

    <BaseAlert
      v-else-if="!connecting && hasAnyError"
      type="warning"
      title="Một hoặc nhiều dịch vụ gặp sự cố"
      message="Hệ thống đang sử dụng dữ liệu tạm. Bạn có thể nhấn 'Thử lại' ở từng khu vực bên dưới."
    />

    <main class="flex-1 space-y-5">
      <!-- Profile summary (always visible) -->
      <section class="rounded-2xl border border-slate-200 bg-white p-4 shadow-xs">
        <div class="flex items-center justify-between gap-4">
          <div class="flex items-center gap-3 min-w-0">
            <div class="h-12 w-12 rounded-full bg-[#0E4D92] flex items-center justify-center text-white font-bold text-lg shrink-0 shadow-2xs">
              {{ (patient.profile?.fullName ?? auth.user?.name ?? 'B').slice(0, 1).toUpperCase() }}
            </div>
            <div class="min-w-0">
              <p class="text-sm font-bold text-slate-800 truncate">
                {{ patient.profile?.fullName ?? auth.user?.name ?? 'Bệnh nhân' }}
              </p>
              <p class="text-xs text-slate-500 truncate mt-0.5">
                {{ patient.profile?.email ?? auth.user?.email ?? 'Chưa có email' }}
              </p>
            </div>
          </div>
          <div class="flex items-center gap-2">
            <div class="hidden sm:block text-xs text-slate-600">
              Lịch sắp tới: <span class="font-bold text-[#0E4D92]">{{ patient.upcomingAppointments?.length || 0 }}</span>
            </div>
            <div class="hidden sm:block text-xs text-slate-600">
              Trong hàng đợi: <span class="font-bold text-[#0E4D92]">{{ patient.queue?.myTicket ? 'Có' : 'Chưa' }}</span>
            </div>

            <RouterLink
              to="/patient/booking"
              class="px-3.5 py-1.5 rounded-xl bg-[#0E4D92] text-white text-xs font-semibold hover:bg-[#0b3d75] transition-colors"
            >
              + Đặt khám
            </RouterLink>
          </div>
        </div>

        <div v-if="auth.error" class="mt-3 text-xs text-amber-700 bg-amber-50 p-2.5 rounded-lg border border-amber-200 flex items-center justify-between">
          <span>⚠️ {{ auth.error }}</span>
          <RouterLink to="/profile" class="font-semibold text-[#0E4D92] hover:underline">Hoàn thiện hồ sơ →</RouterLink>
        </div>
      </section>

      <!-- Greeting Banner -->
      <section class="flex items-center justify-between rounded-2xl bg-gradient-to-r from-[#0E4D92] to-[#1a6bbf] p-5 text-white shadow-xs">
        <div>
          <p class="text-xs font-medium text-blue-100">Xin chào,</p>
          <h1 class="text-2xl font-extrabold tracking-tight mt-0.5">
            {{ patient.profile?.fullName || auth.user?.name || 'Bệnh nhân' }} 👋
          </h1>
          <p class="mt-1 text-xs text-blue-100/90">{{ today }}</p>
        </div>

        <div class="flex h-14 w-14 items-center justify-center rounded-2xl bg-white/15 text-3xl shrink-0 backdrop-blur-xs">
          🧑‍⚕️
        </div>
      </section>

      <!-- Quick Actions -->
      <div class="grid grid-cols-1 gap-3 sm:grid-cols-3">
        <RouterLink
          to="/patient/booking"
          class="rounded-2xl bg-[#0E4D92] p-4 text-white transition-all hover:-translate-y-0.5 hover:shadow-md active:translate-y-0"
        >
          <div class="mb-2 text-2xl">📅</div>
          <p class="text-sm font-bold">Đặt lịch khám</p>
          <p class="text-xs text-blue-200 mt-0.5">Chọn BS & khung giờ</p>
        </RouterLink>

        <RouterLink
          to="/patient/queue"
          class="rounded-2xl bg-[#00A878] p-4 text-white transition-all hover:-translate-y-0.5 hover:shadow-md active:translate-y-0"
        >
          <div class="mb-2 text-2xl">🔢</div>
          <p class="text-sm font-bold">Theo dõi hàng đợi</p>
          <p class="text-xs text-emerald-100 mt-0.5">Xem số thứ tự trực tiếp</p>
        </RouterLink>

        <RouterLink
          to="/patient/history"
          class="rounded-2xl border border-slate-200 bg-white p-4 text-slate-800 transition-all hover:bg-slate-50 hover:-translate-y-0.5 hover:shadow-sm active:translate-y-0"
        >
          <div class="mb-2 text-2xl">📋</div>
          <p class="text-sm font-bold text-slate-800">Lịch sử khám</p>
          <p class="text-xs text-slate-500 mt-0.5">Xem đơn thuốc & kết quả</p>
        </RouterLink>
      </div>

      <!-- Upcoming Appointment Section -->
      <section class="rounded-2xl border border-slate-200 bg-white p-5 shadow-xs">
        <div class="mb-4 flex items-center justify-between">
          <h2 class="font-bold text-slate-800 text-base">Lịch hẹn sắp tới</h2>
          <div class="flex items-center gap-3">
            <RouterLink to="/patient/booking" class="text-xs font-semibold text-[#0E4D92] hover:underline">+ Đặt thêm</RouterLink>
            <button
              v-if="patient.appointmentsError"
              @click="retryAppointments"
              class="text-xs rounded-lg px-2.5 py-1 bg-red-50 text-red-600 border border-red-200 font-semibold hover:bg-red-100"
            >
              Thử lại
            </button>
          </div>
        </div>

        <div v-if="patient.appointmentsLoading && initialLoading" class="rounded-xl bg-slate-50 p-4 text-sm text-slate-500 text-center">
          Đang tải lịch hẹn...
        </div>

        <div v-else-if="patient.appointmentsError" class="rounded-xl bg-red-50 p-4 text-sm text-red-600 border border-red-200">
          {{ patient.appointmentsError }}
        </div>

        <div v-else-if="!patient.upcomingAppointments?.length" class="rounded-xl bg-slate-50 p-4 text-sm text-slate-500 text-center">
          Bạn chưa có lịch hẹn sắp tới nào.
        </div>

        <div v-else class="space-y-3">
          <PatientAppointmentCard
            v-for="appointment in patient.upcomingAppointments"
            :key="appointment.id"
            :appointment="appointment"
            :cancelling="cancellingAppointmentId === appointment.id"
            @cancel="handleCancelAppointment"
          />
        </div>
      </section>

      <!-- Queue Status Section -->
      <section class="rounded-2xl border border-slate-200 bg-white p-5 shadow-xs">
        <div class="flex items-center justify-between mb-4">
          <h2 class="font-bold text-slate-800 text-base">Trạng thái hàng đợi</h2>
          <div>
            <button
              v-if="patient.queueError"
              @click="retryQueue"
              class="text-xs rounded-lg px-2.5 py-1 bg-red-50 text-red-600 border border-red-200 font-semibold hover:bg-red-100"
            >
              Thử lại
            </button>
          </div>
        </div>

        <div v-if="patient.queueLoading && initialLoading" class="py-6 text-center text-sm text-slate-400">
          Đang tải hàng đợi...
        </div>

        <div v-else-if="patient.queueError" class="rounded-xl border border-red-200 bg-red-50 p-6 text-center text-sm text-red-600">
          {{ patient.queueError }}
        </div>

        <PatientQueueCard
          v-else
          :queue="patient.queue"
          :show-details-link="true"
        />
      </section>

      <!-- Recent History Section -->
      <section class="rounded-2xl border border-slate-200 bg-white p-5 shadow-xs">
        <div class="mb-4 flex items-center justify-between">
          <h2 class="font-bold text-slate-800 text-base">Lịch khám gần đây</h2>
          <div class="flex items-center gap-3">
            <RouterLink to="/patient/history" class="text-xs font-semibold text-[#0E4D92] hover:underline">Xem tất cả →</RouterLink>
            <button
              v-if="patient.historyError"
              @click="retryHistory"
              class="text-xs rounded-lg px-2.5 py-1 bg-red-50 text-red-600 border border-red-200 font-semibold hover:bg-red-100"
            >
              Thử lại
            </button>
          </div>
        </div>

        <div v-if="patient.historyLoading && initialLoading" class="py-6 text-center text-sm text-slate-400">
          Đang tải lịch sử khám...
        </div>

        <div v-else-if="patient.historyError" class="rounded-xl border border-red-200 bg-red-50 p-6 text-center text-sm text-red-600">
          {{ patient.historyError }}
        </div>

        <div v-else-if="!patient.history || patient.history.length === 0" class="rounded-2xl border border-slate-200 bg-white p-6 text-center text-sm text-slate-400">
          Chưa có lịch sử khám bệnh nào được ghi nhận.
        </div>

        <div v-else class="space-y-4">
          <PatientHistoryCard
            v-for="record in patient.history.slice(0, 3)"
            :key="record.id"
            :record="record"
          />
        </div>
      </section>
    </main>
  </div>
</template>