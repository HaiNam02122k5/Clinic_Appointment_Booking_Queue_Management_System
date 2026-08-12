// =========================
// Doctor
// =========================

export type DoctorGender =
  | 'Male'
  | 'Female'

export type DoctorStatus =
  | 'ACTIVE'
  | 'INACTIVE'

export interface DoctorSpecialty {
  id: number
  name: string
}

// GET /doctors
export interface Doctor {
  id: number
  fullName: string
  specialty: DoctorSpecialty
  qualification: string
  gender: DoctorGender
  ExperienceYears: number
  dateOfBirth: string
  status: DoctorStatus
}

// GET /doctors/{doctorId}
export interface DoctorDetail
  extends Doctor {
  licenseNumber: string
  biography: string
}


// =========================
// Pagination
// =========================

export interface DoctorPagination {
  page: number
  pageSize: number
  totalItems: number
  totalPages: number
}


// =========================
// GET /doctors response
// =========================

export interface DoctorsResponse {
  items: Doctor[]
  pagination: DoctorPagination
}


// =========================
// GET /doctors query params
// =========================

export type DoctorSortBy =
  | 'Name'
  | 'ExperienceYears'
  | 'DateOfBirth'

export type DoctorOrderBy =
  | 'asc'
  | 'desc'

export interface GetDoctorsParams {
  sortBy?: DoctorSortBy
  orderBy?: DoctorOrderBy
  gender?: DoctorGender
  status?: DoctorStatus
  specialtyId?: number
  qualification?: string
  search?: string
  page?: number
  pageSize?: number
}
