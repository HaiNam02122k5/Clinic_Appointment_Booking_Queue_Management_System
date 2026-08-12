import { http } from '@/lib/api/http'

import type {
  DoctorDetail,
  DoctorsResponse,
  GetDoctorsParams,
} from './doctor.types'

export const doctorApi = {
  // =========================
  // GET /doctors
  // Admin + Receptionist
  // =========================

  async getDoctors(
    params?: GetDoctorsParams,
  ): Promise<DoctorsResponse> {
    const response =
      await http.get<DoctorsResponse>(
        '/doctors',
        {
          params,
        },
      )

    return response.data
  },

  // =========================
  // GET /doctors/{doctorId}
  // Admin
  // =========================

  async getDoctor(
    doctorId: number,
  ): Promise<DoctorDetail> {
    const response =
      await http.get<DoctorDetail>(
        `/doctors/${doctorId}`,
      )

    return response.data
  },
}
