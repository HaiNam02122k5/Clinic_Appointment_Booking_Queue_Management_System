<script setup lang="ts">
import { ref } from 'vue'

import CheckInResult from '@/features/receptionist/components/CheckInResult.vue'
import { http } from '@/lib/api/http'

type PatientSearchItem = {
  id: string
  fullName: string
  phoneNumber?: string | null
  email?: string | null
  insuranceNumber?: string | null
}

type AppointmentItem = {
  id: string
  patientId: string
  doctorId: string
  patientName: string
  doctorName: string
  date?: string | null
  timeSlot?: string | { hours?: number; minutes?: number; seconds?: number } | null
  reason?: string | null
  status?: string | number | null
}

type QueueItem = {
  appointmentId?: string | null
  id?: string | null
  queueNumber?: number | string | null
}

function unwrapApiResult<T>(payload: unknown): T | null {
  if (!payload || typeof payload !== 'object') {
    return payload as T | null
  }

  const maybeEnvelope = payload as { result?: T; data?: T; items?: T }
  if (maybeEnvelope.result !== undefined) {
    return maybeEnvelope.result
  }

  if (maybeEnvelope.data !== undefined) {
    return maybeEnvelope.data
  }

  if (maybeEnvelope.items !== undefined) {
    return maybeEnvelope.items
  }

  return payload as T
}

const searchValue = ref('')
const searchResults = ref<PatientSearchItem[]>([])
const selectedPatient = ref<PatientSearchItem | null>(null)
const upcomingAppointments = ref<AppointmentItem[]>([])
const selectedAppointmentId = ref<string | null>(null)
const errorMessage = ref('')
const successMessage = ref('')
const queueNumber = ref<string | null>(null)
const isLoading = ref(false)

function formatDate(value?: string | null): string {
  if (!value) return '—'

  const normalized = String(value).trim()
  if (!normalized) return '—'

  const parsed = new Date(normalized.includes('T') ? normalized : `${normalized}T00:00:00`)
  if (Number.isNaN(parsed.getTime())) return normalized

  return parsed.toLocaleDateString('vi-VN', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  })
}

function formatTime(value: unknown): string {
  if (!value) {
    return '—'
  }

  if (typeof value === 'string') {
    const text = value.trim()
    if (!text) return '—'
    return text.includes(':') ? text.slice(0, 5) : text
  }

  if (typeof value === 'object' && value && 'hours' in value) {
    const nextValue = value as { hours?: number; minutes?: number; seconds?: number }
    const hours = String(nextValue.hours ?? 0).padStart(2, '0')
    const minutes = String(nextValue.minutes ?? 0).padStart(2, '0')
    return `${hours}:${minutes}`
  }

  return String(value)
}

function patientDisplayName(patient: PatientSearchItem): string {
  return patient.fullName || 'Bệnh nhân'
}

function formatQueueNumber(value: number | string | null | undefined): string | null {
  if (value === null || value === undefined || value === '') return null

  const number = Number(value)
  if (Number.isNaN(number)) {
    return null
  }

  return `A-${String(number).padStart(3, '0')}`
}

function appointmentStatusLabel(status: unknown): string {
  if (status === null || status === undefined || status === '') {
    return 'Chưa rõ'
  }

  const normalized = String(status).trim().toLowerCase().replace(/[_\s-]+/g, '')

  if (['pending', '0'].includes(normalized)) return 'Chờ xác nhận'
  if (['confirmed', '1'].includes(normalized)) return 'Đã xác nhận'
  if (['checkedin', '2'].includes(normalized)) return 'Đã check-in'
  if (['completed', '3'].includes(normalized)) return 'Hoàn thành'
  if (['cancelled', 'canceled', '4'].includes(normalized)) return 'Đã hủy'
  if (['noshow', '5'].includes(normalized)) return 'Không đến'

  return String(status)
}

