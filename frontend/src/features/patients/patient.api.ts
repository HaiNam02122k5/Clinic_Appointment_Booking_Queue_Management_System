import { env } from '@/config/env'
import { http } from '@/lib/api/http'
import {
  mockAppointments,
  mockAvailableSlots,
  mockDoctors,
  mockMedicalHistory,
  mockQueue,
} from '@/mock/clinic-data'
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
    if (env.enableMock) {
      const doctors = specialty
        ? mockDoctors.filter((doctor) =>
            doctor.specialty.toLowerCase().includes(specialty.toLowerCase()),
          )
        : mockDoctors

      return Promise.resolve(doctors)
    }

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
    if (env.enableMock) {
      return Promise.resolve(mockAvailableSlots[doctorId] ?? [])
    }

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
    if (env.enableMock) {
      const doctor =
        mockDoctors.find((item) => item.id === payload.doctorId) ?? mockDoctors[0]
      const fallbackDoctor: Doctor = doctor ?? {
        id: payload.doctorId,
        name: 'BS. Chưa xác định',
        specialty: 'Khác',
      }
      const nextAppointment: Appointment = {
        id: Date.now(),
        doctorId: payload.doctorId,
        doctorName: fallbackDoctor.name,
        specialty: fallbackDoctor.specialty,
        appointmentDate: payload.appointmentDate,
        appointmentTime: payload.appointmentTime,
        status: 'Pending',
        queueNumber: `A-${Math.floor(10 + Math.random() * 90)}`,
      }

      mockAppointments.unshift(nextAppointment)
      return Promise.resolve(nextAppointment)
    }

    return http
      .post<Appointment>('/appointments', payload)
      .then((res) => res.data)
  },

  // Lấy lịch hẹn sắp tới của bệnh nhân
  getMyAppointments(): Promise<Appointment[]> {
    if (env.enableMock) {
      return Promise.resolve(mockAppointments)
    }

    return http
      .get<Appointment[]>('/appointments/my')
      .then((res) => res.data)
  },

  // Hủy lịch
  cancelAppointment(id: number): Promise<void> {
    if (env.enableMock) {
      const target = mockAppointments.find((appointment) => appointment.id === id)
      if (target) {
        target.status = 'Cancelled'
      }
      return Promise.resolve(undefined)
    }

    return http
      .patch(`/appointments/${id}/cancel`)
      .then(() => undefined)
  },

  // Hàng đợi của bệnh nhân
  getMyQueue(): Promise<QueueStatus> {
    if (env.enableMock) {
      return Promise.resolve(mockQueue)
    }

    return http
      .get<QueueStatus>('/queue/my')
      .then((res) => res.data)
  },

  // Lịch sử khám
  getMedicalHistory(): Promise<MedicalRecord[]> {
    if (env.enableMock) {
      return Promise.resolve(mockMedicalHistory)
    }

    return http
      .get<MedicalRecord[]>('/medical-records/my')
      .then((res) => res.data)
  },
}