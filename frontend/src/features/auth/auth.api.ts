import { http } from '@/lib/api/http'
import type { LoginPayload, RegisterPayload, LoginResponse, AuthUser } from './auth.types'

export const authApi = {
  login(payload: LoginPayload): Promise<LoginResponse> {
    return http.post<LoginResponse>('/auth/login', payload).then((r) => r.data)
  },

  register(payload: RegisterPayload): Promise<LoginResponse> {
    return http.post<LoginResponse>('/auth/register', payload).then((r) => r.data)
  },

  getMe(): Promise<AuthUser> {
    return http.get<AuthUser>('/auth/me').then((r) => r.data)
  }
}