async function searchPatient() {
  const keyword = searchValue.value.trim()

  errorMessage.value = ''
  successMessage.value = ''
  queueNumber.value = null
  searchResults.value = []
  selectedPatient.value = null
  upcomingAppointments.value = []
  selectedAppointmentId.value = null

  if (!keyword) {
    errorMessage.value = 'Vui lòng nhập tên, email, số điện thoại hoặc số bảo hiểm của bệnh nhân.'
    return
  }

  isLoading.value = true

  try {
    const { data } = await http.get('/patients', {
      params: {
        search: keyword,
        pageNumber: 1,
        pageSize: 10,
        sortBy: 'fullName',
        orderBy: 'asc',
      },
    })

    const payload = unwrapApiResult<{ items?: PatientSearchItem[] } | PatientSearchItem[] | null>(data)
    const patients = Array.isArray(payload)
      ? payload
      : Array.isArray(payload?.items)
        ? payload.items
        : []

    if (!patients.length) {
      errorMessage.value = 'Không tìm thấy bệnh nhân phù hợp.'
      return
    }

    searchResults.value = patients.map((patient) => ({
      id: String(patient.id),
      fullName: patient.fullName ?? 'Bệnh nhân',
      phoneNumber: patient.phoneNumber ?? '',
      email: patient.email ?? '',
      insuranceNumber: patient.insuranceNumber ?? '',
    }))
  } catch (error: unknown) {
    const status = typeof error === 'object' && error !== null && 'response' in error
      ? Number((error as { response?: { status?: number } }).response?.status)
      : undefined

    if (status === 401 || status === 403) {
      errorMessage.value = 'Bạn không có quyền tìm bệnh nhân.'
      return
    }

    errorMessage.value = 'Không thể tìm bệnh nhân. Vui lòng thử lại.'
  } finally {
    isLoading.value = false
  }
}

async function loadPatientAppointments(patientId: string) {
  isLoading.value = true

  try {
    const { data } = await http.get(`/patients/${patientId}/appointments`)
    const payload = unwrapApiResult<AppointmentItem[] | null>(data)
    const appointments = Array.isArray(payload) ? payload : []

    upcomingAppointments.value = appointments
      .filter((appointment) => appointment && appointment.id)
      .sort((a, b) => {
        const dateA = new Date(`${a.date ?? '2000-01-01'}T00:00:00`).getTime()
        const dateB = new Date(`${b.date ?? '2000-01-01'}T00:00:00`).getTime()
        return dateA - dateB
      })

    if (!upcomingAppointments.value.length) {
      errorMessage.value = 'Bệnh nhân chưa có lịch hẹn sắp tới.'
    }
  } catch (error: unknown) {
    const status = typeof error === 'object' && error !== null && 'response' in error
      ? Number((error as { response?: { status?: number } }).response?.status)
      : undefined

    if (status === 401 || status === 403) {
      errorMessage.value = 'Bạn không có quyền xem lịch hẹn của bệnh nhân này.'
      return
    }

    if (status === 404) {
      errorMessage.value = 'Không tìm thấy bệnh nhân hoặc lịch hẹn của bệnh nhân.'
      return
    }

    errorMessage.value = 'Không thể tải lịch hẹn của bệnh nhân.'
  } finally {
    isLoading.value = false
  }
}

function selectPatient(patient: PatientSearchItem) {
  selectedPatient.value = patient
  selectedAppointmentId.value = null
  queueNumber.value = null
  successMessage.value = ''
  errorMessage.value = ''
  void loadPatientAppointments(patient.id)
}

function selectAppointment(appointmentId: string) {
  selectedAppointmentId.value = appointmentId
  errorMessage.value = ''
  successMessage.value = ''
  queueNumber.value = null
}

/**
 * Perform the appropriate action for the selected appointment:
 * - If status is pending => call /appointments/{id}/confirm and update local status to confirmed
 * - If status is confirmed => call /appointments/{id}/check-in and then fetch queue number
 */
async function performAppointmentAction(appointmentId?: string | null) {
  if (!appointmentId) {
    errorMessage.value = 'Vui lòng chọn một cuộc hẹn để thực hiện hành động.'
    return
  }

  const appointment = upcomingAppointments.value.find((item) => item.id === appointmentId)
  if (!appointment) {
    errorMessage.value = 'Không tìm thấy cuộc hẹn đã chọn.'
    return
  }

  // Normalize status to determine action
  const statusText = String(appointment.status ?? '').trim().toLowerCase()
  const isPending = ['pending', '0'].some((s) => statusText.includes(s))
  const isConfirmed = ['confirmed', '1'].some((s) => statusText.includes(s))

  errorMessage.value = ''
  successMessage.value = ''
  isLoading.value = true

  try {
    if (isPending) {
      // Call confirm endpoint
      await http.post(`/appointments/${appointment.id}/confirm`)
      // Update local model so UI shows Confirmed and next action becomes Check-in
      appointment.status = 'confirmed'
      successMessage.value = 'Đã xác nhận cuộc hẹn.'
      return
    }

    if (isConfirmed) {
      // Proceed to check-in
      await http.post(`/appointments/${appointment.id}/check-in`)

      if (appointment.doctorId) {
        try {
          const { data } = await http.get(`/doctors/${appointment.doctorId}/queue`)
          const payload = unwrapApiResult<QueueItem[] | { items?: QueueItem[]; data?: QueueItem[]; result?: QueueItem[] } | null>(data)
          const queueList = Array.isArray(payload)
            ? payload
            : Array.isArray(payload?.items)
              ? payload.items
              : Array.isArray(payload?.data)
                ? payload.data
                : Array.isArray(payload?.result)
                  ? payload.result
                  : []

          const matchedQueue = queueList.find((item) => String(item.appointmentId ?? item.id) === String(appointment.id))
          queueNumber.value = formatQueueNumber(matchedQueue?.queueNumber ?? null)
        } catch {
          queueNumber.value = 'A-001'
        }
      }

      // Update local status to checked-in
      appointment.status = 'checkedin'
      successMessage.value = 'Check-in bệnh nhân thành công.'
      return
    }

    errorMessage.value = 'Cuộc hẹn hiện không thể thực hiện hành động.'
  } catch (error: unknown) {
    const status = typeof error === 'object' && error !== null && 'response' in error
      ? Number((error as { response?: { status?: number } }).response?.status)
      : undefined

    if (status === 400) {
      errorMessage.value = 'Hành động không hợp lệ cho cuộc hẹn này.'
      return
    }

    if (status === 404) {
      errorMessage.value = 'Không tìm thấy cuộc hẹn.'
      return
    }

    if (status === 401 || status === 403) {
      errorMessage.value = 'Bạn không có quyền thực hiện hành động này.'
      return
    }

    errorMessage.value = 'Đã xảy ra lỗi khi thực hiện hành động. Vui lòng thử lại.'
  } finally {
    isLoading.value = false
  }
}

