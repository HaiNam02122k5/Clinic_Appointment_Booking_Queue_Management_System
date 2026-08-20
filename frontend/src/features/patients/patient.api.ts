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

function mapDoctor(raw: any): Doctor {
  const id = Number(raw?.id ?? raw?.doctorId ?? raw?.DoctorId ?? 0)
  const name = raw?.fullName ?? raw?.name ?? raw?.doctorName ?? 'BS. Chưa xác định'
  const specialty =
    raw?.currentSpecialty ??
    raw?.specialty ??
    raw?.specialtyName ??
    raw?.CurrentSpecialty ??
    'Khác'

  return {
    id: Number.isFinite(id) ? id : 0,
    name,
    specialty,
    room: raw?.room ?? raw?.roomNumber,
  }
}

function normalizeDoctorList(data: any): Doctor[] {
  if (Array.isArray(data)) return data.map((item) => mapDoctor(item))
  if (Array.isArray(data?.items)) return data.items.map((item: any) => mapDoctor(item))
  if (Array.isArray(data?.result)) return data.result.map((item: any) => mapDoctor(item))
  if (Array.isArray(data?.data)) return data.data.map((item: any) => mapDoctor(item))
  return []
}

function normalizeSlotList(data: any): AvailableSlot[] {
  if (Array.isArray(data)) return data as AvailableSlot[]
  if (Array.isArray(data?.items)) return data.items as AvailableSlot[]
  if (Array.isArray(data?.result)) return data.result as AvailableSlot[]
  return []
}

async function callWithFallback<T>(
  call: () => Promise<T>,
  fallback: T,
  allowFallbackStatus: number[] = [401, 403, 404, 405],
): Promise<T> {
  try {
    return await call()
  } catch (error: any) {
    const status = Number(error?.response?.status ?? error?.status ?? 0)
    if (allowFallbackStatus.includes(status) || status >= 500) {
      return fallback
    }
    throw error
  }
}

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

    const fallbackDoctors = specialty
      ? mockDoctors.filter((doctor) =>
          doctor.specialty.toLowerCase().includes(specialty.toLowerCase()),
        )
      : mockDoctors

    return callWithFallback(
      async () => {
        const res = await http.get<any>('/doctors', {
          params: specialty ? { specialty } : undefined,
        })
        const list = normalizeDoctorList(res.data)
        return list.length > 0 ? list : fallbackDoctors
      },
      fallbackDoctors,
    )
  },

  // Lấy các giờ còn trống
  getAvailableSlots(
    doctorId: number,
    date: string,
  ): Promise<AvailableSlot[]> {
    if (env.enableMock) {
      return Promise.resolve(mockAvailableSlots[doctorId] ?? [])
    }

    const fallbackSlots = mockAvailableSlots[doctorId] ?? []
    return callWithFallback(
      async () => {
        const res = await http.get<any>(`/doctors/${doctorId}/shifts`, {
          params: { startDate: date, endDate: date },
        })
        const list = normalizeSlotList(res.data)
        return list.length > 0 ? list : fallbackSlots
      },
      fallbackSlots,
    )
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

    const doctor =
      mockDoctors.find((item) => item.id === payload.doctorId) ?? mockDoctors[0]
    const fallbackDoctor: Doctor = doctor ?? {
      id: payload.doctorId,
      name: 'BS. Chưa xác định',
      specialty: 'Khác',
    }

    return callWithFallback(
      async () => {
        const res = await http.post<any>('/appointments', payload)
        const data = res.data
        const created = data?.result ?? data

        if (created && typeof created === 'object') {
          return {
            id: Number(created.id ?? Date.now()),
            doctorId: Number(created.doctorId ?? payload.doctorId),
            doctorName: created.doctorName ?? fallbackDoctor.name,
            specialty: created.specialty ?? fallbackDoctor.specialty,
            appointmentDate: created.appointmentDate ?? payload.appointmentDate,
            appointmentTime: created.appointmentTime ?? payload.appointmentTime,
            status: created.status ?? 'Pending',
            queueNumber: created.queueNumber ?? `A-${Math.floor(10 + Math.random() * 90)}`,
          }
        }

        return {
          id: Date.now(),
          doctorId: payload.doctorId,
          doctorName: fallbackDoctor.name,
          specialty: fallbackDoctor.specialty,
          appointmentDate: payload.appointmentDate,
          appointmentTime: payload.appointmentTime,
          status: 'Pending',
          queueNumber: `A-${Math.floor(10 + Math.random() * 90)}`,
        }
      },
      {
        id: Date.now(),
        doctorId: payload.doctorId,
        doctorName: fallbackDoctor.name,
        specialty: fallbackDoctor.specialty,
        appointmentDate: payload.appointmentDate,
        appointmentTime: payload.appointmentTime,
        status: 'Pending',
        queueNumber: `A-${Math.floor(10 + Math.random() * 90)}`,
      },
    )
  },

  // Lấy lịch hẹn sắp tới của bệnh nhân
  getMyAppointments(): Promise<Appointment[]> {
    if (env.enableMock) {
      return Promise.resolve(mockAppointments)
    }

    return callWithFallback(
      async () => {
        const res = await http.get<any>('/me/appointments')
        const list = Array.isArray(res.data) ? res.data : res.data?.items ?? res.data?.result ?? []
        return Array.isArray(list) ? list : mockAppointments
      },
      mockAppointments,
    )
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

    return callWithFallback(
      async () => {
        await http.patch(`/appointments/${id}/cancel`)
        return undefined
      },
      undefined,
    )
  },

  // Hàng đợi của bệnh nhân
  getMyQueue(): Promise<QueueStatus> {
    if (env.enableMock) {
      return Promise.resolve(mockQueue)
    }

    return callWithFallback(
      async () => {
        const res = await http.get<any>('/queue/my')
        const data = res.data?.result ?? res.data
        return data ?? mockQueue
      },
      mockQueue,
    )
  },

  // Lịch sử khám
  getMedicalHistory(): Promise<MedicalRecord[]> {
    if (env.enableMock) {
      return Promise.resolve(mockMedicalHistory)
    }

    return callWithFallback(
      async () => {
        const res = await http.get<any>('/medical-records/my')
        const list = Array.isArray(res.data) ? res.data : res.data?.items ?? res.data?.result ?? []
        return Array.isArray(list) ? list : mockMedicalHistory
      },
      mockMedicalHistory,
    )
  },
}