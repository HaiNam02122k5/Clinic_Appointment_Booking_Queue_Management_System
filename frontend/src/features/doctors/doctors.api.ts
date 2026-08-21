import { http } from '@/lib/api/http'
import type {
  ApiResponse,
  AppointmentDetail,
  CreateDoctorRequest,
  Doctor,
  DoctorDetail,
  DoctorSchedule,
  GetDoctorsParams,
  PagedDoctorsResponse,
  QueueTicket,
  RequestedShift,
  SkipQueueResult,
  UpdateDoctorRequest,
  WorkSchedule,
} from './doctors.types'

const unwrap = <T>(data: ApiResponse<T> | T): T => {
  if (data && typeof data === 'object' && 'result' in data) {
    return (data as ApiResponse<T>).result
  }
  return data as T
}

export const doctorsApi = {
  list: (params?: GetDoctorsParams) =>
    http.get<ApiResponse<PagedDoctorsResponse>>('/doctors', { params }).then((r) => unwrap(r.data)),

  get: (id: string) =>
    http.get<ApiResponse<Doctor>>(`/doctors/${id}`).then((r) => unwrap(r.data)),

  create: (data: CreateDoctorRequest) =>
    http.post<ApiResponse<Doctor>>('/doctors', data).then((r) => unwrap(r.data)),

  update: (id: string, data: UpdateDoctorRequest) =>
    http.put<ApiResponse<unknown>>(`/doctors/${id}`, data).then((r) => unwrap(r.data)),

  getOwnProfile: () =>
    http.get<ApiResponse<DoctorDetail>>('/doctors/me').then((r) => unwrap(r.data)),

  updateOwnProfile: (data: UpdateDoctorRequest) =>
    http.put<ApiResponse<unknown>>('/doctors/me', data).then((r) => unwrap(r.data)),

  getOwnSchedule: (startDate: string, endDate: string) =>
    http
      .get<ApiResponse<DoctorSchedule<WorkSchedule>>>('/shifts', {
        params: { StartDate: startDate, EndDate: endDate },
      })
      .then((r) => unwrap(r.data)),

getOwnShiftRequests: (startDate: string, endDate: string) =>
  http
    .get<DoctorSchedule<RequestedShift>>('/shifts/suggestions', {
      params: {
        StartDate: startDate,
        EndDate: endDate,
      },
    })
    .then((r) => r.data),

createShiftRequest: (data: {
  date: string
  startTime: string
  endTime: string
  patientLimit: number
  reason: string
}) =>
  http
    .post<RequestedShift>('/shifts/suggestions', data)
    .then((r) => r.data),

  updateShiftRequest: (
    id: string,
    data: {
      date: string
      startTime: string
      endTime: string
      patientLimitPerSlot: number
      reason: string
    },
  ) =>
    http.patch<ApiResponse<unknown>>(`/shifts/suggestions/${id}`, data).then((r) => unwrap(r.data)),

  cancelShiftRequest: (id: string) =>
    http.post<ApiResponse<unknown>>(`/shifts/suggestions/${id}/cancel`).then((r) => unwrap(r.data)),

  getQueue: (doctorId: string) =>
    http.get<ApiResponse<QueueTicket[]>>(`/doctors/${doctorId}/queue`).then((r) => unwrap(r.data)),

  callNext: (doctorId: string) =>
    http.post<ApiResponse<QueueTicket>>(`/doctors/${doctorId}/queue/next`).then((r) => unwrap(r.data)),

  startExam: (queueTicketId: string) =>
    http.post<ApiResponse<unknown>>(`/queue/${queueTicketId}/start-exam`).then((r) => unwrap(r.data)),

  completeExam: (queueTicketId: string) =>
    http.post<ApiResponse<unknown>>(`/queue/${queueTicketId}/complete-exam`).then((r) => unwrap(r.data)),

  skip: (queueTicketId: string) =>
    http.post<ApiResponse<SkipQueueResult>>(`/queue/${queueTicketId}/skip`).then((r) => unwrap(r.data)),

  setPriority: (queueTicketId: string, priority: boolean) =>
    http
      .patch<ApiResponse<unknown>>(`/queue/${queueTicketId}/priority`, { priority })
      .then((r) => unwrap(r.data)),

  getAppointment: (appointmentId: string) =>
    http
      .get<ApiResponse<AppointmentDetail>>(`/appointments/${appointmentId}`)
      .then((r) => unwrap(r.data)),
}
