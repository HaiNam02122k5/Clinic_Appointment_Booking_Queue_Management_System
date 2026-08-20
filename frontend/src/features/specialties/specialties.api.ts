import { http } from '@/lib/api/http'

import type {
  ApiResponse,
  GetSpecialtiesParams,
  PagedSpecialtiesResponse,
} from '@/features/doctors/doctors.types'

export const specialtiesApi = {
  // GET /specialties
  list: (params?: GetSpecialtiesParams) =>
    http
      .get<ApiResponse<PagedSpecialtiesResponse>>(
        '/specialties',
        {
          params,
        },
      )
      .then((r) => r.data.result),
}
