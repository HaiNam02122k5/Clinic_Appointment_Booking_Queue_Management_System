import { http } from '@/lib/api/http'

import type {
  ApiResponse,
  CancelShiftRequest,
  CreateShiftRequest,
  DoctorSchedule,
  GetShiftsParams,
  Shift,
  UpdateShiftRequest,
} from './shifts.types'

export const shiftsApi = {
  getAll: (params: GetShiftsParams) =>
    http
      .get<ApiResponse<DoctorSchedule[]>>(
        '/admin/shifts',
        {
          params,
        },
      )
      .then((r) => r.data.result),

  create: (
    doctorId: string,
    data: CreateShiftRequest,
  ) =>
    http
      .post<ApiResponse<Shift>>(
        `/doctors/${doctorId}/shifts`,
        data,
      )
      .then((r) => r.data.result),

  update: (
    shiftId: string,
    data: UpdateShiftRequest,
  ) =>
    http
      .put(
        `/shifts/${shiftId}`,
        data,
      )
      .then((r) => r.data),

  cancel: (
    shiftId: string,
    data: CancelShiftRequest,
  ) =>
    http
      .post(
        `/shifts/${shiftId}/cancel`,
        data,
      )
      .then((r) => r.data),
}
