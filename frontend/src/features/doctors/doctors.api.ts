import { http } from '@/lib/api/http'
import type {
  Doctor,
  DoctorDetail,
  GetDoctorsParams,
  PagedDoctorsResponse,
  CreateDoctorPayload,
  UpdateDoctorPayload,
} from './doctors.types'

export const doctorsApi = {
  // GET /doctors
  async list(params?: GetDoctorsParams): Promise<PagedDoctorsResponse> {
    const res = await http
      .get<any>('/doctors', {
        params: {
          Search: params?.search || undefined,
          SpecialtyId: params?.specialtyId || undefined,
          SortBy: params?.sortBy || 'fullName',
          OrderBy: params?.orderBy || 'asc',
          Gender: params?.gender,
          Status: params?.status !== undefined && params?.status !== '' && params?.status !== 'all' ? params.status : undefined,
          PageNumber: params?.pageNumber ?? 1,
          PageSize: params?.pageSize ?? 10,
        },
      })
      .then((r) => r.data)

    const data = res?.result !== undefined ? res.result : res
    const rawItems = data?.items || data?.Items || (Array.isArray(data) ? data : [])
    const items: Doctor[] = rawItems.map((d: any) => ({
      id: String(d.id || d.Id || ''),
      name: d.fullName || d.FullName || d.name || d.Name || '',
      fullName: d.fullName || d.FullName || d.name || d.Name || '',
      specialty: d.currentSpecialty || d.CurrentSpecialty || d.specialty || '',
      specialtyId: d.specialtyId || d.SpecialtyId || '',
      experienceYears: d.experienceYears ?? d.ExperienceYears ?? 0,
      rating: d.rating ?? d.Rating ?? 5.0,
      avatarUrl: d.avatarUrl || d.AvatarUrl || '',
      email: d.email || d.Email || '',
      phoneNumber: d.phoneNumber || d.PhoneNumber || '',
      licenseNumber: d.licenseNumber || d.LicenseNumber || '',
      qualification: d.qualification || d.Qualification || '',
      biography: d.biography || d.Biography || '',
      status: d.status ?? d.Status ?? 0,
      dateOfBirth: d.dateOfBirth || d.DateOfBirth || '',
      gender: typeof d.gender === 'string' ? (d.gender.toLowerCase() === 'female' ? 1 : 0) : (d.gender ?? 0),
    }))

    const pageNumber = data?.pageNumber || data?.PageNumber || params?.pageNumber || 1
    const pageSize = data?.pageSize || data?.PageSize || params?.pageSize || 10
    const totalCount = data?.totalCount ?? data?.TotalCount ?? items.length
    const totalPages = data?.totalPages || data?.TotalPages || Math.ceil(totalCount / pageSize) || 1

    return {
      items,
      pageNumber,
      pageSize,
      totalCount,
      totalPages,
      hasPrevious: pageNumber > 1,
      hasNext: pageNumber < totalPages,
    }
  },

  // GET /doctors/{doctorId}
  async getById(id: string): Promise<DoctorDetail> {
    const res = await http.get<any>(`/doctors/${id}`).then((r) => r.data)
    const d = res?.result !== undefined ? res.result : res
    return {
      id: String(d.id || d.Id || ''),
      name: d.fullName || d.FullName || d.name || d.Name || '',
      fullName: d.fullName || d.FullName || d.name || d.Name || '',
      specialty: d.currentSpecialty || d.CurrentSpecialty || d.specialty || '',
      specialtyId: d.specialtyId || d.SpecialtyId || '',
      experienceYears: d.experienceYears ?? d.ExperienceYears ?? 0,
      rating: d.rating ?? d.Rating ?? 5.0,
      avatarUrl: d.avatarUrl || d.AvatarUrl || '',
      email: d.email || d.Email || '',
      phoneNumber: d.phoneNumber || d.PhoneNumber || '',
      licenseNumber: d.licenseNumber || d.LicenseNumber || '',
      qualification: d.qualification || d.Qualification || '',
      biography: d.biography || d.Biography || '',
      status: d.status ?? d.Status ?? 0,
      dateOfBirth: d.dateOfBirth || d.DateOfBirth || '',
      gender: typeof d.gender === 'string' ? (d.gender.toLowerCase() === 'female' ? 1 : 0) : (d.gender ?? 0),
      address: d.address || d.Address || '',
      hireDate: d.hireDate || d.HireDate || '',
    }
  },

  // GET /doctors/me
  async getMyProfile(): Promise<DoctorDetail> {
    const res = await http.get<any>('/doctors/me').then((r) => r.data)
    const d = res?.result !== undefined ? res.result : res
    return {
      id: String(d.id || d.Id || ''),
      name: d.fullName || d.FullName || d.name || d.Name || '',
      fullName: d.fullName || d.FullName || d.name || d.Name || '',
      specialty: d.currentSpecialty || d.CurrentSpecialty || d.specialty || '',
      specialtyId: d.specialtyId || d.SpecialtyId || '',
      experienceYears: d.experienceYears ?? d.ExperienceYears ?? 0,
      rating: d.rating ?? d.Rating ?? 5.0,
      avatarUrl: d.avatarUrl || d.AvatarUrl || '',
      email: d.email || d.Email || '',
      phoneNumber: d.phoneNumber || d.PhoneNumber || '',
      licenseNumber: d.licenseNumber || d.LicenseNumber || '',
      qualification: d.qualification || d.Qualification || '',
      biography: d.biography || d.Biography || '',
      status: d.status ?? d.Status ?? 0,
      dateOfBirth: d.dateOfBirth || d.DateOfBirth || '',
      gender: typeof d.gender === 'string' ? (d.gender.toLowerCase() === 'female' ? 1 : 0) : (d.gender ?? 0),
      address: d.address || d.Address || '',
      hireDate: d.hireDate || d.HireDate || '',
    }
  },

  // POST /doctors
  async create(payload: CreateDoctorPayload): Promise<any> {
    const res = await http
      .post<any>('/doctors', {
        username: payload.username,
        password: payload.password,
        fullName: payload.fullName,
        phoneNumber: payload.phoneNumber,
        email: payload.email,
        dateOfBirth: payload.dateOfBirth,
        gender: payload.gender,
        address: payload.address,
        hireDate: new Date().toISOString().split('T')[0],
        licenseNumber: payload.licenseNumber,
        qualification: payload.qualification,
        biography: payload.biography || null,
        experienceYears: Number(payload.experienceYears) || 0,
        status: 0,
        specialtyId: payload.specialtyId,
      })
      .then((r) => r.data)
    return res?.result !== undefined ? res.result : res
  },

  // PUT /doctors/{doctorId}
  async update(id: string, payload: UpdateDoctorPayload): Promise<void> {
    await http.put(`/doctors/${id}`, {
      fullName: payload.fullName,
      phoneNumber: payload.phoneNumber,
      email: payload.email,
      dateOfBirth: payload.dateOfBirth || '1990-01-01',
      gender: payload.gender ?? 0,
      address: payload.address || 'Cơ sở khám chữa bệnh',
      licenseNumber: payload.licenseNumber,
      qualification: payload.qualification || 'Bác sĩ chuyên khoa',
      experienceYears: Number(payload.experienceYears) || 1,
      biography: payload.biography || null,
    })
  },

  // DELETE /doctors/{doctorId} -> Chuyển trạng thái sang Ngừng hoạt động (Inactive = 1) qua PATCH /doctors/{doctorId}/status
  async delete(id: string): Promise<void> {
    await http.patch(`/doctors/${id}/status`, 1)
  },

  // PATCH /doctors/{doctorId}/status
  async updateStatus(id: string, status: number): Promise<void> {
    await http.patch(`/doctors/${id}/status`, status)
  },
}
