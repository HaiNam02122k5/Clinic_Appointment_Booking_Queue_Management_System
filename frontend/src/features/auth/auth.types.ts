// phân quyền người dùng
export type UserRole = 'Patient' | 'Receptionist' | 'Doctor' | 'Admin' | 'TV'

export interface AuthUser {
  id: number
  name: string
  email: string
  role: UserRole
}

export interface LoginPayload {
  email: string
  password: string
}

export interface LoginResponse {
  accessToken: string
  refreshToken?: string
  user: AuthUser
}