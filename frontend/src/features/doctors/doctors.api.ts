import { http } from '@/lib/api/http'

import type {
  CreateDoctorRequest,
  DoctorDetail,
  GetDoctorsParams,
  PagedDoctorsResponse,
  UpdateDoctorRequest,
} from './doctors.types'

// =================================
// API RESPONSE WRAPPER
// =================================

interface ApiResponse<T> {
  statusCode: string
  isSuccess: boolean
  errorMessages: string[]
  result: T
}

export const doctorsApi = {
  // =================================
  // GET /doctors
  // =================================

  list: (params?: GetDoctorsParams) =>
    http
      .get<ApiResponse<PagedDoctorsResponse>>(
        '/doctors',
        {
          params,
        },
      )
      .then((r) => r.data.result),

  // =================================
  // GET /doctors/{doctorId}
  // =================================

  get: (id: string) =>
    http
      .get<ApiResponse<DoctorDetail>>(
        `/doctors/${id}`,
      )
      .then((r) => r.data.result),

  // =================================
  // POST /doctors
  // =================================

  create: (data: CreateDoctorRequest) =>
    http
      .post<ApiResponse<unknown>>(
        '/doctors',
        data,
      )
      .then((r) => r.data),

  // =================================
  // PUT /doctors/{doctorId}
  // =================================

  update: (
    id: string,
    data: UpdateDoctorRequest,
  ) =>
    http
      .put<ApiResponse<unknown>>(
        `/doctors/${id}`,
        data,
      )
      .then((r) => r.data),
}
