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

  const loading = ref(false)
  const error = ref<string | null>(null)

  const upcomingAppointments = computed(() =>
    appointments.value.filter(
      (a) =>
        a.status === 'Pending' ||
        a.status === 'Confirmed' ||
        a.status === 'CheckedIn',
    ),
  )

  async function loadDoctors(specialty?: string) {
    loading.value = true
    error.value = null

    try {
      doctors.value = await patientApi.getDoctors(specialty)
    } catch (e: any) {
      error.value =
        e.response?.data?.message ||
        'Không thể tải danh sách bác sĩ'
    } finally {
      loading.value = false
    }
  }

  async function loadSlots(doctorId: number, date: string) {
    loading.value = true
    error.value = null

    try {
      slots.value = await patientApi.getAvailableSlots(
        doctorId,
        date,
      )
    } catch (e: any) {
      error.value =
        e.response?.data?.message ||
        'Không thể tải khung giờ'
    } finally {
      loading.value = false
    }
  }

  async function createAppointment(
    payload: CreateAppointmentRequest,
  ) {
    loading.value = true
    error.value = null

    try {
      const appointment =
        await patientApi.createAppointment(payload)

      appointments.value.unshift(appointment)

      return appointment
    } catch (e: any) {
      error.value =
        e.response?.data?.message ||
        'Đặt lịch thất bại'

      throw e
    } finally {
      loading.value = false
    }
  }

  async function loadAppointments() {
    loading.value = true
    error.value = null

    try {
      appointments.value =
        await patientApi.getMyAppointments()
    } catch (e: any) {
      error.value =
        e.response?.data?.message ||
        'Không thể tải lịch hẹn'
    } finally {
      loading.value = false
    }
  }

  async function cancelAppointment(id: number) {
    loading.value = true
    error.value = null

    try {
      await patientApi.cancelAppointment(id)

      const appointment = appointments.value.find(
        (a) => a.id === id,
      )

      if (appointment) {
        appointment.status = 'Cancelled'
      }
    } catch (e: any) {
      error.value =
        e.response?.data?.message ||
        'Không thể hủy lịch'
      throw e
    } finally {
      loading.value = false
    }
  }

  async function loadQueue() {
    loading.value = true
    error.value = null

    try {
      queue.value = await patientApi.getMyQueue()
    } catch (e: any) {
      error.value =
        e.response?.data?.message ||
        'Không thể tải hàng đợi'
    } finally {
      loading.value = false
    }
  }

  async function loadHistory() {
    loading.value = true
    error.value = null

    try {
      history.value =
        await patientApi.getMedicalHistory()
    } catch (e: any) {
      error.value =
        e.response?.data?.message ||
        'Không thể tải lịch sử khám'
    } finally {
      loading.value = false
    }
  }

  return {
    doctors,
    slots,
    appointments,
    upcomingAppointments,
    queue,
    history,

    loading,
    error,

    loadDoctors,
    loadSlots,
    createAppointment,
    loadAppointments,
    cancelAppointment,
    loadQueue,
    loadHistory,
  }
})