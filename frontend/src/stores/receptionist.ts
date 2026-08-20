import { computed, ref } from 'vue'
import { defineStore } from 'pinia'

import { http } from '@/lib/api/http'
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
}

function mapQueueStatus(status: string): QueuePatient['status'] {
  switch (status?.toLowerCase()) {
    case 'waiting':
      return 'waiting'
    case 'called':
    case 'inprogress':
      return 'examining'
    case 'completed':
      return 'completed'
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
    doctor: 'Bác sĩ',
    time: formatApiTime(item.checkInTime),
    status: mapQueueStatus(item.status),
  }
}

export const useReceptionistStore = defineStore(
  'receptionist',
  () => {
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

    async function fetchQueue(doctorId?: string) {
      queueLoading.value = true
      queueError.value = null

      try {
        let targetDoctorId = doctorId

        if (!targetDoctorId) {
          const doctorsRes = await http.get<{ items?: Array<{ id?: string | number }> }>(`/doctors`, {
            params: { pageNumber: 1, pageSize: 20 },
          })

          const doctorList = doctorsRes.data.items ?? []
          const firstDoctor = doctorList.find((item) => item.id != null)
          targetDoctorId = firstDoctor ? String(firstDoctor.id) : undefined
        }

        if (!targetDoctorId) {
          queue.value = queueData.map((patient) => ({ ...patient }))
          return
        }

        const { data } = await http.get<QueueApiItem[]>(`/doctors/${targetDoctorId}/queue`)
        queue.value = data.map((item) => toQueuePatient(item))
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
