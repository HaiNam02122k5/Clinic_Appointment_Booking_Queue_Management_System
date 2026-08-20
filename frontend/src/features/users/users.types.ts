export type AccountRole =
  | 'admin'
  | 'doctor'
  | 'receptionist'
  | 'patient'

export interface User {
  id: string
  username: string
  fullName: string
  phoneNumber: string | null
  email: string | null
  gender: number
  roles: string[]
}

export interface PagedUsersResponse {
  items: User[]
  pageNumber: number
  pageSize: number
  totalCount: number
  totalPages: number
  hasPrevious: boolean
  hasNext: boolean
}

export interface GetUsersParams {
  search?: string
  sortBy?: string
  orderBy?: string
  gender?: number
  pageNumber?: number
  pageSize?: number
}
