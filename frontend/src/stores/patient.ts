import { defineStore } from 'pinia'
import { computed, ref } from 'vue'

import { patientApi } from '@/features/patients/patient.api'

import type {
  Doctor,
  AvailableSlot,
  Appointment,
  CreateAppointmentRequest,
  QueueStatus,
  MedicalRecord,
} from '@/features/patients/patient.types'

export const usePatientStore = defineStore('patient', () => {
  const doctors = ref<Doctor[]>([])
  const slots = ref<AvailableSlot[]>([])
  const appointments = ref<Appointment[]>([])
  const queue = ref<QueueStatus | null>(null)
  const history = ref<MedicalRecord[]>([])

  // Loading riêng
  const doctorsLoading = ref(false)
  const slotsLoading = ref(false)
  const appointmentsLoading = ref(false)
  const queueLoading = ref(false)
  const historyLoading = ref(false)

  // Error riêng
  const doctorsError = ref<string | null>(null)
  const slotsError = ref<string | null>(null)
  const appointmentsError = ref<string | null>(null)
  const queueError = ref<string | null>(null)
  const historyError = ref<string | null>(null)

  const upcomingAppointments = computed(() =>
    appointments.value.filter(
      (a) =>
        a.status === 'Pending' ||
        a.status === 'Confirmed' ||
        a.status === 'CheckedIn',
    ),
  )

  async function loadDoctors(specialty?: string) {
    doctorsLoading.value = true
    doctorsError.value = null

    try {
      doctors.value = await patientApi.getDoctors(specialty)
    } catch (e: any) {
      doctorsError.value =
        e.response?.data?.message ||
        'Không thể tải danh sách bác sĩ'
    } finally {
      doctorsLoading.value = false
    }
  }

  async function loadSlots(doctorId: number, date: string) {
    slotsLoading.value = true
    slotsError.value = null

    try {
      slots.value = await patientApi.getAvailableSlots(
        doctorId,
        date,
      )
    } catch (e: any) {
      slotsError.value =
        e.response?.data?.message ||
        'Không thể tải khung giờ'
    } finally {
      slotsLoading.value = false
    }
  }

  function clearSlots() {
    slots.value = []
  }

  async function createAppointment(
    payload: CreateAppointmentRequest,
  ) {
    appointmentsLoading.value = true
    appointmentsError.value = null

    try {
      const appointment =
        await patientApi.createAppointment(payload)

      appointments.value.unshift(appointment)

      return appointment
    } catch (e: any) {
      appointmentsError.value =
        e.response?.data?.message ||
        'Đặt lịch thất bại'

      throw e
    } finally {
      appointmentsLoading.value = false
    }
  }

  async function loadAppointments() {
    appointmentsLoading.value = true
    appointmentsError.value = null

    try {
      appointments.value =
        await patientApi.getMyAppointments()
    } catch (e: any) {
      appointmentsError.value =
        e.response?.data?.message ||
        'Không thể tải lịch hẹn'
    } finally {
      appointmentsLoading.value = false
    }
  }

  async function cancelAppointment(id: number) {
    appointmentsLoading.value = true
    appointmentsError.value = null

    try {
      await patientApi.cancelAppointment(id)

      const appointment = appointments.value.find(
        (a) => a.id === id,
      )

      if (appointment) {
        appointment.status = 'Cancelled'
      }
    } catch (e: any) {
      appointmentsError.value =
        e.response?.data?.message ||
        'Không thể hủy lịch'

      throw e
    } finally {
      appointmentsLoading.value = false
    }
  }

  async function loadQueue() {
    queueLoading.value = true
    queueError.value = null

    try {
      queue.value = await patientApi.getMyQueue()
    } catch (e: any) {
      queueError.value =
        e.response?.data?.message ||
        'Không thể tải hàng đợi'
    } finally {
      queueLoading.value = false
    }
  }

  async function loadHistory() {
    historyLoading.value = true
    historyError.value = null

    try {
      history.value =
        await patientApi.getMedicalHistory()
    } catch (e: any) {
      historyError.value =
        e.response?.data?.message ||
        'Không thể tải lịch sử khám'
    } finally {
      historyLoading.value = false
    }
  }

  return {
    // Data
    doctors,
    slots,
    appointments,
    upcomingAppointments,
    queue,
    history,

    // Loading
    doctorsLoading,
    slotsLoading,
    appointmentsLoading,
    queueLoading,
    historyLoading,

    // Error
    doctorsError,
    slotsError,
    appointmentsError,
    queueError,
    historyError,

    // Actions
    loadDoctors,
    loadSlots,
    clearSlots,
    createAppointment,
    loadAppointments,
    cancelAppointment,
    loadQueue,
    loadHistory,
  }
})