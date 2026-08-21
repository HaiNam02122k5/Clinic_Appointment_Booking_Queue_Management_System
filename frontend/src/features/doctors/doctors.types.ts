export type DoctorStatus = 'Active' | 'Inactive'
export type Gender = 'Male' | 'Female' | 'Other'
export type DoctorGender = Gender

export interface Specialty {
  id: string
  name: string
}

export interface Doctor {
  id: string
  fullName: string
  phoneNumber: string
  email: string
  gender: Gender
  licenseNumber: string
  qualification: string
  currentSpecialty: string
  experienceYears: number
  status: DoctorStatus
  biography?: string
}

export interface DoctorDetail extends Doctor {
  biography: string
  dateOfBirth?: string
  address?: string
}

export interface PagedDoctorsResponse {
  items: Doctor[]
  pageNumber: number
  pageSize: number
  totalCount: number
  totalPages: number
  hasPrevious: boolean
  hasNext: boolean
}

export interface GetDoctorsParams {
  Search?: string
  Qualification?: string
  SortBy?: string
  Status?: DoctorStatus
  SpecialtyId?: string
  OrderBy?: string
  PageNumber?: number
  PageSize?: number
}

export interface CreateDoctorRequest {
  hireDate: string
  licenseNumber: string
  qualification: string
  experienceYears: number
  status: DoctorStatus
  specialtyId: string
  email: string
  address: string
  username: string
  password: string
  fullName: string
  phoneNumber: string
  dateOfBirth: string
  gender: Gender
  biography: string
}

export interface UpdateDoctorRequest {
  fullName: string
  phoneNumber: string
  email: string
  dateOfBirth: string
  gender: Gender
  address: string
  licenseNumber: string
  qualification: string
  experienceYears: number
  biography: string
}

export interface WorkSchedule {
  id: string
  doctorId: string
  date: string
  startTime: string
  endTime: string
  patientLimit: number
  status: string
}

export interface DoctorSchedule<T = WorkSchedule> {
  doctorId: string
  doctorName: string
  schedules: T[]
  startDate: string
  endDate: string
}

export interface RequestedShift {
  id: string
  doctorId: string
  date: string
  startTime: string
  endTime: string
  patientLimit: number
  reason?: string | null
  status: string
}

export interface QueueTicket {
  id: string
  appointmentId: string
  queueNumber: number
  priority: boolean
  status: string
  checkInTime: string
  calledAt?: string | null
  patientName?: string | null
}

export interface SkipQueueResult {
  skippedTicket: QueueTicket
  nextCalledTicket?: QueueTicket | null
}

export interface AppointmentDetail {
  id: string
  patientId: string
  doctorId: string
  patientName: string
  doctorName: string
  timeSlot: string
  date: string
  reason: string
  status: string
  queueNumber?: string | null
  queueTime?: string | null
  medicalReport?: MedicalReport | null
  createdAt: string
}

export interface MedicalReport {
  id: string
  doctorName: string
  examDate: string
  symptoms?: string | null
  diagnosis?: string | null
  prescription?: string | null
  notes?: string | null
}

export interface ApiResponse<T> {
  statusCode: string
  isSuccess: boolean
  errorMessages: string[]
  result: T
}
