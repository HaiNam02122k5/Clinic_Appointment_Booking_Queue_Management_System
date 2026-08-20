import { http } from '@/lib/api/http'

import type {
  ApiResponse,
  CreateDoctorRequest,
  Doctor,
  DoctorDetail,
  GetDoctorsParams,
  PagedDoctorsResponse,
  UpdateDoctorRequest,
} from './doctors.types'

export const doctorsApi = {
  // GET /doctors
  list: (params?: GetDoctorsParams) =>
    http
      .get<ApiResponse<PagedDoctorsResponse>>('/doctors', {
        params,
      })
      .then((r) => r.data.result),

  // GET /doctors/{doctorId}
  get: (id: string) =>
    http
      .get<ApiResponse<DoctorDetail>>(`/doctors/${id}`)
      .then((r) => r.data.result),

  // POST /doctors
  create: (data: CreateDoctorRequest) =>
    http
      .post<ApiResponse<Doctor>>('/doctors', data)
      .then((r) => r.data.result),

  // PUT /doctors/{doctorId}
  update: (
    id: string,
    data: UpdateDoctorRequest,
  ) =>
    http
      .put(`/doctors/${id}`, data)
}
