import { http } from '@/lib/api/http'

import type {
  GetUsersParams,
  PagedUsersResponse,
  User,
} from './users.types'

export const usersApi = {
  // GET /admin/users
  list: (params?: GetUsersParams) =>
    http
      .get<PagedUsersResponse>(
        '/admin/users',
        {
          params,
        },
      )
      .then((r) => r.data),

  // GET /admin/users/all-brief
  listAllBrief: () =>
    http
      .get<User[]>(
        '/admin/users/all-brief',
      )
      .then((r) => r.data),

  // GET /admin/users/{userId}
  get: (id: string) =>
    http
      .get<User>(
        `/admin/users/${id}`,
      )
      .then((r) => r.data),
}
