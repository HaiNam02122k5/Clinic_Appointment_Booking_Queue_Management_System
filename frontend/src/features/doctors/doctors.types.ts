export type DoctorGender =
  | 'Male'
  | 'Female'
  | 'Other'

export type DoctorStatus =
  | 'Active'
  | 'Inactive'

export interface Doctor {
  id: string
  fullName: string
  phoneNumber: string
  email: string
  gender: DoctorGender
  licenseNumber: string
  qualification: string
  currentSpecialty: string
  experienceYears: number
  status: DoctorStatus
  biography?: string
}

export interface DoctorDetail extends Doctor {
  biography: string
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

export interface GetDoctorsParams {
  Search?: string
  Qualification?: string
  SortBy?: string
  Status?: DoctorStatus
  SpecialtyId?: string
  OrderBy?: string
  PageNumber?: number
  PageSize?: number
}

export interface ApiResponse<T> {
  statusCode: string
  isSuccess: boolean
  errorMessages: string[]
  result: T
}

export interface CreateDoctorRequest {
  hireDate: string
  licenseNumber: string
  qualification: string
  experienceYears: number
  status: DoctorStatus
  specialtyId: string

  email: string
  address: string
  username: string
  password: string
  fullName: string
  phoneNumber: string
  dateOfBirth: string
  gender: DoctorGender
  biography: string
}

export interface UpdateDoctorRequest {
  fullName: string
  phoneNumber: string
  email: string
  dateOfBirth: string
  gender: DoctorGender
  address: string
  licenseNumber: string
  qualification: string
  experienceYears: number
  biography: string
}

export interface Specialty {
  id: string
  name: string
  description: string
  establishedDate: string
}

export interface PagedSpecialtiesResponse {
  items: Specialty[]
  pageNumber: number
  pageSize: number
  totalCount: number
  totalPages: number
  hasPrevious: boolean
  hasNext: boolean
}

export interface GetSpecialtiesParams {
  Search?: string
  SortBy?: string
  Descending?: boolean
  Page?: number
  PageSize?: number
}
