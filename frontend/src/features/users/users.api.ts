import { http } from '@/lib/api/http'
import type {
  CreateUserInput,
  GetUsersParams,
  PagedUsersResponse,
  UpdateUserInput,
  User,
} from './users.types'

export const usersApi = {
  // GET /admin/users
  list: (params?: GetUsersParams) =>
    http
      .get<PagedUsersResponse>('/admin/users', { params })
      .then((r) => r.data),

  // GET /admin/users/all-brief
  listAllBrief: () =>
    http
      .get<User[]>('/admin/users/all-brief')
      .then((r) => r.data),

  // GET /admin/users/{userId}
  get: (id: string) =>
    http
      .get<User>(`/admin/users/${id}`)
      .then((r) => r.data),

  // POST /employees (or create user)
  create: (input: CreateUserInput) =>
    http
      .post<User>('/employees', input)
      .then((r) => r.data),

  // PUT /employees/{id}
  update: (id: string, input: UpdateUserInput) =>
    http
      .put<User>(`/employees/${id}`, input)
      .then((r) => r.data),

  // DELETE /admin/users/{id}
  remove: (id: string) =>
    http
      .delete(`/admin/users/${id}`)
      .then((r) => r.data),
}
