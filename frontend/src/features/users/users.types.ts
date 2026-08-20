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
  Role?: string
  IsActive?: boolean
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

export interface CreateReceptionistRequest {
  username: string
  password: string
  fullName: string
  phoneNumber: string
  email: string
  dateOfBirth: string
  gender: Gender
  address: string
  hireDate: string
  roles: string[]
}

export interface CreateDoctorRequest {
  hireDate: string
  licenseNumber: string
  qualification: string
  experienceYears: number
  status: 'Active' | 'Inactive'
  specialtyId: string

  email: string
  address: string
  username: string
  password: string
  fullName: string
  phoneNumber: string
  dateOfBirth: string
  gender: Gender

  biography?: string
}

export interface CreateAccountForm {
  role: 'Doctor' | 'Receptionist'

  username: string
  password: string

  fullName: string
  phoneNumber: string
  email: string
  dateOfBirth: string
  gender: Gender
  address: string

  hireDate: string

  specialtyId?: string
  licenseNumber?: string
  qualification?: string
  experienceYears?: string
  biography?: string
}

export interface Doctor {
  id: string
  fullName: string
  phoneNumber: string
  email: string
  gender: Gender
  licenseNumber: string
  qualification: string
  currentSpecialty: string
  experienceYears: number
  status: 'Active' | 'Inactive'
}

export interface Employee {
  id: string
  fullName: string
  phoneNumber: string
  email: string
  gender: Gender
  dateOfBirth: string
  status: 'Active' | 'OnLeave' | 'Resigned'
  roles: string[]
}

export interface PagedDoctorsResponse {
  items: Doctor[]
  pageNumber: number
  pageSize: number
  totalCount: number
  totalPages: number
  hasPrevious: boolean
  hasNext: boolean
}

export interface PagedEmployeesResponse {
  items: Employee[]
  pageNumber: number
  pageSize: number
  totalCount: number
  totalPages: number
  hasPrevious: boolean
  hasNext: boolean
}
