import { computed, ref } from 'vue'
import { defineStore } from 'pinia'

import { http } from '@/lib/api/http'
import { useAuthStore } from '@/stores/auth'
import {
  mockAppointments,
  queueData,
  type AppointmentPatient,
  type QueuePatient,
} from '@/features/receptionist/receptionist.mock'

type QueueApiItem = {
  id: string
  appointmentId: string
  queueNumber: number
  priority: boolean
  status: string
  checkInTime: string
  calledAt?: string | null
  patientName?: string | null
  doctorName?: string | null
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

function normalizeStatusText(value?: string | null): string {
  return String(value ?? '')
    .trim()
    .toLowerCase()
    .replace(/[_\s-]+/g, '')
}

function mapQueueStatus(status: string): QueuePatient['status'] {
  const normalized = normalizeStatusText(status)

  switch (normalized) {
    case 'waiting':
    case 'pending':
    case 'queued':
      return 'waiting'
    case 'called':
    case 'inprogress':
    case 'examining':
    case 'beingexamined':
      return 'examining'
    case 'completed':
    case 'done':
    case 'finished':
      return 'completed'
    case 'skipped':
    case 'cancelled':
    case 'canceled':
    default:
      return 'waiting'
  }
}

function formatQueueNumber(number: number): string {
  return `A-${String(number).padStart(3, '0')}`
}

function formatApiTime(value?: string | null): string {
  if (!value) return ''

  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value

  return date.toLocaleTimeString('en-GB', {
    hour: '2-digit',
    minute: '2-digit',
    hour12: false,
  })
}

function toQueuePatient(item: QueueApiItem): QueuePatient {
  return {
    id: item.id,
    appointmentId: item.appointmentId,
    no: formatQueueNumber(item.queueNumber),
    name: item.patientName ?? 'Bệnh nhân',
    doctor: item.doctorName ?? 'Bác sĩ',
    time: formatApiTime(item.checkInTime),
    status: mapQueueStatus(item.status),
  }
}

export const useReceptionistStore = defineStore(
  'receptionist',
  () => {
    const authStore = useAuthStore()

    // ================================
    // STATE
    // ================================

    const appointments = ref<AppointmentPatient[]>(
      mockAppointments.map((appointment) => ({
        ...appointment,
      }))
    )

    const queue = ref<QueuePatient[]>(
      queueData.map((patient) => ({
        ...patient,
      }))
    )

    const queueError = ref<string | null>(null)
    const queueLoading = ref(false)

    // ================================
    // GETTERS
    // ================================

    const waitingCount = computed(() => {
      return queue.value.filter(
        (patient) => patient.status === 'waiting'
      ).length
    })

    const examiningCount = computed(() => {
      return queue.value.filter(
        (patient) => patient.status === 'examining'
      ).length
    })

    const completedCount = computed(() => {
      return queue.value.filter(
        (patient) => patient.status === 'completed'
      ).length
    })

    // ================================
    // SEARCH APPOINTMENT
    // ================================

    async function resolveQueueDoctorId(doctorId?: string): Promise<string | undefined> {
      if (doctorId) {
        return doctorId
      }

      const activeRole = normalizeStatusText(authStore.user?.activeRole ?? authStore.user?.role)
      if (activeRole === 'doctor') {
        try {
          const { data } = await http.get<{ id?: string | number } | null>('/doctors/me')
          const payload = unwrapApiResult<{ id?: string | number } | null>(data)
          if (payload?.id != null) {
            return String(payload.id)
          }
        } catch {
          // fall through to doctor list lookup below
        }
      }

      try {
        const doctorsRes = await http.get<{ items?: Array<{ id?: string | number }> }>('/doctors', {
          params: { pageNumber: 1, pageSize: 20 },
        })

        const doctorList = unwrapApiResult<{ items?: Array<{ id?: string | number }> } | null>(doctorsRes.data)?.items ?? []
        const firstDoctor = doctorList.find((item) => item.id != null)
        return firstDoctor ? String(firstDoctor.id) : undefined
      } catch {
        return undefined
      }
    }

    async function fetchQueue(doctorId?: string) {
      queueLoading.value = true
      queueError.value = null

      try {
        const targetDoctorId = await resolveQueueDoctorId(doctorId)

        if (!targetDoctorId) {
          queue.value = queueData.map((patient) => ({ ...patient }))
          return
        }

        const { data } = await http.get<QueueApiItem[] | { items?: QueueApiItem[]; data?: QueueApiItem[]; result?: QueueApiItem[] }>(`/doctors/${targetDoctorId}/queue`)
        const rawItems = unwrapApiResult<QueueApiItem[] | { items?: QueueApiItem[]; data?: QueueApiItem[] } | null>(data)
        const queueList = Array.isArray(rawItems)
          ? rawItems
          : Array.isArray((rawItems as { items?: QueueApiItem[] } | null)?.items)
            ? (rawItems as { items?: QueueApiItem[] }).items ?? []
            : Array.isArray((rawItems as { data?: QueueApiItem[] } | null)?.data)
              ? (rawItems as { data?: QueueApiItem[] }).data ?? []
              : []

        const nextQueue = queueList
          .map((item) => toQueuePatient(item as QueueApiItem))
          .sort((a, b) => {
            const numberA = Number(a.no.replace(/\D/g, '')) || 0
            const numberB = Number(b.no.replace(/\D/g, '')) || 0
            return numberA - numberB
          })

        queue.value = nextQueue
      } catch (error: unknown) {
        const status = typeof error === 'object' && error !== null && 'response' in error
          ? Number((error as { response?: { status?: number } }).response?.status)
          : 0

        if (status === 401 || status === 403) {
          queueError.value = 'Bạn không có quyền xem hàng đợi.'
        } else {
          queueError.value = 'Không thể tải hàng đợi từ máy chủ.'
        }

        queue.value = queueData.map((patient) => ({ ...patient }))
      } finally {
        queueLoading.value = false
      }
    }

    function findAppointment(
      keyword: string
    ): AppointmentPatient | null {
      const value = keyword.trim().toLowerCase()

      if (!value) {
        return null
      }

      return (
        appointments.value.find(
          (appointment) =>
            appointment.appointmentId.toLowerCase() === value ||
            appointment.phone === value
        ) ?? null
      )
    }

    // ================================
    // GENERATE QUEUE NUMBER
    // ================================

    function generateQueueNumber(): string {
      const numbers = queue.value
        .map((patient) => {
          const match = patient.no.match(/^A-(\d+)$/)

          return match ? Number(match[1]) : 0
        })
        .filter((number) => number > 0)

      const maxNumber = numbers.length
        ? Math.max(...numbers)
        : 0

      return `A-${String(maxNumber + 1).padStart(3, '0')}`
    }

    // ================================
    // CHECK-IN
    // ================================

    function checkIn(
      appointmentId: string
    ): QueuePatient | null {
      const appointment = appointments.value.find(
        (item) => item.appointmentId === appointmentId
      )

      if (!appointment) {
        return null
      }

      // Không cho check-in 2 lần
      if (appointment.checkedIn) {
        return null
      }

      const queueNumber = generateQueueNumber()

      // Đánh dấu lịch hẹn đã check-in
      appointment.checkedIn = true

      // Tạo bệnh nhân trong queue
      const newPatient: QueuePatient = {
        no: queueNumber,
        name: appointment.name,
        doctor: appointment.doctor,
        time: appointment.appointmentTime,
        status: 'waiting',
      }

      // Thêm vào hàng đợi
      queue.value.push(newPatient)

      return newPatient
    }

    async function callQueueTicket(queueTicketId: string) {
      await http.post(`/queue/${queueTicketId}/start-exam`)
      await fetchQueue()
    }

    async function completeQueueTicket(queueTicketId: string) {
      await http.post(`/queue/${queueTicketId}/complete-exam`)
      await fetchQueue()
    }

    // ================================
    // CALL PATIENT
    // ================================

    async function callPatient(no: string) {
      const patient = queue.value.find(
        (item) => item.no === no
      )

      if (!patient) {
        return
      }

      if (patient.status !== 'waiting') {
        return
      }

      if (patient.id) {
        await callQueueTicket(patient.id)
        return
      }

      patient.status = 'examining'
    }

    // ================================
    // COMPLETE PATIENT
    // ================================

    async function completePatient(no: string) {
      const patient = queue.value.find(
        (item) => item.no === no
      )

      if (!patient) {
        return
      }

      if (patient.status !== 'examining') {
        return
      }

      if (patient.id) {
        await completeQueueTicket(patient.id)
        return
      }

      patient.status = 'completed'
    }

    // ================================
    // RESET MOCK DATA
    // ================================

    function resetMockData() {
      queue.value = queueData.map((patient) => ({
        ...patient,
      }))

      appointments.value = mockAppointments.map(
        (appointment) => ({
          ...appointment,
        })
      )
    }

    return {
      appointments,
      queue,
      queueLoading,
      queueError,

      waitingCount,
      examiningCount,
      completedCount,

      fetchQueue,
      findAppointment,
      checkIn,
      callPatient,
      completePatient,

      resetMockData,
    }
  }
)
