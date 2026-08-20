export interface Doctor {
  id: number | string
  name: string
  specialty: string
  room?: string
}

export interface AvailableSlot {
  id: number | string
  workScheduleId?: number | string
  time: string
  available: boolean
}

export interface Appointment {
  id: number
  doctorId: number | string
  doctorName: string
  specialty: string
  appointmentDate: string
  appointmentTime: string
  status: 'Pending' | 'Confirmed' | 'CheckedIn' | 'Completed' | 'Cancelled'
  queueNumber?: string
}

export interface CreateAppointmentRequest {
  doctorId?: number | string
  workScheduleId?: number | string
  appointmentDate?: string
  appointmentTime?: string
  timeSlot?: string
  reason?: string
  symptoms?: string
}

export interface QueueEntry {
  ticket: string
  patientName: string
  doctorId: number
  doctorName: string
  appointmentTime: string
  status: 'Waiting' | 'InProgress' | 'Completed' | 'Skipped'
  estimatedWaitMinutes: number
  position: number
  urgent: boolean
}

export interface QueueStatus {
  myTicket: string
  position: number
  estimatedWaitMinutes: number
  doctorName: string
  appointmentTime: string
  currentTicket: string
  entries: QueueEntry[]
}

export interface MedicalRecord {
  id: number
  examinationDate: string
  doctorName: string
  specialty: string
  diagnosis: string
  prescription: string
  note?: string
}

export interface PatientProfile {
  id?: number | string
  fullName?: string
  email?: string
  phoneNumber?: string
  address?: string
  dateOfBirth?: string
  gender?: number | string
  insuranceNumber?: string
  emergencyContact?: string
}