async function checkIn() {
  if (!selectedAppointmentId.value) {
    errorMessage.value = 'Vui lòng chọn một cuộc hẹn để check-in.'
    return
  }

  const appointment = upcomingAppointments.value.find((item) => item.id === selectedAppointmentId.value)
  if (!appointment) {
    errorMessage.value = 'Không tìm thấy cuộc hẹn đã chọn.'
    return
  }

  isLoading.value = true

  try {
    await http.post(`/appointments/${appointment.id}/check-in`)

    if (appointment.doctorId) {
      try {
        const { data } = await http.get(`/doctors/${appointment.doctorId}/queue`)
        const payload = unwrapApiResult<QueueItem[] | { items?: QueueItem[]; data?: QueueItem[]; result?: QueueItem[] } | null>(data)
        const queueList = Array.isArray(payload)
          ? payload
          : Array.isArray(payload?.items)
            ? payload.items
            : Array.isArray(payload?.data)
             ? payload.data
             : Array.isArray(payload?.result)
               ? payload.result
               : []

        const matchedQueue = queueList.find((item) => String(item.appointmentId ?? item.id) === String(appointment.id))
        queueNumber.value = formatQueueNumber(matchedQueue?.queueNumber ?? null)
      } catch {
        queueNumber.value = 'A-001'
      }
    }

    successMessage.value = 'Check-in bệnh nhân thành công.'
    errorMessage.value = ''
  } catch (error: unknown) {
    const status = typeof error === 'object' && error !== null && 'response' in error
      ? Number((error as { response?: { status?: number } }).response?.status)
      : undefined

    if (status === 400) {
      errorMessage.value = 'Lịch hẹn này không thể check-in ở thời điểm hiện tại.'
      return
    }

    if (status === 404) {
      errorMessage.value = 'Không tìm thấy lịch hẹn để check-in.'
      return
    }

    if (status === 401 || status === 403) {
      errorMessage.value = 'Bạn không có quyền check-in bệnh nhân.'
      return
    }

    errorMessage.value = 'Không thể check-in bệnh nhân này.'
  } finally {
    isLoading.value = false
  }
}
</script>

