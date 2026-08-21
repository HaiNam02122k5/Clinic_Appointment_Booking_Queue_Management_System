<script setup lang="ts">
import { ref } from 'vue'

import CheckInResult from '@/features/receptionist/components/CheckInResult.vue'
import { http } from '@/lib/api/http'

import type { AppointmentPatient } from '@/features/receptionist/receptionist.mock'

type AppointmentDetailResponse = {
  id: string
  patientId: string
  doctorId: string
  patientName: string
  doctorName: string
  timeSlot?: string | { hours?: number; minutes?: number; seconds?: number }
  date?: string
  status?: string
  reason?: string
}

type PatientDetailResponse = {
  id?: string
  fullName?: string
  phoneNumber?: string
  PhoneNumber?: string
}

function unwrapApiResult<T>(payload: unknown): T | null {
  if (!payload || typeof payload !== 'object') {
    return payload as T | null
  }

  const maybeEnvelope = payload as { result?: T; data?: T }
  if (maybeEnvelope.result !== undefined) {
    return maybeEnvelope.result
  }

  if (maybeEnvelope.data !== undefined) {
    return maybeEnvelope.data
  }

  return payload as T
}

const searchValue = ref('')
const patient = ref<AppointmentPatient | null>(null)
const errorMessage = ref('')
const queueNumber = ref<string | null>(null)
const isLoading = ref(false)
const selectedDoctorId = ref<string | null>(null)

function formatTime(value: unknown): string {
  if (!value) {
    return ''
  }

  if (typeof value === 'string') {
    const text = value.trim()
    if (!text) {
      return ''
    }

    if (text.includes(':')) {
      return text.slice(0, 5)
    }

    return text
  }

  if (typeof value === 'object' && value && 'hours' in value) {
    const nextValue = value as { hours?: number; minutes?: number }
    const hours = String(nextValue.hours ?? 0).padStart(2, '0')
    const minutes = String(nextValue.minutes ?? 0).padStart(2, '0')

    return `${hours}:${minutes}`
  }

  return String(value)
}

function formatQueueNumber(value: number | string | null | undefined): string {
  const number = Number(value)

  if (Number.isNaN(number)) {
    return 'A-001'
  }

  return `A-${String(number).padStart(3, '0')}`
}

async function resolvePhoneNumber(patientId?: string): Promise<string> {
  if (!patientId) {
    return ''
  }

  try {
    const { data } = await http.get<PatientDetailResponse | { result?: PatientDetailResponse }>(`/patients/${patientId}`)
    const patient = unwrapApiResult<PatientDetailResponse | null>(data)

    return patient?.phoneNumber ?? patient?.PhoneNumber ?? ''
  } catch {
    return ''
  }
}

async function searchPatient() {
  const keyword = searchValue.value.trim()

  errorMessage.value = ''
  patient.value = null
  queueNumber.value = null
  selectedDoctorId.value = null

  if (!keyword) {
    errorMessage.value = 'Vui lòng nhập mã lịch hẹn.'
    return
  }

  isLoading.value = true

  try {
    const appointmentId = keyword
    const { data } = await http.get<AppointmentDetailResponse | { result?: AppointmentDetailResponse }>(`/appointments/${appointmentId}`)
    const appointment = unwrapApiResult<AppointmentDetailResponse | null>(data)

    if (!appointment) {
      errorMessage.value = 'Không tìm thấy lịch hẹn phù hợp.'
      return
    }

    const status = String(appointment.status ?? '').trim().toLowerCase().replace(/[_\s-]+/g, '')

    if (['checkedin', 'completed', 'cancelled', 'canceled', 'noshow', 'finished'].includes(status)) {
      errorMessage.value = 'Bệnh nhân này đã check-in.'
      return
    }

    const phone = await resolvePhoneNumber(appointment.patientId)

    patient.value = {
      appointmentId: appointment.id,
      phone,
      name: appointment.patientName,
      doctor: appointment.doctorName,
      appointmentTime: formatTime(appointment.timeSlot),
      specialty: appointment.reason || 'Khám',
      checkedIn: status === 'checkedin',
      doctorId: appointment.doctorId,
    } as AppointmentPatient & { doctorId?: string }

    selectedDoctorId.value = appointment.doctorId
  } catch (error: unknown) {
    const status = typeof error === 'object' && error !== null && 'response' in error
      ? Number((error as { response?: { status?: number } }).response?.status)
      : undefined

    if (status === 404) {
      errorMessage.value = 'Không tìm thấy lịch hẹn phù hợp.'
      return
    }

    if (status === 401 || status === 403) {
      errorMessage.value = 'Bạn không có quyền truy cập lịch hẹn này.'
      return
    }

    errorMessage.value = 'Không thể tra cứu lịch hẹn. Vui lòng thử lại.'
  } finally {
    isLoading.value = false
  }
}

