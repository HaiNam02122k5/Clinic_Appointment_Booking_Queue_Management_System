import { http } from '@/lib/api/http'

import type {
  Doctor,
} from './receptionist.types'

export const receptionistApi = {
  async getDoctors(): Promise<Doctor[]> {
    const response = await http.get<Doctor[]>(
      '/doctors',
    )

    return response.data
  },
}
