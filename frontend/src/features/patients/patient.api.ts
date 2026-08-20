import { env } from '@/config/env'
import { http } from '@/lib/api/http'
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

  if (typeof value === 'string') {
    const text = value.trim()
    if (!text) return ''

    const timeMatch = text.match(/(\d{1,2}:\d{2})(?::\d{2})?(?:\.\d+)?(?:Z|[+-]\d{2}:?\d{2})?$/)
    if (timeMatch?.[1]) return timeMatch[1]

    const isoMatch = text.match(/T(\d{1,2}):(\d{2})(?::(\d{2}))?(?:\.\d+)?(?:Z|[+-]\d{2}:?\d{2})?$/)
    if (isoMatch) {
      const hh = String(Number(isoMatch[1])).padStart(2, '0')
      const mm = String(Number(isoMatch[2])).padStart(2, '0')
      return `${hh}:${mm}`
    }

    const date = new Date(text)
    if (!Number.isNaN(date.getTime())) {
      return date.toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit', hour12: false })
    }

    return text.slice(0, 5)
  }

  if (typeof value === 'object' && value && 'hours' in (value as object)) {
    const v = value as { hours?: number; minutes?: number; seconds?: number }
    const hh = String(v.hours ?? 0).padStart(2, '0')
    const mm = String(v.minutes ?? 0).padStart(2, '0')
    return `${hh}:${mm}`
  }

  const raw = String(value)
  if (raw.includes(':')) {
    return raw.slice(0, 5)
  }

  return raw
}

const PATIENT_PROTECTED_ENDPOINTS_DISABLED_KEY = 'clinic.patient.protected-disabled'

function isProtectedPatientEndpointsDisabled(): boolean {
  try {
    return localStorage.getItem(PATIENT_PROTECTED_ENDPOINTS_DISABLED_KEY) === '1'
  } catch {
    return false
  }
}

function disableProtectedPatientEndpoints(): void {
  try {
    localStorage.setItem(PATIENT_PROTECTED_ENDPOINTS_DISABLED_KEY, '1')
  } catch {
    // ignore storage issues in private browsing or restricted environments
  }
}

// Allow re-enabling protected endpoints (for users/devs who want to retry backend connections)
export function enableProtectedPatientEndpoints(): void {
  try {
    localStorage.removeItem(PATIENT_PROTECTED_ENDPOINTS_DISABLED_KEY)
  } catch {
    // ignore
  }
}

export function areProtectedPatientEndpointsDisabled(): boolean {
  return isProtectedPatientEndpointsDisabled()
}

function shouldSkipProtectedPatientRequest(): boolean {
  return isProtectedPatientEndpointsDisabled()
}

function normalizeId(value: unknown): number | string {
  if (typeof value === 'number' && Number.isFinite(value)) return value
  if (typeof value === 'string') {
    const trimmed = value.trim()
    if (!trimmed) return 0
    const numeric = Number(trimmed)
    if (!Number.isNaN(numeric) && String(numeric) === trimmed) return numeric
    return trimmed
  }
  if (value == null) return 0

  const str = String(value).trim()
  if (!str) return 0
  const numeric = Number(str)
  return Number.isNaN(numeric) ? str : numeric
}

function normalizeDisplayDate(value: unknown): string {
  if (value === null || value === undefined) return ''

  const raw = String(value).trim()
  if (!raw) return ''

  const ddmmyyyy = /^([0-3]?\d)\/(0?[1-9]|1[0-2])\/(\d{4})$/.exec(raw)
  if (ddmmyyyy) {
    const day = Number(ddmmyyyy[1])
    const month = Number(ddmmyyyy[2])
    const year = Number(ddmmyyyy[3])
    return `${String(day).padStart(2, '0')}/${String(month).padStart(2, '0')}/${year}`
  }

  const ymd = /^(\d{4})-(\d{1,2})-(\d{1,2})$/.exec(raw)
  if (ymd) {
    const year = Number(ymd[1])
    const month = Number(ymd[2])
    const day = Number(ymd[3])
    const date = new Date(year, month - 1, day)
    if (!Number.isNaN(date.getTime())) {
      return `${String(day).padStart(2, '0')}/${String(month).padStart(2, '0')}/${year}`
    }
  }

  const parsed = new Date(raw)
  if (Number.isNaN(parsed.getTime())) {
    return raw
  }

  const year = parsed.getFullYear()
  const month = parsed.getMonth() + 1
  const day = parsed.getDate()
  return `${String(day).padStart(2, '0')}/${String(month).padStart(2, '0')}/${year}`
}

