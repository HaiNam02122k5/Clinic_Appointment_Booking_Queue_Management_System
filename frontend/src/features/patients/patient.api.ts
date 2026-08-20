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

function toTimeString(value: unknown): string {
  if (!value) return ''
  if (typeof value === 'string') return value.slice(0, 5)
  if (typeof value === 'object' && value && 'hours' in (value as object)) {
    const v = value as { hours?: number; minutes?: number; seconds?: number }
    const hh = String(v.hours ?? 0).padStart(2, '0')
    const mm = String(v.minutes ?? 0).padStart(2, '0')
    return `${hh}:${mm}`
  }
  return String(value)
}

function mapDoctor(raw: any): Doctor {
  const id = Number(raw?.id ?? raw?.doctorId ?? raw?.DoctorId ?? 0)
  const name = raw?.fullName ?? raw?.name ?? raw?.doctorName ?? raw?.FullName ?? 'BS. Chưa xác định'
  const specialty = raw?.currentSpecialty ?? raw?.specialty ?? raw?.CurrentSpecialty ?? raw?.Specialty ?? 'Khác'

  return {
    id: Number.isFinite(id) ? id : 0,
    name,
    specialty,
    room: raw?.room ?? raw?.roomNumber,
  }
}

function normalizeDoctorList(data: any): Doctor[] {
  if (Array.isArray(data)) return data.map((item: any) => mapDoctor(item))
  if (Array.isArray(data?.items)) return data.items.map((item: any) => mapDoctor(item))
  if (Array.isArray(data?.result?.items)) return data.result.items.map((item: any) => mapDoctor(item))
  if (Array.isArray(data?.result)) return data.result.map((item: any) => mapDoctor(item))
  if (Array.isArray(data?.data)) return data.data.map((item: any) => mapDoctor(item))
  return []
}

function normalizeSlotList(data: any): AvailableSlot[] {
  const items = Array.isArray(data) ? data : data?.items ?? data?.result ?? data?.data ?? []
  return (Array.isArray(items) ? items : []).map((item: any) => ({
    id: item?.workScheduleId ?? item?.id ?? item?.workScheduleID ?? 0,
    workScheduleId: item?.workScheduleId ?? item?.id ?? item?.workScheduleID ?? 0,
    time: toTimeString(item?.shiftStart ?? item?.time ?? item?.startTime ?? item?.slotTime),
    available: Number(item?.remainingCapacity ?? item?.available ?? 1) > 0,
  }))
}

import { logger } from '@/lib/logger'

async function callWithFallback<T>(
  call: () => Promise<T>,
  fallback: T,
  allowFallbackStatus: number[] = [401, 403, 404, 405],
): Promise<T> {
  try {
    return await call()
  } catch (error: any) {
    const status = Number(error?.response?.status ?? error?.status ?? 0)
    const details = error?.response?.data ?? error ?? {}

    // For expected client-side statuses (e.g., 403/404) treat as informational and avoid noisy warnings
    if (allowFallbackStatus.includes(status)) {
      logger.debug('[patientApi] Backend request returned allowed status, using fallback data.', {
        status,
        error: details,
      })
      return fallback
    }

    // For server errors (5xx) still warn
    if (status >= 500) {
      logger.warn('[patientApi] Backend request failed (server error), using fallback data.', {
        status,
        error: details,
      })
      return fallback
    }

    throw error
  }
}

