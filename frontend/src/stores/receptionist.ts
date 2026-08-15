import { computed, ref } from 'vue'
import { defineStore } from 'pinia'

import {
  mockAppointments,
  queueData,
  type AppointmentPatient,
  type QueuePatient,
} from '@/features/receptionist/receptionist.mock'

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

    // ================================
    // CALL PATIENT
    // ================================

    function callPatient(no: string) {
      const patient = queue.value.find(
        (item) => item.no === no
      )

      if (!patient) {
        return
      }

      if (patient.status !== 'waiting') {
        return
      }

      patient.status = 'examining'
    }

    // ================================
    // COMPLETE PATIENT
    // ================================

    function completePatient(no: string) {
      const patient = queue.value.find(
        (item) => item.no === no
      )

      if (!patient) {
        return
      }

      if (patient.status !== 'examining') {
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

      waitingCount,
      examiningCount,
      completedCount,

      findAppointment,
      checkIn,
      callPatient,
      completePatient,

      resetMockData,
    }
  }
)
