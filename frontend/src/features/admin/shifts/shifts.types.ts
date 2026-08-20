export type ShiftStatus = 'Active' | 'Cancelled'

export interface Shift {
  id: string
  doctorId: string
  date: string
  startTime: string
  endTime: string
  patientLimit: number
  status: ShiftStatus
}

export interface DoctorSchedule {
  doctorId: string
  doctorName: string
  schedules: Shift[]
  startDate: string
  endDate: string
}

export interface GetShiftsParams {
  StartDate: string
  EndDate: string
}

export interface CreateShiftRequest {
  date: string
  startTime: string
  endTime: string
  patientLimit: number
}

export interface UpdateShiftRequest {
  date: string
  startTime: string
  endTime: string
  patientLimitPerSlot: number
}

export interface CancelShiftRequest {
  reason: string
}

export interface ApiResponse<T> {
  statusCode: string
  isSuccess: boolean
  errorMessages: string[]
  result: T
}
