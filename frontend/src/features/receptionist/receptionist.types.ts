export interface QueueTicketDto {
  id: string
  queueNumber: number | string
  patientName?: string
  priority?: boolean
  status: 'Waiting' | 'Called' | 'InProgress' | 'Completed' | 'Skipped' | 'Cancelled' | string
  checkInTime?: string | null
  calledAt?: string | null
}

export interface ReceptionistDoctor {
  id: string
  fullName: string
  roomNumber?: string
  specialtyName?: string
}
