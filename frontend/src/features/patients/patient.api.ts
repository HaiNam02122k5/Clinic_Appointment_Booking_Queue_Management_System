import { http } from '@/lib/api/http'
import type {
  Doctor,
  AvailableSlot,
  Appointment,
  CreateAppointmentRequest,
  QueueStatus,
  MedicalRecord,
} from './patient.types'

export const patientApi = {
  // Lấy danh sách bác sĩ
  getDoctors(specialty?: string): Promise<Doctor[]> {
    return http
      .get<Doctor[]>('/doctors', {
        params: specialty ? { specialty } : undefined,
      })
      .then((res) => res.data)
  },

  // Lấy các giờ còn trống
  getAvailableSlots(
    doctorId: number,
    date: string,
  ): Promise<AvailableSlot[]> {
    return http
      .get<AvailableSlot[]>(`/doctors/${doctorId}/available-slots`, {
        params: { date },
      })
      .then((res) => res.data)
  },

  // Tạo lịch hẹn
  createAppointment(
    payload: CreateAppointmentRequest,
  ): Promise<Appointment> {
    return http
      .post<Appointment>('/appointments', payload)
      .then((res) => res.data)
  },

  // Lấy lịch hẹn sắp tới của bệnh nhân
  getMyAppointments(): Promise<Appointment[]> {
    return http
      .get<Appointment[]>('/appointments/my')
      .then((res) => res.data)
  },

  // Hủy lịch
  cancelAppointment(id: number): Promise<void> {
    return http
      .patch(`/appointments/${id}/cancel`)
      .then(() => undefined)
  },

  // Hàng đợi của bệnh nhân
  getMyQueue(): Promise<QueueStatus> {
    return http
      .get<QueueStatus>('/queue/my')
      .then((res) => res.data)
  },

  // Lịch sử khám
  getMedicalHistory(): Promise<MedicalRecord[]> {
    return http
      .get<MedicalRecord[]>('/medical-records/my')
      .then((res) => res.data)
  },
}