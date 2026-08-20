export interface Doctor {
  id: string
  fullName: string
  phoneNumber: string
  email: string
  gender: number
  licenseNumber: string
  qualification: string
  currentSpecialty: string
  experienceYears: number
  status: number
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
  search?: string
  qualification?: string
  sortBy?: string
  status?: number
  specialtyId?: string
  orderBy?: string
  pageNumber?: number
  pageSize?: number
}
