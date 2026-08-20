export type DoctorStatus =
  | 'Active'
  | 'Inactive'

export type Gender =
  | 'Male'
  | 'Female'

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
  status: DoctorStatus
}

export interface DoctorDetail
  extends Doctor {
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
  gender: Gender
  biography: string
}

export interface UpdateDoctorRequest {
  fullName: string
  phoneNumber: string
  email: string
  dateOfBirth: string
  gender: Gender
  address: string
  licenseNumber: string
  qualification: string
  experienceYears: number
  biography: string
}
