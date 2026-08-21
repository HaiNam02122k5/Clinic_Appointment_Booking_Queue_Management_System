export interface WorkSchedule {
  id: string
  doctorId: string
  doctorName?: string
  specialtyName?: string
  date: string
  startTime: string
  endTime: string
  patientLimit: number
  status: 'Active' | 'Cancelled' | number | string
  remainingCapacity?: number
}

export interface DoctorScheduleResponse {
  doctorId: string
  doctorName: string
  schedules: WorkSchedule[]
  startDate: string
  endDate: string
}

export interface CreateShiftPayload {
  doctorId: string
  date: string
  startTime: string
  endTime: string
  patientLimit: number
}

export interface UpdateShiftPayload {
  date: string
  startTime: string
  endTime: string
  patientLimitPerSlot: number
}

export interface CancelShiftPayload {
  reason: string
}

export interface DoctorScheduleRow {
  doctorId: string
  doctorName: string
  specialty: string
  // Array of 7 days (Monday to Sunday) containing array of shifts for that day
  dayShifts: WorkSchedule[][]
}
