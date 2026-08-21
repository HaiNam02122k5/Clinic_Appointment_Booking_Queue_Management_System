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
const cancelingAppointmentId = ref<string | number | null>(null)
const appointmentFilter = ref<'all' | 'upcoming' | 'confirmed' | 'cancelled' | 'pending'>('all')

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

function normalizeStatus(value: unknown): string {
  if (value == null) return ''
  return String(value)
    .toLowerCase()
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')
    .replace(/[_-]+/g, ' ')
    .replace(/\s+/g, ' ')
    .trim()
}

function isStatusConfirmed(raw: unknown): boolean {
  const s = normalizeStatus(raw)
  return (
    s === '1' ||
    s === 'confirmed' ||
    s.includes('confirm') ||
    s.includes('xac') ||
    s.includes('approved') ||
    s.includes('duyet')
  )
}

function isStatusPending(raw: unknown): boolean {
  const s = normalizeStatus(raw)
  return (
    s === '0' ||
    s === 'pending' ||
    s.includes('pend') ||
    s.includes('wait') ||
    s.includes('cho') ||
    s.includes('dang cho')
  )
}

function isStatusCancelled(raw: unknown): boolean {
  const s = normalizeStatus(raw)
  return (
    s === '4' ||
    s === '5' ||
    s.includes('cancel') ||
    s.includes('huy') ||
    s === 'cancelled' ||
    s === 'canceled'
  )
}

function parseDate(value: unknown): Date | null {
  if (!value && value !== 0) return null
  const s = String(value).trim()
  if (!s) return null

  const dm = /^([0-3]?\d)\/(0?[1-9]|1[0-2])\/(\d{4})$/.exec(s)
  if (dm) {
    const local = new Date(Number(dm[3]), Number(dm[2]) - 1, Number(dm[1]))
    return Number.isNaN(local.getTime()) ? null : local
  }

  const iso = /^(\d{4})-(\d{1,2})-(\d{1,2})$/.exec(s)
  if (iso) {
    const local = new Date(Number(iso[1]), Number(iso[2]) - 1, Number(iso[3]))
    return Number.isNaN(local.getTime()) ? null : local
  }

  const parsed = new Date(s)
  return Number.isNaN(parsed.getTime()) ? null : parsed
}

function inNext7Days(date: Date): boolean {
  const start = new Date()
  start.setHours(0, 0, 0, 0)
  const end = new Date(start)
  end.setDate(end.getDate() + 6)

  const candidate = new Date(date)
  candidate.setHours(0, 0, 0, 0)
  return candidate >= start && candidate <= end
}

const fallbackAppointments = computed(() => {
  if (Array.isArray(patient.appointments)) return [...patient.appointments]
  if (Array.isArray((patient as any).upcomingAppointments)) return [...(patient as any).upcomingAppointments]
  return []
})

const displayedAppointments = computed(() => {
  const list = Array.isArray(patient.sortedAppointments)
    ? [...patient.sortedAppointments]
    : fallbackAppointments.value

  return list
    .filter((appointment: any) => {
      const status = appointment?.status
      const date = parseDate(appointment?.appointmentDate)

      switch (appointmentFilter.value) {
        case 'upcoming':
          return date !== null && inNext7Days(date) && (isStatusPending(status) || isStatusConfirmed(status))
        case 'confirmed':
          return isStatusConfirmed(status)
        case 'pending':
          return isStatusPending(status)
        case 'cancelled':
          return isStatusCancelled(status)
        case 'all':
        default:
          return true
      }
    })
    .sort((a: any, b: any) => {
      const da = parseDate(a?.appointmentDate)
      const db = parseDate(b?.appointmentDate)
      if (da && db) return da.getTime() - db.getTime()
      if (da && !db) return -1
      if (!da && db) return 1
      return 0
    })
})

function setFilter(f: typeof appointmentFilter.value) {
  appointmentFilter.value = f
}

