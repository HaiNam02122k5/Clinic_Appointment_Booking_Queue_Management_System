import { http } from '@/lib/api/http'

import type {
  PagedEmployeesResponse,
  PagedDoctorsResponse,
  CreateDoctorRequest,
  CreateReceptionistRequest,
  GetUsersParams,
  PagedUsersResponse,
  User,
} from './users.types'

// =================================
// API RESPONSE WRAPPER
// =================================

interface ApiResponse<T> {
  statusCode: string
  isSuccess: boolean
  errorMessages: string[]
  result: T
}

export const usersApi = {
  // =================================
  // ADMIN USERS
  // =================================

  // GET /admin/users
  list: (params?: GetUsersParams) =>
    http
      .get<ApiResponse<PagedUsersResponse>>(
        '/admin/users',
        {
          params,
        },
      )
      .then((r) => r.data.result),

  // GET /admin/users/all-brief
  listAllBrief: () =>
    http
      .get<ApiResponse<User[]>>(
        '/admin/users/all-brief',
      )
      .then((r) => r.data.result),

  // GET /admin/users/{userId}
  get: (id: string) =>
    http
      .get<ApiResponse<User>>(
        `/admin/users/${id}`,
      )
      .then((r) => r.data.result),

  // PATCH /admin/users/{userId}/status
  updateStatus: (
    id: string,
    isActive: boolean,
  ) =>
    http
      .patch(
        `/admin/users/${id}/status`,
        {
          isActive,
        },
      )
      .then((r) => r.data),

  // =================================
  // CREATE EMPLOYEE / RECEPTIONIST
  // =================================

  // POST /employees
  createReceptionist: (
    data: CreateReceptionistRequest,
  ) =>
    http
      .post(
        '/employees',
        data,
      )
      .then((r) => r.data),

  // =================================
  // CREATE DOCTOR
  // =================================

  // POST /doctors
  createDoctor: (
    data: CreateDoctorRequest,
  ) =>
    http
      .post(
        '/doctors',
        data,
      )
      .then((r) => r.data),

  // =================================
  // DOCTORS
  // =================================

  // GET /doctors
  listDoctors: () =>
    http
      .get<ApiResponse<PagedDoctorsResponse>>(
        '/doctors',
        {
          params: {
            PageNumber: 1,
            PageSize: 100,
          },
        },
      )
      .then((r) => r.data.result),

  // =================================
  // EMPLOYEES
  // =================================

  // GET /employees
  listEmployees: () =>
    http
      .get<ApiResponse<PagedEmployeesResponse>>(
        '/employees',
        {
          params: {
            PageNumber: 1,
            PageSize: 100,
          },
        },
      )
      .then((r) => r.data.result),
}
