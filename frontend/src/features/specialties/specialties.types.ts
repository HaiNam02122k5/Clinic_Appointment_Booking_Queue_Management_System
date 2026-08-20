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

export interface CreateSpecialtyRequest {
  name: string
  description: string
  establishedDate: string
}

export interface UpdateSpecialtyRequest {
  name: string
  description: string
  establishedDate: string
}