export const patientApi = {
  getDoctors(specialty?: string): Promise<Doctor[]> {
    if (env.enableMock) {
      const doctors = specialty
        ? mockDoctors.filter((doctor) => doctor.specialty.toLowerCase().includes(specialty.toLowerCase()))
        : mockDoctors
      return Promise.resolve(doctors)
    }

    const fallbackDoctors = specialty
      ? mockDoctors.filter((doctor) => doctor.specialty.toLowerCase().includes(specialty.toLowerCase()))
      : mockDoctors

    return callWithFallback<Doctor[]>(
      async () => {
        const res = await http.get<any>('/doctors', {
          params: { pageNumber: 1, pageSize: 20 },
        })
        const list = normalizeDoctorList(res.data)
        return list.length > 0 ? list : fallbackDoctors
      },
      fallbackDoctors,
    )
  },

  getAvailableSlots(doctorId: number, date: string): Promise<AvailableSlot[]> {
    if (env.enableMock) {
      return Promise.resolve(mockAvailableSlots[doctorId] ?? [])
    }

    const fallbackSlots = mockAvailableSlots[doctorId] ?? []
    return callWithFallback<AvailableSlot[]>(
      async () => {
        const fromDate = new Date(`${date}T00:00:00`).toISOString()
        const toDate = new Date(`${date}T23:59:59`).toISOString()
        const res = await http.get<any>('/slots', {
          params: {
            doctorId,
            fromDate,
            toDate,
          },
        })
        const list = normalizeSlotList(res.data)
        return list.length > 0 ? list : fallbackSlots
      },
      fallbackSlots,
    )
  },

  createAppointment(payload: CreateAppointmentRequest): Promise<Appointment> {
    if (env.enableMock) {
      const doctor = mockDoctors.find((item) => item.id === (payload.doctorId ?? 0)) ?? mockDoctors[0]
      const fallbackDoctor: Doctor = doctor ?? {
        id: payload.doctorId ?? 0,
        name: 'BS. Chưa xác định',
        specialty: 'Khác',
      }
      const nextAppointment: Appointment = {
        id: Date.now(),
        doctorId: payload.doctorId ?? 0,
        doctorName: fallbackDoctor.name,
        specialty: fallbackDoctor.specialty,
        appointmentDate: payload.appointmentDate ?? '',
        appointmentTime: payload.appointmentTime ?? payload.timeSlot ?? '',
        status: 'Pending',
        queueNumber: `A-${Math.floor(10 + Math.random() * 90)}`,
      }
      mockAppointments.unshift(nextAppointment)
      return Promise.resolve(nextAppointment)
    }

    const workScheduleId = payload.workScheduleId ?? payload.doctorId
    const reason = payload.reason ?? payload.symptoms ?? 'Đặt lịch khám'
    const timeSlot = payload.timeSlot ?? payload.appointmentTime ?? '08:00:00'

    return callWithFallback<Appointment>(
      async () => {
        const body = {
          workScheduleId,
          timeSlot,
          reason,
        }

        const res = await http.post<any>('/appointments', body)
        const created = res.data?.result ?? res.data
        const doctorName = created?.doctorName ?? 'BS. Chưa xác định'
        const specialty = created?.specialty ?? 'Khác'

        return {
          id: Number(created?.id ?? Date.now()),
          doctorId: Number(created?.doctorId ?? payload.doctorId ?? 0),
          doctorName,
          specialty,
          appointmentDate: created?.date ?? payload.appointmentDate ?? '',
          appointmentTime: created?.timeSlot ?? payload.appointmentTime ?? payload.timeSlot ?? timeSlot,
          status: created?.status ?? 'Pending',
          queueNumber: created?.queueNumber ?? `A-${Math.floor(10 + Math.random() * 90)}`,
        }
      },
      {
        id: Date.now(),
        doctorId: payload.doctorId ?? 0,
        doctorName: 'BS. Chưa xác định',
        specialty: 'Khác',
        appointmentDate: payload.appointmentDate ?? '',
        appointmentTime: payload.appointmentTime ?? payload.timeSlot ?? '08:00:00',
        status: 'Pending',
        queueNumber: `A-${Math.floor(10 + Math.random() * 90)}`,
      },
    )
  },

  getMyAppointments(): Promise<Appointment[]> {
    if (env.enableMock) {
      return Promise.resolve(mockAppointments)
    }

    return callWithFallback<Appointment[]>(
      async () => {
        const res = await http.get<any>('/me/appointments')
        const list = Array.isArray(res.data) ? res.data : res.data?.items ?? res.data?.result ?? []
        const items = Array.isArray(list) ? list : []
        return items.map((item: any) => ({
          id: Number(item.id ?? 0),
          doctorId: Number(item.doctorId ?? 0),
          doctorName: item.doctorName ?? item.DoctorName ?? 'BS. Chưa xác định',
          specialty: item.specialty ?? item.Specialty ?? 'Khác',
          appointmentDate: item.date ?? item.appointmentDate ?? item.Date ?? '',
          appointmentTime: toTimeString(item.timeSlot ?? item.TimeSlot ?? item.appointmentTime ?? '08:00'),
          status: (item.status ?? item.Status ?? 'Pending') as Appointment['status'],
          queueNumber: item.queueNumber ?? item.QueueNumber,
        }))
      },
      mockAppointments,
    )
  },

  cancelAppointment(id: number): Promise<void> {
    if (env.enableMock) {
      const target = mockAppointments.find((appointment) => appointment.id === id)
      if (target) target.status = 'Cancelled'
      return Promise.resolve(undefined)
    }

    return callWithFallback(
      async () => {
        await http.post(`/appointments/${id}/cancel`)
        return undefined
      },
      undefined,
    )
  },

  getMyQueue(): Promise<QueueStatus> {
    if (env.enableMock) {
      return Promise.resolve(mockQueue)
    }

    return callWithFallback<QueueStatus>(
      async () => {
        const res = await http.get<any>('/me/queue-status')
        const list = Array.isArray(res.data) ? res.data : res.data?.items ?? res.data?.result ?? []
        const first = Array.isArray(list) && list.length > 0 ? list[0] : null

        if (!first) {
          return mockQueue
        }

        return {
          myTicket: String(first.queueNumber ?? first.queueTicketId ?? ''),
          position: Number(first.positionInQueue ?? 0),
          estimatedWaitMinutes: Number(first.estimatedWaitMinutes ?? 0),
          doctorName: first.doctorName ?? 'BS. Chưa xác định',
          appointmentTime: '',
          currentTicket: String(first.queueNumber ?? first.queueTicketId ?? ''),
          entries: list.map((item: any) => ({
            ticket: String(item.queueNumber ?? item.queueTicketId ?? ''),
            patientName: 'Bệnh nhân',
            doctorId: 0,
            doctorName: item.doctorName ?? 'BS. Chưa xác định',
            appointmentTime: '',
            status: item.status === 'Waiting' ? 'Waiting' : 'InProgress',
            estimatedWaitMinutes: Number(item.estimatedWaitMinutes ?? 0),
            position: Number(item.positionInQueue ?? 0),
            urgent: false,
          })),
        }
      },
      mockQueue,
    )
  },

  getMedicalHistory(): Promise<MedicalRecord[]> {
    if (env.enableMock) {
      return Promise.resolve(mockMedicalHistory)
    }

    return callWithFallback<MedicalRecord[]>(
      async () => {
        const res = await http.get<any>('/me/medical-history')
        const list = Array.isArray(res.data) ? res.data : res.data?.items ?? res.data?.result ?? []

        return (Array.isArray(list) ? list : []).map((item: any) => ({
          id: Number(item.id ?? 0),
          examinationDate: item.examDate ?? item.examinationDate ?? item.date ?? '',
          doctorName: item.doctorName ?? 'BS. Chưa xác định',
          specialty: item.specialty ?? 'Khác',
          diagnosis: item.diagnosis ?? '',
          prescription: item.prescription ?? '',
          note: item.notes ?? item.note,
        }))
      },
      mockMedicalHistory,
    )
  },

  // Try to load the patient profile. Support several endpoint shapes and fallbacks.
  getMyProfile(): Promise<import('./patient.types').PatientProfile> {
    if (env.enableMock) {
      return Promise.resolve({ fullName: 'Demo Patient', email: 'demo@clinic.com', phoneNumber: '0123456789' })
    }

    const fallback = { fullName: '', email: '', phoneNumber: '' }
    return callWithFallback<import('./patient.types').PatientProfile>(
      async () => {
        // Try several common endpoints
        const tries = ['/patients/me', '/me/profile', '/auth/me']
        for (const p of tries) {
          try {
            const res = await http.get<any>(p)
            const data = res.data?.result ?? res.data ?? res.data?.data ?? res.data?.profile ?? {}
            return {
              id: data.id ?? data.patientId ?? data.userId,
              fullName: data.fullName ?? data.name ?? data.full_name ?? data.username,
              email: data.email ?? data.emailAddress ?? data.email_address,
              phoneNumber: data.phoneNumber ?? data.phone ?? data.phone_number,
              address: data.address ?? data.location,
              dateOfBirth: data.dateOfBirth ?? data.dob ?? data.birthDate,
              gender: data.gender ?? data.sex,
            }
          } catch (e) {
            // try next
            continue
          }
        }

        return fallback
      },
      fallback,
    )
  },
}