function normalizeAppointmentStatus(value: unknown): Appointment['status'] | string {
  const raw = String(value ?? 'Pending').trim()
  if (!raw) return 'Pending'

  const normalized = raw
    .toLowerCase()
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')
    .replace(/\s+/g, ' ')
    .trim()

  if (normalized.includes('confirm') || normalized.includes('xac') || normalized === 'confirmed') return 'Confirmed'
  if (normalized.includes('pend') || normalized.includes('wait') || normalized.includes('cho') || normalized === 'pending') return 'Pending'
  if (normalized.includes('cancel') || normalized.includes('huy') || normalized === 'cancelled' || normalized === 'canceled') return 'Cancelled'
  if (normalized.includes('check') || normalized.includes('den')) return 'CheckedIn'
  if (normalized.includes('complete') || normalized.includes('hoan')) return 'Completed'
  return raw as Appointment['status']
}

function mapDoctor(raw: any): Doctor {
  const id = normalizeId(raw?.id ?? raw?.doctorId ?? raw?.DoctorId ?? raw?.doctorID ?? raw?.Id ?? 0)
  const name = raw?.fullName ?? raw?.name ?? raw?.doctorName ?? raw?.FullName ?? 'BS. Chưa xác định'
  const specialty = raw?.currentSpecialty ?? raw?.specialty ?? raw?.CurrentSpecialty ?? raw?.Specialty ?? 'Khác'

  return {
    id,
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
  const collection = Array.isArray(data) ? data : data?.items ?? data?.result ?? data?.data ?? []
  const items = Array.isArray(collection) ? collection : []

  const slots: AvailableSlot[] = []

  for (const item of items) {
    const scheduleId = item?.id ?? item?.workScheduleId ?? item?.workScheduleID ?? item?.Id ?? 0
    const rawTimeSlots = Array.isArray(item?.timeSlot)
      ? item.timeSlot
      : Array.isArray(item?.TimeSlot)
        ? item.TimeSlot
        : []

    if (rawTimeSlots.length > 0) {
      rawTimeSlots.forEach((slot: any, index: number) => {
        const time = toTimeString(slot ?? item?.time ?? item?.startTime ?? item?.StartTime ?? item?.slotTime)
        const slotId = `${String(scheduleId)}-${index}-${time || index}`
        slots.push({
          id: slotId,
          workScheduleId: scheduleId,
          time,
          available: true,
        })
      })
      continue
    }

    const time = toTimeString(item?.shiftStart ?? item?.time ?? item?.startTime ?? item?.StartTime ?? item?.slotTime ?? item?.TimeSlot)
    if (!time) continue

    slots.push({
      id: scheduleId || time,
      workScheduleId: scheduleId,
      time,
      available: Number(item?.remainingCapacity ?? item?.available ?? 1) > 0,
    })
  }

  return slots
}

function normalizeGenderValue(value: unknown): number {
  if (typeof value === 'number' && Number.isFinite(value)) return value

  if (typeof value === 'string') {
    const normalized = value.trim().toLowerCase()
    if (normalized === 'male') return 0
    if (normalized === 'female') return 1
    if (normalized === 'other') return 2

    const numeric = Number(value)
    if (Number.isFinite(numeric)) return numeric
  }

  return 0
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
    return callWithFallback<Doctor[]>(
      async () => {
        const res = await http.get<any>('/doctors', {
          params: { pageNumber: 1, pageSize: 20 },
        })
        const list = normalizeDoctorList(res.data)
        return list
      },
      [],
    )
  },

  getAvailableSlots(doctorId: number | string, date: string): Promise<AvailableSlot[]> {
    return callWithFallback<AvailableSlot[]>(
      async () => {
        const res = await http.get<any>(`/doctors/${doctorId}/available`, {
          params: {
            date,
          },
        })

        const list = normalizeSlotList(res.data)
        return list
      },
      [],
    )
  },

  createAppointment(payload: CreateAppointmentRequest): Promise<Appointment> {
    const workScheduleId = payload.workScheduleId
    const reason = payload.reason ?? payload.symptoms ?? 'Đặt lịch khám'
    const timeSlot = payload.timeSlot ?? payload.appointmentTime ?? '08:00:00'

    if (!workScheduleId) {
      throw new Error('WorkScheduleId is required to create an appointment.')
    }

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
          id: normalizeId(created?.id ?? created?.appointmentId ?? Date.now()),
          doctorId: normalizeId(created?.doctorId ?? payload.doctorId ?? 0),
          doctorName,
          specialty,
          appointmentDate: normalizeDisplayDate(created?.date ?? created?.appointmentDate ?? payload.appointmentDate ?? ''),
          appointmentTime: created?.timeSlot ?? created?.appointmentTime ?? payload.appointmentTime ?? payload.timeSlot ?? timeSlot,
          status: normalizeAppointmentStatus(created?.status ?? 'Pending') as Appointment['status'],
          queueNumber: created?.queueNumber ?? `A-${Math.floor(10 + Math.random() * 90)}`,
        }
      },
      {
        id: normalizeId(Date.now()),
        doctorId: normalizeId(payload.doctorId ?? 0),
        doctorName: 'BS. Chưa xác định',
        specialty: 'Khác',
        appointmentDate: normalizeDisplayDate(payload.appointmentDate ?? ''),
        appointmentTime: payload.appointmentTime ?? payload.timeSlot ?? '08:00:00',
        status: 'Pending',
        queueNumber: `A-${Math.floor(10 + Math.random() * 90)}`,
      },
    )
  },

  getMyAppointments(): Promise<Appointment[]> {
    if (shouldSkipProtectedPatientRequest()) {
      return Promise.resolve([])
    }

    return callWithFallback<Appointment[]>(
      async () => {
        try {
          const res = await http.get<any>('/me/appointments')
          const list = Array.isArray(res.data) ? res.data : res.data?.items ?? res.data?.result ?? []
          const items = Array.isArray(list) ? list : []
          return items.map((item: any) => {
            // Handle various field name formats from backend
            const id = normalizeId(item.id ?? item.Id ?? item.appointmentId ?? item.AppointmentId ?? 0)
            const doctorId = normalizeId(item.doctorId ?? item.DoctorId ?? 0)
            const doctorName = item.doctorName ?? item.DoctorName ?? 'BS. Chưa xác định'

            // Backend doesn't return specialty, so use fallback
            const specialty = item.specialty ?? item.Specialty ?? 'Khác'

            // Handle date field - backend returns 'Date' as DateOnly string (YYYY-MM-DD)
            const appointmentDate = normalizeDisplayDate(item.appointmentDate ?? item.date ?? item.Date ?? item.appointmentDateDisplay ?? '')

            // Handle timeSlot - backend returns 'TimeSlot' as TimeOnly or time string
            const appointmentTime = toTimeString(item.appointmentTime ?? item.timeSlot ?? item.TimeSlot ?? '08:00')

            // Handle status - can be string or enum
            const status = normalizeAppointmentStatus(item.status ?? item.Status ?? 'Pending') as Appointment['status']

            // Backend doesn't include queueNumber, but it can be added if needed
            const queueNumber = item.queueNumber ?? item.QueueNumber ?? ''

            return {
              id,
              doctorId,
              doctorName,
              specialty,
              appointmentDate,
              appointmentTime,
              status,
              queueNumber,
            }
          })
        } catch (error: any) {
          const status = Number(error?.response?.status ?? error?.status ?? 0)
          if (status === 403 || status === 404) {
            disableProtectedPatientEndpoints()
          }
          throw error
        }
      },
      [],
    )
  },

  cancelAppointment(id: number | string): Promise<void> {
    return callWithFallback(
      async () => {
        await http.post(`/appointments/${id}/cancel`)
        return undefined
      },
      undefined,
    )
  },

  getMyQueue(): Promise<QueueStatus> {
    if (shouldSkipProtectedPatientRequest()) {
      return Promise.resolve({
        myTicket: '',
        position: 0,
        estimatedWaitMinutes: 0,
        doctorName: 'BS. Chưa xác định',
        appointmentTime: '',
        currentTicket: '',
        entries: [],
      })
    }

    return callWithFallback<QueueStatus>(
      async () => {
        try {
          const res = await http.get<any>('/me/queue-status')
          const list = Array.isArray(res.data) ? res.data : res.data?.items ?? res.data?.result ?? []
          const first = Array.isArray(list) && list.length > 0 ? list[0] : null

          if (!first) {
            return {
              myTicket: '',
              position: 0,
              estimatedWaitMinutes: 0,
              doctorName: 'BS. Chưa xác định',
              appointmentTime: '',
              currentTicket: '',
              entries: [],
            }
          }

          // Handle various field name formats from backend
          // Backend returns: QueueTicketId, QueueNumber, Status, DoctorName, PositionInQueue, EstimatedWaitMinutes
          const myTicket = String(first.queueNumber ?? first.QueueNumber ?? first.queueTicketId ?? first.QueueTicketId ?? '')
          const position = Number(first.positionInQueue ?? first.PositionInQueue ?? 0)
          const estimatedWaitMinutes = Number(first.estimatedWaitMinutes ?? first.EstimatedWaitMinutes ?? 0)
          const doctorName = first.doctorName ?? first.DoctorName ?? 'BS. Chưa xác định'

          // currentTicket can be same as myTicket or the first ticket in queue
          const currentTicket = myTicket

          return {
            myTicket,
            position,
            estimatedWaitMinutes,
            doctorName,
            appointmentTime: '',  // Backend doesn't provide this, frontend can leave empty
            currentTicket,
            entries: list.map((item: any) => ({
              ticket: String(item.queueNumber ?? item.QueueNumber ?? ''),
              patientName: 'Bệnh nhân',
              doctorId: 0,
              doctorName: item.doctorName ?? item.DoctorName ?? 'BS. Chưa xác định',
              appointmentTime: '',
              status: (item.status ?? item.Status ?? 'Waiting') === 'Waiting' ? 'Waiting' : 'InProgress',
              estimatedWaitMinutes: Number(item.estimatedWaitMinutes ?? item.EstimatedWaitMinutes ?? 0),
              position: Number(item.positionInQueue ?? item.PositionInQueue ?? 0),
              urgent: false,
            })),
          }
        } catch (error: any) {
          const status = Number(error?.response?.status ?? error?.status ?? 0)
          if (status === 403 || status === 404) {
            disableProtectedPatientEndpoints()
          }
          throw error
        }
      },
      {
        myTicket: '',
        position: 0,
        estimatedWaitMinutes: 0,
        doctorName: 'BS. Chưa xác định',
        appointmentTime: '',
        currentTicket: '',
        entries: [],
      },
    )
  },

  getMedicalHistory(): Promise<MedicalRecord[]> {
    if (shouldSkipProtectedPatientRequest()) {
      return Promise.resolve([])
    }

    return callWithFallback<MedicalRecord[]>(
      async () => {
        try {
          const res = await http.get<any>('/me/medical-history')
          const list = Array.isArray(res.data) ? res.data : res.data?.items ?? res.data?.result ?? []

          return (Array.isArray(list) ? list : []).map((item: any) => {
            // Handle various field name formats from backend
            // Backend returns: Id, DoctorName, ExamDate, Symptoms, Diagnosis, Prescription, Notes
            const id = Number(item.id ?? item.Id ?? 0)
          const examDate = normalizeDisplayDate(item.examDate ?? item.ExamDate ?? item.examinationDate ?? item.date ?? '')
            const doctorName = item.doctorName ?? item.DoctorName ?? 'BS. Chưa xác định'

            // Backend doesn't return specialty, so use fallback
            const specialty = item.specialty ?? item.Specialty ?? 'Khác'

            const diagnosis = item.diagnosis ?? item.Diagnosis ?? ''
            const prescription = item.prescription ?? item.Prescription ?? ''

            // Handle notes field - backend returns 'Notes' (plural), frontend expects 'note' (singular)
            const note = item.note ?? item.notes ?? item.Notes ?? ''

            return {
              id,
              examinationDate: examDate,
              doctorName,
              specialty,
              diagnosis,
              prescription,
              note,
            }
          })
        } catch (error: any) {
          const status = Number(error?.response?.status ?? error?.status ?? 0)
          if (status === 403 || status === 404) {
            disableProtectedPatientEndpoints()
          }
          throw error
        }
      },
      [],
    )
  },

  // Try to load the patient profile. Support several endpoint shapes and fallbacks.
  getMyProfile(): Promise<import('./patient.types').PatientProfile> {
    const fallback = { fullName: '', email: '', phoneNumber: '' }
    if (shouldSkipProtectedPatientRequest()) {
      return Promise.resolve(fallback)
    }

    return callWithFallback<import('./patient.types').PatientProfile>(
      async () => {
        // Try several common endpoints - /patients/me is the primary one
        const tries = ['/patients/me']
        for (const p of tries) {
          try {
            const res = await http.get<any>(p)
            const data = res.data?.result ?? res.data ?? res.data?.data ?? res.data?.profile ?? {}
            const dateOfBirthRaw = data.dateOfBirth ?? data.DateOfBirth ?? data.dob ?? data.Dob ?? data.birthDate ?? data.BirthDate
            return {
              id: data.id ?? data.Id ?? data.patientId ?? data.PatientId ?? data.userId ?? data.UserId,
              fullName: data.fullName ?? data.FullName ?? data.name ?? data.Name ?? data.full_name ?? data.username,
              email: data.email ?? data.Email ?? data.emailAddress ?? data.EmailAddress ?? data.email_address,
              phoneNumber: data.phoneNumber ?? data.PhoneNumber ?? data.phone ?? data.Phone ?? data.phone_number,
              address: data.address ?? data.Address ?? data.location ?? data.Location,
              dateOfBirth: dateOfBirthRaw,
              dateOfBirthDisplay: normalizeDisplayDate(dateOfBirthRaw),
              gender: normalizeGenderValue(data.gender ?? data.Gender ?? data.sex ?? data.Sex),
              insuranceNumber: data.insuranceNumber ?? data.InsuranceNumber ?? data.insurance_number ?? data.Insurance_Number,
              emergencyContact: data.emergencyContact ?? data.EmergencyContact ?? data.emergency_contact ?? data.Emergency_Contact,
            }
          } catch (e: any) {
            const status = e?.response?.status
            if (status === 403 || status === 404) {
              disableProtectedPatientEndpoints()
              return fallback
            }
            continue
          }
        }

        return fallback
      },
      fallback,
    )
  },
}