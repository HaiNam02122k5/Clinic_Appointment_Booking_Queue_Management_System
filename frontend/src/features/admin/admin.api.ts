import { http } from '@/lib/api/http'

import type {
  ApiResponse,
  DashboardData,
  PagedAdminAppointments,
  StatisticsData,
  StatisticsPeriod,
} from './admin.types'

export const adminApi = {
  // GET /admin/dashboard
  getDashboard: () =>
    http
      .get<ApiResponse<DashboardData>>('/admin/dashboard')
      .then((r) => r.data),

  // GET /admin/statistics?period=Week
  getStatistics: (period: StatisticsPeriod) =>
    http
      .get<ApiResponse<StatisticsData>>('/admin/statistics', {
        params: { period },
      })
      .then((r) => r.data),

  // GET /admin/appointments
  getAppointments: (
    date: string,
    pageNumber = 1,
    pageSize = 5,
  ) =>
    http
      .get<ApiResponse<PagedAdminAppointments>>(
        '/admin/appointments',
        {
          params: {
            Date: date,
            PageNumber: pageNumber,
            PageSize: pageSize,
          },
        },
      )
      .then((r) => r.data),
}
