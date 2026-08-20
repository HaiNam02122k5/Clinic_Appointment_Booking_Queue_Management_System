export interface Doctor {
  id: string
  fullName: string
  name?: string
  phoneNumber: string
  email: string
  gender: number
  currentSpecialty?: string
  specialty?: string
  specialtyId?: string
  licenseNumber: string
  qualification: string
  experienceYears: number
  biography?: string | null
  status: string | number
  room?: string
  dateOfBirth?: string
  address?: string
  avatarUrl?: string
  hireDate?: string
  rating?: number
}

export type DoctorDetail = Doctor

export interface CreateDoctorPayload {
  username: string
  password?: string
  fullName: string
  phoneNumber: string
  email: string
  dateOfBirth: string
  gender: number
  address: string
  hireDate?: string
  licenseNumber: string
  qualification: string
  experienceYears: number
  specialtyId: string
  biography?: string | null
  status?: number
}

export interface UpdateDoctorPayload {
  fullName: string
  phoneNumber: string
  email: string
  dateOfBirth: string
  gender: number
  address: string
  licenseNumber: string
  qualification: string
  experienceYears: number
  specialtyId?: string
  biography?: string | null
  status?: number
}

export interface GetDoctorsParams {
  search?: string
  specialtyId?: string
  status?: string | number
  sortBy?: string
  orderBy?: string
  gender?: number
  pageNumber?: number
  pageSize?: number
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