function formatDate(dateStr?: string): string {
  if (!dateStr) return ''
  const parsed = parseDate(dateStr)
  if (!parsed) return String(dateStr)
  const day = String(parsed.getDate()).padStart(2, '0')
  const month = String(parsed.getMonth() + 1).padStart(2, '0')
  const year = parsed.getFullYear()
  return `${day}/${month}/${year}`
}

function formatTime(timeStr?: string): string {
  if (!timeStr) return '--:--'
  const text = String(timeStr).trim()
  if (!text) return '--:--'
  const match = text.match(/^(\d{1,2}:\d{2})(?::\d{2})?/)
  return match?.[1] ?? text.slice(0, 5)
}

function statusBadgeClass(raw: unknown): string {
  const s = normalizeStatus(raw)
  if (isStatusConfirmed(raw)) return 'bg-emerald-50 text-emerald-700 border border-emerald-100'
  if (isStatusPending(raw)) return 'bg-amber-50 text-amber-700 border border-amber-100'
  if (isStatusCancelled(raw)) return 'bg-red-50 text-red-600 border border-red-100'
  if (s.includes('check') || s.includes('den')) return 'bg-blue-50 text-blue-700 border border-blue-100'
  if (s.includes('complete') || s.includes('hoan')) return 'bg-slate-50 text-slate-700 border border-slate-100'
  return 'bg-slate-50 text-slate-700 border border-slate-100'
}

function statusLabel(raw: unknown): string {
  const s = normalizeStatus(raw)
  if (isStatusConfirmed(raw)) return 'Đã xác nhận'
  if (isStatusPending(raw)) return 'Chờ duyệt'
  if (isStatusCancelled(raw)) return 'Đã hủy'
  if (s.includes('check') || s.includes('den')) return 'Đã đến'
  if (s.includes('complete') || s.includes('hoan')) return 'Hoàn thành'
  return String(raw ?? '') || 'Không rõ'
}

function appointmentHighlightClass(appointment: any): string {
  const counted = Array.isArray(patient.upcomingWeekIds)
    && patient.upcomingWeekIds.some((id) => String(id) === String(appointment?.id))
    && !isStatusCancelled(appointment?.status)

  return counted
    ? 'rounded-xl border-2 border-blue-200 bg-[#f3f9ff] p-4 mb-3 shadow-sm'
    : 'rounded-xl border border-slate-200 bg-white p-4 mb-3'
}

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
  } catch {}

  try {
    await Promise.allSettled(tasks)
  } finally {
    initialLoading.value = false
  }
})

async function retryAppointments() { await patient.loadAppointments() }
async function retryQueue() { await patient.loadQueue() }
async function retryHistory() { await patient.loadHistory() }

async function handleCancelAppointment(id: string | number) {
  let confirmed = true
  if (typeof window !== 'undefined' && typeof window.confirm === 'function') {
    try {
      confirmed = window.confirm('Bạn có chắc chắn muốn hủy lịch khám này?')
    } catch {
      confirmed = true
    }
  }

  if (confirmed === false) return

  cancelingAppointmentId.value = id
  try {
    await patient.cancelAppointment(id)
  } finally {
    cancelingAppointmentId.value = null
  }
}

watch(
  () => fallbackAppointments.value.length,
  () => {
    if (appointmentFilter.value === 'upcoming' && (patient.upcomingWeekCount ?? 0) === 0) {
      appointmentFilter.value = 'all'
    }
  },
)
</script>