async function checkIn() {
  if (!patient.value || !selectedDoctorId.value) {
    errorMessage.value = 'Không thể check-in bệnh nhân này.'
    return
  }

  isLoading.value = true

  try {
    await http.post(`/appointments/${patient.value.appointmentId}/check-in`)

    const { data } = await http.get<Array<{ appointmentId: string; queueNumber: number }> | { result?: Array<{ appointmentId: string; queueNumber: number }> }>(
      `/doctors/${selectedDoctorId.value}/queue`,
    )

    const queueList = unwrapApiResult<Array<{ appointmentId: string; queueNumber: number }> | null>(data) ?? []
    const matchedTicket = queueList.find((item) => item.appointmentId === patient.value!.appointmentId)

    queueNumber.value = matchedTicket
      ? formatQueueNumber(matchedTicket.queueNumber)
      : 'A-001'

    patient.value = {
      ...patient.value,
      checkedIn: true,
    }
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
  <div class="space-y-5 max-w-2xl">

    <!-- TITLE -->
    <div>
      <h1 class="text-xl font-semibold text-slate-800">
        Check-in bệnh nhân
      </h1>

      <p class="mt-1 text-sm text-slate-500">
        Tra cứu lịch hẹn và xác nhận bệnh nhân đến khám
      </p>
    </div>

    <!-- SEARCH CARD -->
    <div
      class="bg-white border border-slate-200
             rounded-xl p-6"
    >

      <h2
        class="text-sm font-semibold
               text-slate-800 mb-4"
      >
        Tra cứu lịch hẹn
      </h2>

      <div>

        <label
          class="block text-xs font-medium
                 text-slate-600 mb-1.5"
        >
          Mã lịch hẹn
        </label>

          <input
            v-model="searchValue"
            type="text"
            placeholder="Ví dụ: 9d3a6c52-..."
            class="w-full px-3 py-2.5
                  text-sm text-slate-900
                  border border-slate-200
                  rounded-lg
                  focus:outline-none
                  focus:border-violet-600
                  focus:ring-1
                  focus:ring-violet-600"
            @keyup.enter="searchPatient"
          />

      </div>

      <!-- ERROR -->
      <div
        v-if="errorMessage"
        class="mt-3 px-3 py-2
               bg-red-50 border border-red-200
               rounded-lg text-sm text-red-600"
      >
        {{ errorMessage }}
      </div>

      <button
        type="button"
        class="mt-4 w-full py-2.5
               bg-violet-600 text-white
               text-sm font-semibold
               rounded-lg
               hover:bg-violet-700
               transition-colors
               disabled:cursor-not-allowed
               disabled:bg-violet-400"
        :disabled="isLoading"
        @click="searchPatient"
      >
        {{ isLoading ? 'Đang tra cứu...' : 'Tra cứu' }}
      </button>

    </div>

    <!-- PATIENT INFORMATION -->
    <div
      v-if="patient && !queueNumber"
      class="bg-white border border-slate-200
             rounded-xl p-6"
    >

      <h2
        class="text-sm font-semibold
               text-slate-800 mb-4"
      >
        Thông tin bệnh nhân
      </h2>

      <div class="space-y-3">

        <div
          class="flex justify-between
                 py-2 border-b border-slate-100"
        >
          <span class="text-sm text-slate-500">
            Họ tên
          </span>

          <span class="text-sm font-medium text-slate-800">
            {{ patient.name }}
          </span>
        </div>

        <div
          class="flex justify-between
                 py-2 border-b border-slate-100"
        >
          <span class="text-sm text-slate-500">
            Mã lịch hẹn
          </span>

          <span class="text-sm font-medium text-slate-800">
            {{ patient.appointmentId }}
          </span>
        </div>

        <div
          class="flex justify-between
                 py-2 border-b border-slate-100"
        >
          <span class="text-sm text-slate-500">
            Số điện thoại
          </span>

          <span class="text-sm font-medium text-slate-800">
            {{ patient.phone }}
          </span>
        </div>

        <div
          class="flex justify-between
                 py-2 border-b border-slate-100"
        >
          <span class="text-sm text-slate-500">
            Bác sĩ
          </span>

          <span class="text-sm font-medium text-slate-800">
            {{ patient.doctor }}
          </span>
        </div>

        <div
          class="flex justify-between
                 py-2 border-b border-slate-100"
        >
          <span class="text-sm text-slate-500">
            Chuyên khoa
          </span>

          <span class="text-sm font-medium text-slate-800">
            {{ patient.specialty }}
          </span>
        </div>

        <div class="flex justify-between py-2">

          <span class="text-sm text-slate-500">
            Giờ hẹn
          </span>

          <span class="text-sm font-medium text-slate-800">
            {{ patient.appointmentTime }}
          </span>

        </div>

      </div>

      <!-- CHECK IN -->
      <button
        type="button"
        class="mt-5 w-full py-2.5
               bg-violet-600 text-white
               text-sm font-semibold
               rounded-lg
               hover:bg-violet-700
               transition-colors
               disabled:cursor-not-allowed
               disabled:bg-violet-400"
        :disabled="isLoading"
        @click="checkIn"
      >
        {{ isLoading ? 'Đang xử lý...' : 'Xác nhận check-in' }}
      </button>

    </div>

    <!-- RESULT -->
    <CheckInResult
      v-if="queueNumber && patient"
      :queue-number="queueNumber"
      :patient-name="patient.name"
    />

  </div>
</template>
