export type QueueStatus = 'waiting' | 'examining' | 'completed'

export interface QueuePatient {
  no: string
  name: string
  doctor: string
  time: string
  status: QueueStatus
}

export const queueData: QueuePatient[] = [
  {
    no: 'A-024',
    name: 'Nguyễn Văn An',
    doctor: 'BS. Lê Thu Hằng',
    time: '08:30',
    status: 'examining'
  },
  {
    no: 'A-025',
    name: 'Trần Thị Bích',
    doctor: 'BS. Lê Thu Hằng',
    time: '09:00',
    status: 'waiting'
  },
  {
    no: 'A-026',
    name: 'Lê Văn Cường',
    doctor: 'BS. Phạm Minh Tuấn',
    time: '09:00',
    status: 'waiting'
  },
  {
    no: 'A-027',
    name: 'Phạm Thị Dung',
    doctor: 'BS. Phạm Minh Tuấn',
    time: '09:30',
    status: 'waiting'
  },
  {
    no: 'A-028',
    name: 'Hoàng Văn Em',
    doctor: 'BS. Nguyễn Sơn',
    time: '09:30',
    status: 'waiting'
  }
]

export const receptionistStats = [
  {
    label: 'Tổng lịch hẹn',
    value: '156',
    color: '#7C3AED'
  },
  {
    label: 'Đã check-in',
    value: '89',
    color: '#0E4D92'
  },
  {
    label: 'Đang chờ',
    value: '23',
    color: '#D97706'
  },
  {
    label: 'Đã khám xong',
    value: '44',
    color: '#00A878'
  }
]

export interface AppointmentPatient {
  appointmentId: string
  phone: string
  name: string
  doctor: string
  appointmentTime: string
  specialty: string
  checkedIn: boolean
}

export const mockAppointments: AppointmentPatient[] = [
  {
    appointmentId: 'APT-001',
    phone: '0901234567',
    name: 'Nguyễn Văn An',
    doctor: 'BS. Lê Thu Hằng',
    appointmentTime: '08:30',
    specialty: 'Nội tổng quát',
    checkedIn: false
  },
  {
    appointmentId: 'APT-002',
    phone: '0912345678',
    name: 'Trần Thị Bích',
    doctor: 'BS. Lê Thu Hằng',
    appointmentTime: '09:00',
    specialty: 'Nội tổng quát',
    checkedIn: false
  },
  {
    appointmentId: 'APT-003',
    phone: '0987654321',
    name: 'Lê Văn Cường',
    doctor: 'BS. Phạm Minh Tuấn',
    appointmentTime: '09:00',
    specialty: 'Ngoại khoa',
    checkedIn: false
  }
]
