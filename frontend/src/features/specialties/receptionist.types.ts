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

export interface ApiResponse<T> {
  statusCode: string
  isSuccess: boolean
  errorMessages: string[]
  result: T
}