<template>
  <div class="max-w-2xl space-y-5">
    <div>
      <h1 class="text-xl font-semibold text-slate-800">
        Check-in bệnh nhân
      </h1>
      <p class="mt-1 text-sm text-slate-500">
        Tìm bệnh nhân theo tên, email, số điện thoại hoặc số bảo hiểm để check-in.
      </p>
    </div>

    <div class="rounded-xl border border-slate-200 bg-white p-6">
      <h2 class="mb-4 text-sm font-semibold text-slate-800">
        Tìm bệnh nhân
      </h2>

      <label class="mb-1.5 block text-xs font-medium text-slate-600">
        Tên, email, SĐT hoặc số bảo hiểm
      </label>

      <input
        v-model="searchValue"
        type="text"
        placeholder="Nhập thông tin bệnh nhân..."
        class="w-full rounded-lg border border-slate-200 px-3 py-2.5 text-sm text-slate-900 focus:border-violet-600 focus:outline-none focus:ring-1 focus:ring-violet-600"
        @keyup.enter="searchPatient"
      />

      <div
        v-if="errorMessage"
        class="mt-3 rounded-lg border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-600"
      >
        {{ errorMessage }}
      </div>

      <div
        v-if="successMessage"
        class="mt-3 rounded-lg border border-emerald-200 bg-emerald-50 px-3 py-2 text-sm text-emerald-700"
      >
        {{ successMessage }}
      </div>

      <button
        type="button"
        class="mt-4 w-full rounded-lg bg-violet-600 px-3 py-2.5 text-sm font-semibold text-white transition-colors hover:bg-violet-700 disabled:cursor-not-allowed disabled:bg-violet-400"
        :disabled="isLoading"
        @click="searchPatient"
      >
        {{ isLoading ? 'Đang tìm...' : 'Tìm bệnh nhân' }}
      </button>
    </div>

    <div
      v-if="searchResults.length"
      class="rounded-xl border border-slate-200 bg-white p-6"
    >
      <h2 class="mb-4 text-sm font-semibold text-slate-800">
        Chọn bệnh nhân
      </h2>

      <div class="space-y-3">
        <button
          v-for="patient in searchResults"
          :key="patient.id"
          type="button"
          class="w-full rounded-lg border px-4 py-3 text-left transition-colors"
          :class="selectedPatient?.id === patient.id
            ? 'border-violet-500 bg-violet-50'
            : 'border-slate-200 bg-white hover:border-violet-200 hover:bg-violet-50/50'"
          @click="selectPatient(patient)"
        >
          <div class="flex items-center justify-between gap-3">
            <span class="text-sm font-semibold text-slate-800">
             {{ patientDisplayName(patient) }}
            </span>
            <span class="text-xs text-slate-500">{{ patient.insuranceNumber || 'Không có BHYT' }}</span>
          </div>

          <div class="mt-2 space-y-1 text-xs text-slate-600">
            <div v-if="patient.email">Email: {{ patient.email }}</div>
            <div v-if="patient.phoneNumber">SĐT: {{ patient.phoneNumber }}</div>
          </div>
        </button>
      </div>
    </div>

    <div
      v-if="selectedPatient && !upcomingAppointments.length && !isLoading && !errorMessage"
      class="rounded-xl border border-slate-200 bg-white p-6 text-sm text-slate-600"
    >
      Bệnh nhân đã được chọn nhưng chưa có lịch hẹn sắp tới.
    </div>

    <div
      v-if="selectedPatient && upcomingAppointments.length"
      class="rounded-xl border border-slate-200 bg-white p-6"
    >
      <div class="mb-4 flex items-center justify-between gap-3">
        <h2 class="text-sm font-semibold text-slate-800">
          Cuộc hẹn sắp tới của {{ patientDisplayName(selectedPatient) }}
        </h2>
      </div>

      <div class="space-y-3">
        <button
          v-for="appointment in upcomingAppointments"
          :key="appointment.id"
          type="button"
          class="w-full rounded-lg border px-4 py-3 text-left transition-colors"
          :class="selectedAppointmentId === appointment.id
            ? 'border-violet-500 bg-violet-50'
            : 'border-slate-200 bg-white hover:border-violet-200 hover:bg-violet-50/50'"
          @click="selectAppointment(appointment.id)"
        >
          <div class="flex items-center justify-between gap-3">
            <span class="text-sm font-semibold text-slate-800">
             {{ formatDate(appointment.date) }}
            </span>
            <span class="rounded-full bg-violet-100 px-2 py-1 text-[10px] font-medium text-violet-700">
             {{ appointmentStatusLabel(appointment.status) }}
            </span>
          </div>

          <div class="mt-2 space-y-1 text-xs text-slate-600">
            <div>Giờ: {{ formatTime(appointment.timeSlot) }}</div>
            <div>Bác sĩ: {{ appointment.doctorName || 'Chưa xác định' }}</div>
            <div v-if="appointment.reason">Lý do: {{ appointment.reason }}</div>
          </div>
        </button>
      </div>

      <button
        v-if="selectedAppointmentId"
        type="button"
        class="mt-5 w-full rounded-lg bg-violet-600 px-3 py-2.5 text-sm font-semibold text-white transition-colors hover:bg-violet-700 disabled:cursor-not-allowed disabled:bg-violet-400"
        :disabled="isLoading"
        @click="void performAppointmentAction(selectedAppointmentId)"
      >
        {{ isLoading ? 'Đang xử lý...' : (upcomingAppointments.find(a => a.id === selectedAppointmentId)?.status && ['pending','0'].includes(String(upcomingAppointments.find(a => a.id === selectedAppointmentId)?.status).toLowerCase()) ? 'Xác nhận' : 'Check-in') }}
      </button>
    </div>

    <CheckInResult
      v-if="queueNumber && selectedPatient"
      :queue-number="queueNumber"
      :patient-name="patientDisplayName(selectedPatient)"
    />
  </div>
</template>
