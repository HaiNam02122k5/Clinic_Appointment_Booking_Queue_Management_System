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
  list: async (params?: GetUsersParams): Promise<PagedUsersResponse> => {
    try {
      const res = await http
        .get<any>('/admin/users', {
          params: {
            Search: params?.search || undefined,
            SortBy: params?.sortBy || 'fullName',
            OrderBy: params?.orderBy || 'asc',
            Gender: params?.gender,
            PageNumber: params?.pageNumber ?? 1,
            PageSize: params?.pageSize ?? 10,
          },
        })
        .then((r) => r.data)

      const data = res?.result !== undefined ? res.result : res
      const rawItems = data?.items || data?.Items || (Array.isArray(data) ? data : [])
      const items: User[] = rawItems.map((u: any) => ({
        id: String(u.userId || u.UserId || u.id || u.Id || ''),
        username: u.username || u.Username || '',
        fullName: u.fullName || u.FullName || u.name || u.Name || '',
        name: u.fullName || u.FullName || u.name || u.Name || '',
        phoneNumber: u.phoneNumber || u.PhoneNumber || null,
        email: u.email || u.Email || null,
        gender: typeof u.gender === 'string' ? (u.gender.toLowerCase() === 'female' ? 1 : 0) : (u.gender ?? 0),
        roles: Array.isArray(u.roles) ? u.roles : (Array.isArray(u.Roles) ? u.Roles : ['Patient']),
        isActive: u.isActive ?? u.IsActive ?? true,
        createdAt: u.createdAt || u.CreatedAt || '',
      }))

      const pageNumber = data?.pageNumber || data?.PageNumber || params?.pageNumber || 1
      const pageSize = data?.pageSize || data?.PageSize || params?.pageSize || 10
      const totalCount = data?.totalCount ?? data?.TotalCount ?? items.length
      const totalPages = data?.totalPages || data?.TotalPages || Math.ceil(totalCount / pageSize) || 1

      return {
        items,
        pageNumber,
        pageSize,
        totalCount,
        totalPages,
        hasPrevious: pageNumber > 1,
        hasNext: pageNumber < totalPages,
      }
    } catch {
      try {
        const briefList = await usersApi.listAllBrief()
        return {
          items: briefList,
          pageNumber: 1,
          pageSize: briefList.length || 10,
          totalCount: briefList.length,
          totalPages: 1,
          hasPrevious: false,
          hasNext: false,
        }
      } catch {
        return {
          items: [],
          pageNumber: 1,
          pageSize: 10,
          totalCount: 0,
          totalPages: 1,
          hasPrevious: false,
          hasNext: false,
        }
      }
    }
  },

  // GET /admin/users/all-brief
  listAllBrief: async (): Promise<User[]> => {
    const res = await http.get<any>('/admin/users/all-brief').then((r) => r.data)
    const data = res?.result !== undefined ? res.result : res
    const rawList = Array.isArray(data) ? data : (data?.items || [])

    return rawList.map((u: any) => ({
      id: String(u.userId || u.UserId || u.id || u.Id || ''),
      username: u.username || u.Username || '',
      fullName: u.fullName || u.FullName || '',
      name: u.fullName || u.FullName || '',
      roles: Array.isArray(u.roles) ? u.roles : (Array.isArray(u.Roles) ? u.Roles : ['Patient']),
      isActive: u.isActive ?? u.IsActive ?? true,
    }))
  },

  // GET /admin/users/{userId}
  get: async (id: string): Promise<User> => {
    const res = await http.get<any>(`/admin/users/${id}`).then((r) => r.data)
    const u = res?.result !== undefined ? res.result : res
    return {
      id: String(u.userId || u.UserId || u.id || u.Id || ''),
      username: u.username || u.Username || '',
      fullName: u.fullName || u.FullName || u.name || u.Name || '',
      name: u.fullName || u.FullName || u.name || u.Name || '',
      phoneNumber: u.phoneNumber || u.PhoneNumber || null,
      email: u.email || u.Email || null,
      gender: typeof u.gender === 'string' ? (u.gender.toLowerCase() === 'female' ? 1 : 0) : (u.gender ?? 0),
      roles: Array.isArray(u.roles) ? u.roles : (Array.isArray(u.Roles) ? u.Roles : ['Patient']),
      isActive: u.isActive ?? u.IsActive ?? true,
      createdAt: u.createdAt || u.CreatedAt || '',
    }
  },

  // POST /employees (or /auth/register for Patient)
  create: (input: CreateUserInput) => {
    if (input.role?.toLowerCase() === 'patient') {
      return http
        .post('/auth/register', {
          username: input.username,
          password: input.password,
          fullName: input.fullName,
          phoneNumber: input.phoneNumber || '0901234567',
          email: input.email || undefined,
        })
        .then((r) => r.data?.result ?? r.data)
    }

    const payload = {
      username: input.username,
      password: input.password,
      fullName: input.fullName,
      phoneNumber: input.phoneNumber || '0901234567',
      email: input.email || `${input.username.toLowerCase()}@clinic.vn`,
      dateOfBirth: '2000-01-01',
      gender: 0,
      address: 'Cơ sở khám chữa bệnh',
      hireDate: new Date().toISOString().split('T')[0],
      roles: [input.role || 'Receptionist'],
    }
    return http.post<any>('/employees', payload).then((r) => r.data?.result ?? r.data)
  },

  // PUT /employees/{id}
  update: (id: string, input: UpdateUserInput) => {
    const payload = {
      fullName: input.fullName,
      phoneNumber: input.phoneNumber || '0901234567',
      email: input.email || undefined,
      dateOfBirth: '2000-01-01',
      gender: 0,
      address: 'Cơ sở khám chữa bệnh',
      roles: [input.role || 'Receptionist'],
    }
    return http.put<any>(`/employees/${id}`, payload).then((r) => r.data?.result ?? r.data)
  },

  // DELETE /admin/users/{id}
  remove: (id: string) =>
    http
      .delete(`/admin/users/${id}`)
      .then((r) => r.data),
}
