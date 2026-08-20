export interface ApiResponse<T> {
  statusCode: string
  isSuccess: boolean
  errorMessages: string[]
  result: T
}

export interface DashboardMetric {
  value: number
  change: number
}

export interface DashboardData {
  totalAppointments: DashboardMetric
  totalPatients: DashboardMetric
  totalDoctors: DashboardMetric
  activeDoctor: DashboardMetric
  completionRate: DashboardMetric
  averageWaitingMinute: DashboardMetric
}

export type StatisticsPeriod = 'Week' | 'Month'

export interface AppointmentByDay {
  date: string
  count: number
}

export interface AppointmentByStatus {
  status: string
  count: number
}

export interface StatisticsData {
  period: string
  appointmentsByDay: AppointmentByDay[]
  appointmentsByStatus: AppointmentByStatus[]
}

export type AppointmentStatus =
  | 'completed'
  | 'waiting'
  | 'cancelled'
  | 'absent'

export interface AdminAppointment {
  id: string
  patient: string
  doctor: string
  specialty: string
  datetime: string
  status: AppointmentStatus
}

export interface PagedAdminAppointments {
  items: AdminAppointment[]
  pageNumber: number
  pageSize: number
  totalCount: number
  totalPages: number
  hasPrevious: boolean
  hasNext: boolean
}
