export type AccountRole =
  | 'admin'
  | 'doctor'
  | 'receptionist'
  | 'patient'

export type Gender =
  | 'Male'
  | 'Female'
  | 'Other'

export interface User {
  id: string
  username: string
  fullName: string
  phoneNumber: string
  email: string
  gender: Gender
  roles: string[]
  isActive: boolean
  createdAt: string
}

export interface GetUsersParams {
  Search?: string
  SortBy?: string
  OrderBy?: string
  Gender?: Gender
  PageNumber?: number
  PageSize?: number
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
