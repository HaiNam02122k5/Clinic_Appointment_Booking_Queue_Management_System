import { http } from '@/lib/api/http'

import type {
  CreateSpecialtyRequest,
  GetSpecialtiesParams,
  PagedSpecialtiesResponse,
  Specialty,
  UpdateSpecialtyRequest,
} from './specialties.types'

// =================================
// API RESPONSE WRAPPER
// =================================

interface ApiResponse<T> {
  statusCode: string
  isSuccess: boolean
  errorMessages: string[]
  result: T
}

// =================================
// SPECIALTIES API
// =================================

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

  // GET /specialties/{specialtyId}

  get: (id: string) =>
    http
      .get<ApiResponse<Specialty>>(
        `/specialties/${id}`,
      )
      .then((r) => r.data.result),

  // POST /specialties

  create: (data: CreateSpecialtyRequest) =>
    http
      .post<ApiResponse<Specialty>>(
        '/specialties',
        data,
      )
      .then((r) => r.data.result),

  // PUT /specialties/{specialtyId}

  update: (
    id: string,
    data: UpdateSpecialtyRequest,
  ) =>
    http
      .put<ApiResponse<void>>(
        `/specialties/${id}`,
        data,
      )
      .then((r) => r.data.result),

  // PATCH /specialties/{specialtyId}/status

  updateStatus: (id: string) =>
    http
      .patch<ApiResponse<void>>(
        `/specialties/${id}/status`,
      )
      .then((r) => r.data.result),
}
