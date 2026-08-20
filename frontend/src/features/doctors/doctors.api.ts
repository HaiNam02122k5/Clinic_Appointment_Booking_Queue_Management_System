import { http } from '@/lib/api/http'

import type {
  Doctor,
  GetDoctorsParams,
  PagedDoctorsResponse,
} from './doctors.types'

export const doctorsApi = {
  // GET /doctors
  list: (params?: GetDoctorsParams) =>
    http
      .get<PagedDoctorsResponse>('/doctors', {
        params,
      })
      .then((r) => r.data),

  // GET /doctors/{doctorId}
  get: (id: string) =>
    http
      .get<Doctor>(`/doctors/${id}`)
      .then((r) => r.data),
}