3<template>
  <div class="mx-auto max-w-3xl space-y-5">
    <div v-if="connecting" class="rounded-xl border border-blue-200 bg-blue-50 px-4 py-3 text-sm text-blue-800 mb-4">
      {{ initialLoading ? 'Đang kết nối tới dịch vụ bệnh nhân...' : 'Đang đồng bộ dữ liệu...' }}
    </div>

    <div v-else-if="!connecting && hasAnyError" class="rounded-xl border border-amber-200 bg-amber-50 px-4 py-3 text-sm text-amber-800 mb-4">
      Một hoặc nhiều dịch vụ đang gặp sự cố — xem chi tiết bên dưới và thử lại từng mục.
    </div>

    <main class="flex-1 space-y-5">
      <!-- HEADER CARD -->
      <section class="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm mb-2">
        <div class="flex items-center justify-between gap-4">
          <div class="flex items-center gap-3 min-w-0">
            <div class="h-12 w-12 rounded-full bg-[#6b46c1] flex items-center justify-center text-white font-bold text-lg">
              {{ (patient.profile?.fullName ?? auth.user?.name ?? 'B').slice(0, 1) }}
            </div>
            <div class="min-w-0">
              <p class="text-sm font-semibold text-slate-800 truncate">
                {{ patient.profile?.fullName ?? auth.user?.name ?? 'Bệnh nhân' }}
              </p>
              <p class="text-xs text-slate-400 truncate">
                {{ patient.profile?.email ?? auth.user?.email ?? '' }}
              </p>
            </div>
          </div>
          <div class="flex items-center gap-3 flex-wrap justify-end">
            <div class="inline-flex items-center gap-2 rounded-full border border-slate-200 bg-slate-50 px-3 py-1.5 text-sm text-slate-700 shadow-sm">
              <span class="font-medium">Lịch sắp tới (1 tuần):</span>
              <span class="min-w-5 text-center font-semibold text-[#0E4D92]">{{ patient.upcomingWeekCount ?? 0 }}</span>
            </div>
            <RouterLink to="/patient/booking" class="inline-flex items-center justify-center rounded-md bg-[#0E4D92] px-3 py-1.5 text-sm font-medium text-white shadow-sm transition hover:bg-[#0c3d79]">Đặt khám</RouterLink>
          </div>
        </div>
      </section>

      <!-- BANNER -->
      <section class="flex items-center justify-between rounded-2xl bg-gradient-to-r from-[#0E4D92] to-[#1a6bbf] p-5 text-white">
        <div>
          <p class="text-sm text-blue-200">Xin chào,</p>
          <h1 class="text-2xl font-bold">{{ patient.profile?.fullName || auth.user?.name || 'Bệnh nhân' }} 👋</h1>
          <p class="mt-1 text-xs text-blue-200">{{ today }}</p>
        </div>
        <div class="flex h-16 w-16 items-center justify-center rounded-2xl bg-white/10 text-3xl">🧑‍⚕️</div>
      </section>

      <!-- ACTION BUTTONS -->
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

      <!-- UPCOMING APPOINTMENTS SECTION -->
      <section class="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
        <div class="mb-4 flex items-center justify-between">
          <h2 class="font-semibold text-slate-700">Lịch hẹn</h2>
          <div class="flex items-center gap-3">
            <RouterLink to="/patient/booking" class="text-xs font-medium text-[#0E4D92]">+ Đặt thêm</RouterLink>
            <button v-if="patient.appointmentsError" @click="retryAppointments" class="text-xs rounded px-2 py-1 bg-red-50 text-red-600 border border-red-100">Thử lại</button>
          </div>
        </div>

        <!-- Filter controls -->
        <div class="mb-4 flex flex-wrap items-center gap-2">
          <button :class="['text-xs px-3 py-1 rounded transition-colors', appointmentFilter === 'all' ? 'bg-[#0E4D92] text-white' : 'bg-white border text-slate-600']" @click.prevent="setFilter('all')">Tất cả</button>
          <button :class="['text-xs px-3 py-1 rounded transition-colors', appointmentFilter === 'upcoming' ? 'bg-[#0E4D92] text-white' : 'bg-white border text-slate-600']" @click.prevent="setFilter('upcoming')">Sắp tới</button>
          <button :class="['text-xs px-3 py-1 rounded transition-colors', appointmentFilter === 'confirmed' ? 'bg-[#0E4D92] text-white' : 'bg-white border text-slate-600']" @click.prevent="setFilter('confirmed')">Đã xác nhận</button>
          <button :class="['text-xs px-3 py-1 rounded transition-colors', appointmentFilter === 'pending' ? 'bg-[#0E4D92] text-white' : 'bg-white border text-slate-600']" @click.prevent="setFilter('pending')">Chờ duyệt</button>
          <button :class="['text-xs px-3 py-1 rounded transition-colors', appointmentFilter === 'cancelled' ? 'bg-[#0E4D92] text-white' : 'bg-white border text-slate-600']" @click.prevent="setFilter('cancelled')">Đã hủy</button>
        </div>

        <!-- Appointments List -->
        <div v-if="patient.appointmentsLoading && initialLoading" class="rounded-xl bg-slate-50 p-4 text-sm text-slate-500">Đang tải lịch hẹn...</div>

        <div v-else-if="patient.appointmentsError" class="rounded-xl bg-red-50 p-4 text-sm text-red-600">{{ patient.appointmentsError }}</div>

        <div v-else-if="displayedAppointments.length > 0">
          <div v-for="appointment in displayedAppointments" :key="appointment.id" :class="appointmentHighlightClass(appointment)">
            <div class="flex items-center gap-4">
              <div class="w-16 text-center">
                <p class="text-xs font-semibold text-blue-600">{{ formatDate(appointment.appointmentDate) }}</p>
                <p class="text-lg font-bold text-[#0E4D92]">{{ formatTime(appointment.appointmentTime) }}</p>
              </div>

              <div class="h-10 w-px bg-blue-200" />

              <div class="min-w-0 flex-1">
                <p class="font-semibold text-slate-800">
                  <span v-if="Array.isArray(patient.upcomingWeekIds) && patient.upcomingWeekIds.some((id) => String(id) === String(appointment.id)) && !isStatusCancelled(appointment.status)" class="inline-block w-2.5 h-2.5 rounded-full bg-[#0E4D92] mr-1.5 align-middle" title="Trong 1 tuần"></span>
                  {{ appointment.doctorName || 'Bác sĩ' }}
                </p>
                <p class="text-xs text-slate-500 truncate" :title="appointment.specialty || 'Khác'">{{ appointment.specialty || 'Khác' }}</p>
              </div>

              <div class="text-right">
                <span class="rounded-full px-2.5 py-1 text-xs font-medium" :class="statusBadgeClass(appointment.status)">
                  {{ statusLabel(appointment.status) }}
                </span>
              </div>
            </div>

            <div class="mt-3 flex items-center justify-between border-t border-slate-100 pt-2">
              <button
                v-if="['pending', 'confirmed'].includes(normalizeStatus(appointment.status))"
                class="text-xs font-medium text-red-500 hover:text-red-700 disabled:cursor-not-allowed disabled:opacity-50"
                :disabled="String(cancelingAppointmentId) === String(appointment.id)"
                @click="handleCancelAppointment(appointment.id)"
              >
                {{ String(cancelingAppointmentId) === String(appointment.id) ? 'Đang hủy...' : 'Hủy lịch' }}
              </button>
              <span v-else></span>

              <span v-if="Array.isArray(patient.upcomingWeekIds) && patient.upcomingWeekIds.some((id) => String(id) === String(appointment.id)) && !isStatusCancelled(appointment.status)" class="text-xs rounded-full bg-[#f0f9ff] border border-blue-100 px-2 py-0.5 text-blue-700">
                Trong 1 tuần
              </span>
            </div>
          </div>
        </div>

        <div v-else class="rounded-xl bg-slate-50 p-6 text-center text-sm text-slate-500">
          Không tìm thấy lịch hẹn phù hợp.
        </div>
      </section>
    </main>
  </div>
</template>