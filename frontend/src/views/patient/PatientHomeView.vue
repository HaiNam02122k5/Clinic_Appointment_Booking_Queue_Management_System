<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue'
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

const initialLoading = ref(true)
const cancelingAppointmentId = ref<number | null>(null)
const toast = ref<{ visible: boolean; type: 'success' | 'error'; message: string } | null>(null)

function showToast(message: string, type: 'success' | 'error' = 'success') {
  toast.value = { visible: true, type, message }
  window.setTimeout(() => {
    toast.value = null
  }, 3000)
}

const connecting = computed(() => {
  return (
    initialLoading.value ||
    patient.appointmentsLoading ||
    patient.queueLoading ||
    patient.historyLoading
  )
})

const hasAnyError = computed(() => {
  return Boolean(
    patient.appointmentsError || patient.queueError || patient.historyError,
  )
})

onMounted(async () => {
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
  }

  try {
    await Promise.allSettled(tasks)
  } finally {
    initialLoading.value = false
  }
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

async function handleCancelAppointment(id: number) {
  const confirmed = typeof window !== 'undefined' && typeof window.confirm === 'function'
    ? window.confirm('Bạn có chắc chắn muốn hủy lịch khám này?')
    : true
  if (!confirmed) return

  cancelingAppointmentId.value = id
  try {
    await patient.cancelAppointment(id)
    showToast('Lịch khám đã được hủy thành công.', 'success')
  } catch (error: any) {
    const message = error?.message || patient.appointmentsError || 'Không thể hủy lịch khám.'
    showToast(message, 'error')
  } finally {
    cancelingAppointmentId.value = null
  }
}

const appointmentFilter = ref<'all' | 'upcoming' | 'confirmed' | 'cancelled' | 'pending'>('all')

function parseDate(value: unknown): Date | null {
  if (!value && value !== 0) return null
  const s = String(value).trim()
  if (!s) return null

  const dm = /^([0-3]?\d)\/(0?[1-9]|1[0-2])\/(\d{4})$/.exec(s)
  if (dm) {
    const d = new Date(Number(dm[3]), Number(dm[2]) - 1, Number(dm[1]))
    return Number.isNaN(d.getTime()) ? null : d
  }

  const parsed = new Date(s)
  return Number.isNaN(parsed.getTime()) ? null : parsed
}

function inNext7Days(date: Date): boolean {
  const start = new Date()
  start.setHours(0, 0, 0, 0)
  const end = new Date(start)
  end.setDate(end.getDate() + 6)
  const d = new Date(date)
  d.setHours(0, 0, 0, 0)
  return d >= start && d <= end
}

function normalizeStatus(value: unknown): string {
  if (value == null) return ''
  return String(value)
    .toLowerCase()
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')
    .replace(/\s+/g, ' ')
    .trim()
}

function isStatusConfirmed(raw: unknown): boolean {
  const s = normalizeStatus(raw)
  return s.includes('confirm') || s.includes('xac') || s.includes('da xac') || s.includes('xac nhan') || s === 'confirmed'
}

function isStatusPending(raw: unknown): boolean {
  const s = normalizeStatus(raw)
  return s.includes('pend') || s.includes('wait') || s.includes('cho') || s.includes('dang cho') || s === 'pending'
}

function isStatusCancelled(raw: unknown): boolean {
  const s = normalizeStatus(raw)
  return s.includes('cancel') || s.includes('huy') || s === 'cancelled' || s === 'canceled'
}

const appointmentsList = computed(() => {
  // patient.sortedAppointments and patient.upcomingAppointments are computed refs from the store
  // access their .value to get the underlying arrays so filtering works correctly
  return patient.sortedAppointments?.value ?? patient.upcomingAppointments?.value ?? patient.appointments ?? []
})

const displayedAppointments = computed(() => {
  const list = appointmentsList.value

  switch (appointmentFilter.value) {
    case 'upcoming':
      return list.filter((a: any) => {
        const dt = parseDate(a?.appointmentDate)
        if (!dt) return false
        if (!isStatusPending(a?.status) && !isStatusConfirmed(a?.status)) return false
        return inNext7Days(dt)
      })
    case 'confirmed':
      return list.filter((a: any) => isStatusConfirmed(a?.status))
    case 'cancelled':
      return list.filter((a: any) => isStatusCancelled(a?.status))
    case 'pending':
      return list.filter((a: any) => isStatusPending(a?.status))
    default:
      return list
  }
})

function setFilter(f: typeof appointmentFilter.value) {
  appointmentFilter.value = f
}

function appointmentHighlightClass(appointment: any) {
  try {
    const isInWeek = Array.isArray(patient.upcomingWeekIds) && patient.upcomingWeekIds.includes(appointment.id)
    return isInWeek
      ? 'rounded-xl border-2 border-blue-200 bg-white p-4 mb-3 shadow-sm'
      : 'rounded-xl border border-slate-200 bg-white p-4 mb-3'
  } catch {
    return 'rounded-xl border border-slate-200 bg-white p-4 mb-3'
  }
}

function formatStatusLabel(raw: unknown): string {
  const s = normalizeStatus(raw)
  if (s.includes('confirm') || s.includes('xac')) return 'Đã xác nhận'
  if (s.includes('pend') || s.includes('cho') || s.includes('wait')) return 'Chờ duyệt'
  if (s.includes('cancel') || s.includes('huy')) return 'Đã hủy'
  if (s.includes('check') || s.includes('dang den')) return 'Đã đến'
  if (s.includes('complete') || s.includes('hoan')) return 'Hoàn thành'
  return String(raw ?? '') || 'Không rõ'
}

function statusBadgeClass(raw: unknown): string {
  const s = normalizeStatus(raw)
  if (s.includes('confirm') || s.includes('xac')) return 'bg-emerald-50 text-emerald-700 border border-emerald-100'
  if (s.includes('pend') || s.includes('cho') || s.includes('wait')) return 'bg-amber-50 text-amber-700 border border-amber-100'
  if (s.includes('cancel') || s.includes('huy')) return 'bg-red-50 text-red-600 border border-red-100'
  if (s.includes('check') || s.includes('dang den')) return 'bg-blue-50 text-blue-700 border border-blue-100'
  if (s.includes('complete') || s.includes('hoan')) return 'bg-slate-50 text-slate-700 border border-slate-100'
  return 'bg-slate-50 text-slate-700 border border-slate-100'
}

watch(
  () => (patient.appointments?.length ?? patient.upcomingAppointments?.value?.length ?? appointmentsList.value.length ?? 0),
  () => {
    if (appointmentFilter.value === 'upcoming' && (patient.upcomingWeekCount?.value ?? 0) === 0) {
      appointmentFilter.value = 'all'
    }
  },
)
</script>

<template>
  <div class="mx-auto max-w-3xl space-y-5">
    <div v-if="connecting" class="mb-4 rounded-xl border border-blue-200 bg-blue-50 px-4 py-3 text-sm text-blue-800">
      {{ initialLoading ? 'Đang kết nối tới dịch vụ bệnh nhân...' : 'Đang đồng bộ dữ liệu...' }}
    </div>

    <div v-else-if="!connecting && hasAnyError" class="mb-4 rounded-xl border border-amber-200 bg-amber-50 px-4 py-3 text-sm text-amber-800">
      Một hoặc nhiều dịch vụ đang gặp sự cố — xem chi tiết bên dưới và thử lại từng mục.
    </div>

    <main class="flex-1 space-y-5">
      <section class="mb-2 rounded-2xl border border-slate-200 bg-white p-4 shadow-sm">
        <div class="flex items-center justify-between gap-4">
          <div class="flex min-w-0 items-center gap-3">
            <div class="flex h-12 w-12 items-center justify-center rounded-full bg-[#6b46c1] text-lg font-bold text-white">
              {{ (patient.profile?.fullName ?? auth.user?.name ?? 'B').slice(0, 1) }}
            </div>
            <div class="min-w-0">
              <p class="truncate text-sm font-semibold text-slate-800">
                {{ patient.profile?.fullName ?? auth.user?.name ?? 'Bệnh nhân' }}
              </p>
              <p class="truncate text-xs text-slate-400">
                {{ patient.profile?.email ?? auth.user?.email ?? '' }}
              </p>
            </div>
          </div>

          <div class="flex items-center gap-2">
            <div class="hidden text-sm text-slate-600 sm:block">
              Lịch sắp tới (trong vòng 1 tuần):
              <span class="font-semibold text-[#0E4D92]">{{ patient.upcomingWeekCount }}</span>
            </div>
            <div class="hidden text-sm text-slate-600 sm:block">
              Trong hàng đợi:
              <span class="font-semibold text-[#0E4D92]">{{ patient.queue?.myTicket ? 'Có' : 'Chưa' }}</span>
            </div>
            <RouterLink to="/patient/booking" class="rounded-md bg-[#0E4D92] px-3 py-1 text-sm text-white">Đặt khám</RouterLink>
          </div>
        </div>

        <div v-if="auth.error" class="mt-3 text-sm text-amber-700">
          ⚠️ {{ auth.error }}
          <RouterLink to="/profile" class="ml-3 text-sm font-medium text-[#0E4D92]">Hoàn thiện hồ sơ</RouterLink>
        </div>
      </section>

      <section class="flex items-center justify-between rounded-2xl bg-gradient-to-r from-[#0E4D92] to-[#1a6bbf] p-5 text-white">
        <div>
          <p class="text-sm text-blue-200">Xin chào,</p>
          <h1 class="text-2xl font-bold">{{ patient.profile?.fullName || auth.user?.name || 'Bệnh nhân' }} 👋</h1>
          <p class="mt-1 text-xs text-blue-200">{{ today }}</p>
        </div>

        <div class="flex h-16 w-16 items-center justify-center rounded-2xl bg-white/10 text-3xl">🧑‍⚕️</div>
      </section>

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

      <section class="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
        <div class="mb-4 flex items-center justify-between">
          <h2 class="font-semibold text-slate-700">Lịch hẹn của tôi</h2>
          <div class="flex items-center gap-3">
            <RouterLink to="/patient/booking" class="text-xs font-medium text-[#0E4D92]">+ Đặt thêm</RouterLink>
            <button v-if="patient.appointmentsError" @click="retryAppointments" class="rounded border border-red-100 bg-red-50 px-2 py-1 text-xs text-red-600">Thử lại</button>
          </div>
        </div>

        <div class="mb-4 flex flex-wrap items-center gap-2">
          <button :class="['rounded px-3 py-1 text-xs', appointmentFilter === 'all' ? 'bg-[#0E4D92] text-white' : 'border border-slate-200 bg-white text-slate-700']" @click.prevent="setFilter('all')">Tất cả</button>
          <button :class="['rounded px-3 py-1 text-xs', appointmentFilter === 'upcoming' ? 'bg-[#0E4D92] text-white' : 'border border-slate-200 bg-white text-slate-700']" @click.prevent="setFilter('upcoming')">Sắp tới</button>
          <button :class="['rounded px-3 py-1 text-xs', appointmentFilter === 'confirmed' ? 'bg-[#0E4D92] text-white' : 'border border-slate-200 bg-white text-slate-700']" @click.prevent="setFilter('confirmed')">Đã xác nhận</button>
          <button :class="['rounded px-3 py-1 text-xs', appointmentFilter === 'pending' ? 'bg-[#0E4D92] text-white' : 'border border-slate-200 bg-white text-slate-700']" @click.prevent="setFilter('pending')">Chờ duyệt</button>
          <button :class="['rounded px-3 py-1 text-xs', appointmentFilter === 'cancelled' ? 'bg-[#0E4D92] text-white' : 'border border-slate-200 bg-white text-slate-700']" @click.prevent="setFilter('cancelled')">Đã hủy</button>
        </div>

        <div v-if="patient.appointmentsLoading" class="rounded-xl bg-slate-50 p-4 text-sm text-slate-500">Đang tải lịch hẹn...</div>
        <div v-else-if="patient.appointmentsError" class="rounded-xl bg-red-50 p-4 text-sm text-red-600">{{ patient.appointmentsError }}</div>
        <div v-else-if="displayedAppointments.length === 0" class="rounded-xl bg-slate-50 p-4 text-sm text-slate-500">
          <span v-if="appointmentFilter === 'all'">Bạn chưa có lịch hẹn sắp tới.</span>
          <span v-else>Không tìm thấy lịch phù hợp với bộ lọc hiện tại.</span>
        </div>

        <div v-else>
          <div v-for="appointment in displayedAppointments" :key="appointment.id" :class="appointmentHighlightClass(appointment)">
            <div class="flex items-center gap-4">
              <div class="w-16 text-center">
                <p class="text-xs font-semibold text-blue-600">{{ appointment.appointmentDate }}</p>
                <p class="text-xl font-bold text-[#0E4D92]">{{ appointment.appointmentTime }}</p>
              </div>

              <div class="h-10 w-px bg-blue-200" />

              <div class="min-w-0 flex-1">
                <p class="font-semibold text-slate-800">
                  <span v-if="Array.isArray(patient.upcomingWeekIds) && patient.upcomingWeekIds.includes(appointment.id)" class="mr-2 inline-block h-3 w-3 rounded-full bg-[#0E4D92] align-middle" title="Trong 1 tuần"></span>
                  {{ appointment.doctorName }}
                </p>
                <p class="text-xs text-slate-400">{{ appointment.specialty }}</p>
              </div>

              <div class="text-right">
                <span :class="['rounded-full px-2.5 py-1 text-xs font-medium', statusBadgeClass(appointment.status)]">
                  {{ formatStatusLabel(appointment.status) }}
                </span>
                <p v-if="appointment.queueNumber" class="mt-1 text-xs font-bold text-[#0E4D92]">{{ appointment.queueNumber }}</p>
              </div>
            </div>

            <div class="mt-3 flex items-center justify-between">
              <button v-if="['Pending', 'Confirmed'].includes(appointment.status)" class="text-xs font-medium text-red-500" :disabled="cancelingAppointmentId === Number(appointment.id)" @click="() => handleCancelAppointment(Number(appointment.id))">
                <span v-if="cancelingAppointmentId === Number(appointment.id)">Đang huỷ...</span>
                <span v-else>Hủy lịch</span>
              </button>
            </div>
          </div>
        </div>
      </section>

      <section class="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
        <div class="mb-4 flex items-center justify-between">
          <h2 class="font-semibold text-slate-700">Trạng thái hàng đợi</h2>
          <div>
            <button v-if="patient.queueError" @click="retryQueue" class="rounded border border-red-100 bg-red-50 px-2 py-1 text-xs text-red-600">Thử lại</button>
          </div>
        </div>

        <div v-if="patient.queueLoading && initialLoading" class="py-6 text-center text-sm text-slate-400">Đang tải hàng đợi...</div>
        <div v-else-if="patient.queueError" class="rounded-xl border border-red-200 bg-red-50 p-6 text-center text-sm text-red-600">{{ patient.queueError }}</div>
        <div v-else-if="patient.queue?.myTicket" class="flex items-center gap-5">
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

      <section class="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
        <div class="mb-4 flex items-center justify-between">
          <h2 class="font-semibold text-slate-700">Lịch khám gần đây</h2>
          <div class="flex items-center gap-3">
            <RouterLink to="/patient/history" class="text-xs font-medium text-[#0E4D92]">Xem tất cả</RouterLink>
            <button v-if="patient.historyError" @click="retryHistory" class="rounded border border-red-100 bg-red-50 px-2 py-1 text-xs text-red-600">Thử lại</button>
          </div>
        </div>

        <div v-if="patient.historyLoading && initialLoading" class="py-6 text-center text-sm text-slate-400">Đang tải lịch sử khám...</div>
        <div v-else-if="patient.historyError" class="rounded-xl border border-red-200 bg-red-50 p-6 text-center text-sm text-red-600">{{ patient.historyError }}</div>

        <template v-else>
          <div v-if="!patient.history || patient.history.length === 0" class="rounded-2xl border border-slate-200 bg-white p-6 text-center text-sm text-slate-400">Chưa có lịch sử khám.</div>

          <article v-else v-for="record in patient.history?.slice(0, 3)" :key="record.id" class="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
            <div class="mb-3 flex items-start justify-between gap-3">
              <div>
                <p class="text-xs text-slate-400">{{ record.examinationDate }}</p>
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
